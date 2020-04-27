namespace Swiftie.Server.WebApi.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity;

    /// <summary>
    /// Stories Db Context
    /// </summary>
    public class StoryDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StoryDbContext() : base("StoryDBContext")
        {

        }

        /// <summary>
        /// List of Stories
        /// </summary>
        public DbSet<Story> Stories { get; set; }
        /// <summary>
        /// List of users
        /// </summary>
        public DbSet<UserProfile> UsersProfiles { get; set; }
        /// <summary>
        /// List of Comments
        /// </summary>
        public DbSet<Comment> Comments { get; set; }
    }
}