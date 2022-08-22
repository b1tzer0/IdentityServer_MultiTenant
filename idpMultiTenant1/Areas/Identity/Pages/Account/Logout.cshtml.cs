using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityModel;
using idpMultiTenant1.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace idpMultiTenant1.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        [BindProperty]
        public string LogoutId { get; set; }

        public LogoutModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LogoutModel> logger,
            IIdentityServerInteractionService interaction, 
            IEventService events)
        {
            _signInManager = signInManager;
            _logger = logger;
            _interaction = interaction;
            _events = events;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            //await _signInManager.SignOutAsync();
            //_logger.LogInformation("User logged out.");
            //if (returnUrl != null)
            //{
            //    return LocalRedirect(returnUrl);
            //}
            //else
            //{
            //    return RedirectToPage();
            //}

            if (User?.Identity.IsAuthenticated == true)
            {
                // if there's no current logout context, we need to create one
                // this captures necessary info from the current logged in user
                // this can still return null if there is no context needed
                LogoutId ??= await _interaction.CreateLogoutContextAsync();

                // delete local authentication cookie
                await HttpContext.SignOutAsync();
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

                // see if we need to trigger federated logout
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                // if it's a local login we can ignore this workflow
                if (idp != null && idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
                {
                    // we need to see if the provider supports external logout
                    if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                    {
                        // build a return URL so the upstream provider will redirect back
                        // to us after the user has logged out. this allows us to then
                        // complete our single sign-out processing.
                        string url = Url.Page("/Account/Logout/Loggedout", new { logoutId = LogoutId });

                        // this triggers a redirect to the external provider for sign-out
                        return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                    }
                }
            }

            return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = LogoutId });
        }
    }
}
