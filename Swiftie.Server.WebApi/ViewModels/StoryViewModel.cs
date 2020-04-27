namespace Swiftie.Server.WebApi.ViewModels
{
    using Elmah.ContentSyndication;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Swiftie.Server.WebApi.Models;
    using System.Collections.Generic;
    using System;

    public class StoryViewModel
    {
        /// <summary>
        /// User
        /// </summary>
        public SimpleUserViewModel User { get; set; }
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
        public Models.Image Image { get; set; }
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
        /// <summary>
        /// story id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// story creation date
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Maps a story to a story view model with user profile
        /// </summary>
        /// <param name="story"></param>
        /// <returns></returns>
        public static object MapToViewModel(Story story)
        {
            return new StoryViewModel
            {
                Id = story.Id,
                User = SimpleUserViewModel.MapFromUser(story.User.Id),
                Title = story.Title,
                Description = story.Description,
                Image = story.Image,
                Audio = story.Audio,
                Video = story.Video,
                IsFeatured = story.IsFeatured,
                FeatureLink = story.FeatureLink,
                Comments = story.Comments,
                DateCreated = story.DateCreated
            };
        }

        /// <summary>
        /// Maps a comment to a story view model
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static object MapToViewModel(Comment comment)
        {
            return new StoryViewModel
            {
                Id = comment.Id,
                User = SimpleUserViewModel.MapFromUser(comment.User.Id),
                Title = comment.Title,
                Description = comment.Description,
                Image = comment.Image,
                Audio = comment.Audio,
                Video = comment.Video,
                DateCreated = comment.DateCreated
            };
        }
    }
}