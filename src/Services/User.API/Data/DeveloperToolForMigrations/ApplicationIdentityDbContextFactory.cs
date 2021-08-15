namespace User.API.Data.DeveloperToolForMigrations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using System.Reflection;

    /// <summary>
    /// 1. add _> Microsoft.EntityFrameworkCore.Tools
    /// 2. Set as Startup Project
    /// 3. Open Package Manager Console
    /// 4. Add-Migration.....
    /// 
    /// for migrations scripts:
    ///_> dotnet ef migrations add Initial
    ///_> dotnet ef database update
    ///_>#dotnet ef database update Initial
    ///
    /// Add-Migration Initial -OutputDir "Data/Migrations"
    /// dotnet ef migrations add Initial -o "Data/Migrations"
    /// 
    /// dotnet ef migrations add Initial --context ApplicationIdentityDbContext -o "Data/Migrations/Identity"
    /// Add-Migration Initial -Context ApplicationIdentityDbContext -OutputDir "Data/Migrations/Identity"
    /// </summary>
    public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            //Database connection string
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=user_api_db;Trusted_Connection=True;MultipleActiveResultSets=true";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name; //"User.API"

            var builder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly));

            return new ApplicationIdentityDbContext(builder.Options);
        }
    }
}
