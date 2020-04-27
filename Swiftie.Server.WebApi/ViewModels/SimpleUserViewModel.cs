namespace Swiftie.Server.WebApi.ViewModels
{
    using System;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Swiftie.Server.WebApi.Models;
    using System.Linq;
    /// <summary>
    /// User view model with minimal properties
    /// </summary>
    public class SimpleUserViewModel
    {
        /// <summary>
        /// user id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// user display name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// user avatar
        /// </summary>
        public string Avatar { get; set; }
        
        internal static SimpleUserViewModel MapFromUser(string id)
        {
            StoryDbContext db = new StoryDbContext();
            var _profile = db.UsersProfiles.Where(c => c.UserId == id).SingleOrDefault();

            if(_profile == null)
            {
                return new SimpleUserViewModel();
            }

            return new SimpleUserViewModel
            {
                Id = _profile.UserId,
                Name = _profile.DisplayName,
                Avatar = _profile.Avatar
            };
        }
    }
}