namespace Swiftie.Server.WebApi.Models
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A user story
    /// </summary>
    public class Story : EntityBase
    {
        /// <summary>
        /// User
        /// </summary>
        public IdentityUser User { get; set; }
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
        /// <summary>
        /// Specify if a story is featured
        /// </summary>
        public bool IsFeatured { get; set; }
        /// <summary>
        /// Featured link
        /// </summary>
        public string FeatureLink { get; set; }
        /// <summary>
        /// Story Comments
        /// </summary>
        public List<Comment> Comments { get; set; }        
    }
}
