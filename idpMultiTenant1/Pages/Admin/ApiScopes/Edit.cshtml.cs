using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace idpMultiTenant1.Pages.Admin.ApiScopes
{
    [SecurityHeaders]
    [Authorize]
    public class EditModel : PageModel
    {
        readonly ApiScopeRepository _repository;

        public EditModel(ApiScopeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            InputModel = await _repository.GetByIdAsync(id);
            if (InputModel == null)
            {
                return RedirectToPage("/Admin/ApiScopes/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (Button == "delete")
            {
                await _repository.DeleteAsync(id);
                return RedirectToPage("/Admin/ApiScopes/Index");
            }

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(InputModel);
                return RedirectToPage("/Admin/ApiScopes/Edit", new { id });
            }

            return Page();
        }

        [BindProperty]
        public string Button { get; set; }

        [BindProperty]
        public ApiScopeModel InputModel { get; set; }
    }
}