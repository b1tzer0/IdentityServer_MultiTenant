using Duende.IdentityServer.EntityFramework.Options;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.DbContexts;

namespace idpMultiTenant1.Data
{
    public class MultiTenantConfigurationDbContextFactory : IDesignTimeDbContextFactory<MultiTenantConfigurationDbContext>
    {
        public MultiTenantConfigurationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MultiTenantConfigurationDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=IdPConfig;Trusted_Connection=True;MultipleActiveResultSets=true");
            
            var services = new ServiceCollection();
            services.AddIdentityServer()
                .AddConfigurationStore(
                    options =>
                    {
                        options.ConfigureDbContext = b => b.UseSqlServer(
                            "Server=(localdb)\\mssqllocaldb;Database=IdPConfig;Trusted_Connection=True;MultipleActiveResultSets=true",
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(
                                    typeof(MultiTenantConfigurationDbContext).Assembly.FullName);
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: 15,
                                    maxRetryDelay: TimeSpan.FromSeconds(30),
                                    errorNumbersToAdd: null);
                            });
                    });

            optionsBuilder.UseApplicationServiceProvider(services.BuildServiceProvider());
            
            var dummyTenant = new TenantInfo();
            return new MultiTenantConfigurationDbContext(dummyTenant, optionsBuilder.Options);
        }
    }
}
