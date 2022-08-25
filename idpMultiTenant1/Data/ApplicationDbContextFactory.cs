using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Diagnostics;

namespace idpMultiTenant1.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=IdPUserStore;Trusted_Connection=True;MultipleActiveResultSets=true";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, o=>
                {
                    o.MigrationsAssembly(typeof(Program).Assembly.FullName);
                });
            });

            optionsBuilder.UseApplicationServiceProvider(services.BuildServiceProvider());

            var dummyTenant = new TenantInfo()
            {
                ConnectionString = connectionString
            };
            
            return new ApplicationDbContext(dummyTenant, optionsBuilder.Options);
        }
    }
}
