namespace Swiftie.Server.WebApi.Models
{
    using System.Data.Entity;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    /// <summary>
    /// User profile data
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    ///// <summary>
    ///// Application database context
    ///// </summary>
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    /// <summary>
    //    /// Creates a new database context
    //    /// </summary>
    //    /// <returns></returns>
    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }


    //    /// <summary>
    //    /// List of application users
    //    /// </summary>
    //    public System.Data.Entity.DbSet<Swiftie.Server.WebApi.Models.User> ApplicationUsers { get; set; }
    //}
}