// ***********************************************************************
// Assembly         : idpMultiTenant1
// Author           : Rick Mathers
// Created          : 08-17-2022
//
// Last Modified By : Rick Mathers
// Last Modified On : 08-17-2022
// ***********************************************************************
// <copyright file="NovacProfileService.cs" company="Megasys Hospitality Solutions">
//     Copyright (c) Megasys Hospitality Solutions. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using idpMultiTenant1.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace idpMultiTenant1.Services
{
    /// <summary>
    /// Class NovacProfileService.
    /// Implements the <see cref="Duende.IdentityServer.AspNetIdentity.ProfileService{idpMultiTenant1.Models.ApplicationUser}" />
    /// </summary>
    /// <seealso cref="Duende.IdentityServer.AspNetIdentity.ProfileService{idpMultiTenant1.Models.ApplicationUser}" />
    public class NovacProfileService : ProfileService<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NovacProfileService"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        public NovacProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : base(userManager, claimsFactory)
        {
        }

        /// <summary>
        /// Get profile data as an asynchronous operation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="user">The user.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
        {
            var principal = await GetUserClaimsAsync(user);
            var id = (ClaimsIdentity)principal.Identity;
            if (!string.IsNullOrEmpty(user.TenantId))
            {
                id.AddClaim(new Claim("tenant", user.TenantId));
            }

            context.AddRequestedClaims(principal.Claims);
        }
    }
}
