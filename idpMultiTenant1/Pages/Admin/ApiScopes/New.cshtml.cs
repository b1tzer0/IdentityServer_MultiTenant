using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace idpMultiTenant1.Pages.Admin.ApiScopes
{
    [SecurityHeaders]
    [Authorize]
    public class NewModel : PageModel
    {
        readonly ApiScopeRepository _repository;

        public NewModel(ApiScopeRepository repository)
        {
            _repository = repository;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _repository.CreateAsync(InputModel);
                return RedirectToPage("/Admin/ApiScopes/Edit", new { id = InputModel.Name });
            }

            return Page();
        }

        [BindProperty]
        public ApiScopeModel InputModel { get; set; }
    }
}