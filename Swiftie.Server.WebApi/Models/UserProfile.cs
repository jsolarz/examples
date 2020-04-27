namespace Swiftie.Server.WebApi.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// User profile
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// User Id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Identity User
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// user swifties
        /// </summary>
        public List<Story> Stories { get; set; }
        /// <summary>
        /// user followers
        /// </summary>
        public List<UserProfile> Followers { get; set; }
        /// <summary>
        /// user followers
        /// </summary>
        public List<UserProfile> Following { get; set; }
        /// <summary>
        /// Remaining Feature Stars
        /// </summary>
        public int RemainingFeatureStars { get; set; }
        /// <summary>
        /// user avatar url
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// Display name for the user instead of email
        /// </summary>
        public string DisplayName { get; set; }
    }
}