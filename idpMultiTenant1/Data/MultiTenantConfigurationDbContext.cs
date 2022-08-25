using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Duende.IdentityServer.EntityFramework.Options;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;

namespace idpMultiTenant1.Data
{
    public class MultiTenantConfigurationDbContext : ConfigurationDbContext<MultiTenantConfigurationDbContext>, IMultiTenantDbContext
    {
        public MultiTenantConfigurationDbContext(ITenantInfo tenantInfo, DbContextOptions<MultiTenantConfigurationDbContext> options) : base(options)
        {
            TenantInfo = tenantInfo;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>().IsMultiTenant();
            //builder.Entity<ClientClaim>().IsMultiTenant();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.EnforceMultiTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.EnforceMultiTenant();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public ITenantInfo TenantInfo { get; }
        public TenantMismatchMode TenantMismatchMode { get; } = TenantMismatchMode.Throw;
        public TenantNotSetMode TenantNotSetMode { get; } = TenantNotSetMode.Throw;
    }
}
