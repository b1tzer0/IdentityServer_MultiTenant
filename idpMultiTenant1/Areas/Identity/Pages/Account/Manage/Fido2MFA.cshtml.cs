using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace idpMultiTenant1.Areas.Identity.Pages.Account.Manage
{
    public class Fido2MFAModel : PageModel
    {
        public string TenantId
        {
            get => HttpContext.GetMultiTenantContext<TenantInfo>()?.TenantInfo?.Identifier;
        }

        public void OnGet()
        {
        }
    }
}
