using Duende.IdentityServer.Extensions;
using idpMultiTenant1.Data;
using idpMultiTenant1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Encodings.Web;

namespace idpMultiTenant1.Management
{
    [Route("[controller]")]
    public class ManagementController : Controller
    {
        readonly UserManager<ApplicationUser> userManager;
        
        readonly IEmailSender emailSender;

        public ManagementController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        [HttpGet("Register")]
        public async Task<IActionResult> RegisterUser(string email, string password)
        {
            try
            {
                var user = new ApplicationUser { UserName = email, Email = email };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return Ok(user.Id);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                // If we got this far, something failed
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.HResult.ToString(), ex.Message);
                return BadRequest(ModelState);
            }            
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user.Id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.HResult.ToString(), ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
