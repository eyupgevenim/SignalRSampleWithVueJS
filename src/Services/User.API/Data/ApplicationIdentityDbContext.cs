namespace User.API.Data
{
    using User.API.Data.Entities.Identity;
    using User.API.Data.Mapping;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Reflection;

    public class ApplicationIdentityDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /**
            builder.Entity<User>(entity =>
            {
                entity.ToTable($"{nameof(User)}s");
                entity.Property(x => x.Email).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(100);
            });
            builder.Entity<Role>(entity =>
            {
                entity.ToTable($"{nameof(Role)}s");
            });
            builder.Entity<UserClaim>(entity =>
            {
                entity.ToTable($"{nameof(UserClaim)}s");
            });
            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable($"{nameof(UserRole)}s");
            });
            builder.Entity<UserLogin>(entity =>
            {
                entity.ToTable($"{nameof(UserLogin)}s");
            });
            builder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable($"{nameof(RoleClaim)}s");
            });
            builder.Entity<UserToken>(entity =>
            {
                entity.ToTable($"{nameof(UserToken)}s");
            });
            */

            //dynamically load all entity and query type configurations
            var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => (type.BaseType?.IsGenericType ?? false)
                && (type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)));

            foreach (var typeConfiguration in typeConfigurations)
            {
                var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                configuration.ApplyConfiguration(builder);
            } 
        }

    }
}
