namespace Swiftie.Server.WebApi.ViewModels
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Swiftie.Server.WebApi.Models;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System;

    /// <summary>
    /// User viewModel
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// user swifties
        /// </summary>
        public List<Story> Stories { get; set; }
        /// <summary>
        /// user followers
        /// </summary>
        public List<UserViewModel> Followers { get; set; }
        /// <summary>
        ///  user display name
        /// </summary>
        public string DisplayName { get; internal set; }
        /// <summary>
        /// user avatar url
        /// </summary>
        public string Avatar { get; internal set; }
        /// <summary>
        /// user following list
        /// </summary>
        public List<UserViewModel> Following { get; set; }

        /// <summary>
        /// Gets a user view model
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserViewModel MapFromUser(IdentityUser user)
        {
            StoryDbContext db = new StoryDbContext();
            var profile = db.UsersProfiles
                .Include(c => c.Followers)
                .Include(c => c.Following)
                .Where(c => c.UserId == user.Id).SingleOrDefault();

            if (profile == null)
            {
                return new UserViewModel
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email ?? string.Empty
                };
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email ?? string.Empty,
                DisplayName = profile.DisplayName ?? string.Empty,
                Avatar = profile.Avatar,
                Followers = new List<UserViewModel>(),
                Following = new List<UserViewModel>()
            };

            if (profile.Followers == null)
            {

                foreach (var item in profile.Followers)
                {
                    var followerUser = db.Users.Find(item.UserId);
                    var followerProfile = db.UsersProfiles.Where(c => c.UserId == item.UserId).SingleOrDefault();

                    model.Followers.Add(new UserViewModel
                    {
                        Id = followerUser.Id,
                        Name = followerUser.UserName,
                        Email = followerUser.Email ?? string.Empty,
                        DisplayName = followerProfile.DisplayName ?? string.Empty,
                        Avatar = followerProfile.Avatar ?? string.Empty
                    });
                }
            }

            if (profile.Following != null)
            {
                foreach (var item in profile.Following)
                {
                    var followingUser = db.Users.Find(item.UserId);
                    var followingProfile = db.UsersProfiles.Where(c => c.UserId == item.UserId).SingleOrDefault();

                    model.Following.Add(new UserViewModel
                    {
                        Id = followingUser.Id,
                        Name = followingUser.UserName,
                        Email = followingUser.Email ?? string.Empty,
                        DisplayName = followingProfile.DisplayName ?? string.Empty,
                        Avatar = followingProfile.Avatar ?? string.Empty
                    });
                }
            }

            return model;
        }

    }
}