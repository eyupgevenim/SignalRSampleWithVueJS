using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;
using User.API.Data;
using User.API.Data.Entities.Identity;
using User.API.Data.Repository;
using User.API.Hubs;

namespace User.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomDatabaseConfiguration(Configuration)
                .AddCustomIdentityServer(Configuration)
                .AddCustomJwtBearer(Configuration)
                .AddCustomSwaggerGen(Configuration)
                .AddCustomServices(Configuration);

            services.AddCors();
            services.AddSignalR();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User.API v1"));
            }

            // Enable CORS so the Vue client can send requests
            app.UseCors(builder => 
            {
                var origins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins(origins)
                    //.SetIsOriginAllowed((host) => true) //todo:?
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Register SignalR hubs
                endpoints.MapHub<ChatHub>("/chat-hub");// The URL passed can be named whatever you'd like
                endpoints.MapHub<ChatHub>("/chat-hub-jwt");// The URL passed can be named whatever you'd like
            });

            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }

    static class ServiceCollectionExtensions
    {
        public const string CookieAuthScheme = "CookieAuthScheme";
        public const string JWTAuthScheme = "JWTAuthScheme";

        public static IServiceCollection AddCustomDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionStrings = configuration.GetSection(nameof(AppSettings.ConnectionStrings)).Get<ConnectionStringsOptions>();

            Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = (sqlOptions) =>
            {
                sqlOptions.MigrationsAssembly(migrationsAssembly);
                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            };
            // Add EF services to the services container.
            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(connectionStrings.DefaultConnection, sqlServerOptionsAction: sqlServerOptionsAction));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }

        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            // Adds IdentityServer
            services
                .AddIdentity<User.API.Data.Entities.Identity.User, Role>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 0;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.AllowedUserNameCharacters = "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+'#!/^%{}*";
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddCustomJwtBearer(this IServiceCollection services, IConfiguration configuration)
        {
            // Sets the default scheme to cookies
            services.AddAuthentication(CookieAuthScheme)
                // Now configure specific Cookie and JWT auth options
                .AddCookie(CookieAuthScheme, options =>
                {
                    // Set the cookie
                    options.Cookie.Name = "soSignalR.AuthCookie";
                    // Set the samesite cookie parameter as none, otherwise CORS scenarios where the client uses a different domain wont work!
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                    // Simply return 401 responses when authentication fails (as opposed to default redirecting behaviour)
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                    // In order to decide the between both schemas
                    // inspect whether there is a JWT token either in the header or query string
                    options.ForwardDefaultSelector = ctx =>
                    {
                        if (ctx.Request.Query.ContainsKey("access_token")) return JWTAuthScheme;
                        if (ctx.Request.Headers.ContainsKey("Authorization")) return JWTAuthScheme;
                        return CookieAuthScheme;
                    };
                })
                .AddJwtBearer(JWTAuthScheme, options =>
                {
                    var jwtOptions = configuration.GetSection(nameof(AppSettings.Jwt)).Get<JwtOptions>();
                    var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(jwtOptions.IssuerSigningKey));
                    // Configure JWT Bearer Auth to expect our security key
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        LifetimeValidator = (before, expires, token, param) =>
                        {
                            return expires > DateTime.UtcNow;
                        },
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateActor = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = securityKey,
                    };

                    // The JwtBearer scheme knows how to extract the token from the Authorization header
                    // but we will need to manually extract it from the query string in the case of requests to the hub
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            if (ctx.Request.Query.ContainsKey("access_token"))
                            {
                                ctx.Token = ctx.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "User.API", Version = "v1" });

                options.DescribeAllParametersInCamelCase();


                //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1295
                //https://stackoverflow.com/questions/58197244/swaggerui-with-netcore-3-0-bearer-token-authorization
                //options.OperationFilter<TokenOperationFilter>();
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                options.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securitySchema, new[] { "Bearer" });
                options.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<AppSettings>(configuration)
                .Configure<ConnectionStringsOptions>(configuration.GetSection(nameof(AppSettings.ConnectionStrings)))
                .Configure<JwtOptions>(configuration.GetSection(nameof(AppSettings.Jwt)));

            //TODO:...

            return services;
        }

    }
}
