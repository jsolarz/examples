namespace Swiftie.Server.WebApi.ViewModels
{
    using Swiftie.Server.WebApi.Models;
    using System;

    /// <summary>
    /// Comment view model
    /// </summary>
    public class CommentViewModel
    {
        /// <summary>
        /// User
        /// </summary>
        public SimpleUserViewModel User { get; set; }
        /// <summary>
        /// story id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// story creation date
        /// </summary>
        public DateTime DateCreated { get; set; }
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