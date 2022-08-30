using idpMultiTenant1.FIDO2;
using Microsoft.AspNetCore.Identity;

namespace idpMultiTenant1.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    /// <summary>
    /// Class ApplicationUser.
    /// Implements the <see cref="IdentityUser" />
    /// </summary>
    /// <seealso cref="IdentityUser" />
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUser"/> class.
        /// </summary>
        /// <remarks>The Id property is initialized to form a new GUID string value.</remarks>
        public ApplicationUser()
        {
        }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the last login date.
        /// </summary>
        /// <value>The last login date.</value>
        public DateTimeOffset LastLoginDate { get; set; }
    }
}