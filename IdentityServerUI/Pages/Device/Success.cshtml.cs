using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerUI.Pages.Device
{
    [SecurityHeaders]
    [Authorize]
    public class SuccessModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}