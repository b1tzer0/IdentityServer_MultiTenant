using Finbuckle.MultiTenant;
using idpMultiTenant1.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace idpMultiTenant1.Areas.Identity.Pages.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LoginFido2MfaModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool RememberMe { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string TenantId
        {
            get => HttpContext.GetMultiTenantContext<TenantInfo>()?.TenantInfo?.Identifier;
        }
        public void OnGet()
        {
        }
    }
}
