using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace idpMultiTenant1.Pages.Admin.ApiScopes
{
    [SecurityHeaders]
    [Authorize]
    public class IndexModel : PageModel
    {
        readonly ApiScopeRepository _repository;

        public IndexModel(ApiScopeRepository repository)
        {
            _repository = repository;
        }

        public async Task OnGetAsync(string filter)
        {
            Filter = filter;
            Scopes = await _repository.GetAllAsync(filter);
        }

        public string Filter { get; set; }

        public IEnumerable<ApiScopeSummaryModel> Scopes { get; private set; }
    }
}