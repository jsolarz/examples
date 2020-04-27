namespace Swiftie.Server.WebApi.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;

    /// <summary>
    /// A user story
    /// </summary>
    public class Comment : EntityBase
    {
        /// <summary>
        /// User
        /// </summary>
        public IdentityUser User { get; set; }
        /// <summary>
        /// Story Id
        /// </summary>
        public int StoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is story deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Image
        /// </summary>
        public Image Image { get; set; }
        /// <summary>
        /// Audio
        /// </summary>
        public Audio Audio { get; set; }
        /// <summary>
        /// Video
        /// </summary>
        public Video Video { get; set; }
    }
}
