using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace idpMultiTenant1.Pages.Admin.ApiScopes
{
    public class ApiScopeSummaryModel
    {
        public string DisplayName { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class ApiScopeModel : ApiScopeSummaryModel
    {
        public string UserClaims { get; set; }
    }


    public class ApiScopeRepository
    {
        readonly ConfigurationDbContext _context;

        public ApiScopeRepository(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(ApiScopeModel model)
        {
            var scope = new Duende.IdentityServer.Models.ApiScope()
            {
                Name = model.Name,
                DisplayName = model.DisplayName?.Trim()
            };

            var claims = model.UserClaims?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray() ?? Enumerable.Empty<string>();
            if (claims.Any())
            {
                scope.UserClaims = claims.ToList();
            }

            _context.ApiScopes.Add(scope.ToEntity());
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var scope = await _context.ApiScopes.SingleOrDefaultAsync(x => x.Name == id);

            if (scope == null)
            {
                throw new Exception("Invalid Api Scope");
            }

            _context.ApiScopes.Remove(scope);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApiScopeSummaryModel>> GetAllAsync(string filter = null)
        {
            var query = _context.ApiScopes
                .Include(x => x.UserClaims)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(x => x.Name.Contains(filter) || x.DisplayName.Contains(filter));
            }

            var result = query.Select(x => new ApiScopeSummaryModel
            {
                Name = x.Name,
                DisplayName = x.DisplayName
            });

            return await result.ToArrayAsync();
        }

        public async Task<ApiScopeModel> GetByIdAsync(string id)
        {
            var scope = await _context.ApiScopes
                .Include(x => x.UserClaims)
                .SingleOrDefaultAsync(x => x.Name == id);

            if (scope == null)
            {
                return null;
            }

            return new ApiScopeModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                UserClaims = scope.UserClaims.Any() ? scope.UserClaims.Select(x => x.Type).Aggregate((a, b) => $"{a} {b}") : null,
            };
        }

        public async Task UpdateAsync(ApiScopeModel model)
        {
            var scope = await _context.ApiScopes
                .Include(x => x.UserClaims)
                .SingleOrDefaultAsync(x => x.Name == model.Name);

            if (scope == null)
            {
                throw new Exception("Invalid Api Scope");
            }

            if (scope.DisplayName != model.DisplayName)
            {
                scope.DisplayName = model.DisplayName?.Trim();
            }

            var claims = model.UserClaims?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray() ?? Enumerable.Empty<string>();
            var currentClaims = (scope.UserClaims.Select(x => x.Type) ?? Enumerable.Empty<string>()).ToArray();

            var claimsToAdd = claims.Except(currentClaims).ToArray();
            var claimsToRemove = currentClaims.Except(claims).ToArray();

            if (claimsToRemove.Any())
            {
                scope.UserClaims.RemoveAll(x => claimsToRemove.Contains(x.Type));
            }
            if (claimsToAdd.Any())
            {
                scope.UserClaims.AddRange(claimsToAdd.Select(x => new ApiScopeClaim
                {
                    Type = x,
                }));
            }

            await _context.SaveChangesAsync();
        }


    }
}