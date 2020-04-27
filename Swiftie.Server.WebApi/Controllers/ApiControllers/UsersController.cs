namespace Swiftie.Server.WebApi.Controllers
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Swiftie.Server.WebApi.Models;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;
    using ViewModels;
    using System.Data.Entity;

    /// <summary>
    /// Users Controller
    /// </summary>
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private StoryDbContext db = new StoryDbContext();

        // GET: api/Users
        /// <summary>
        /// Get a List of users
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(List<UserViewModel>))]
        public IHttpActionResult GetApplicationUsers()
        {
            List<UserViewModel> list = new List<UserViewModel>();
            var users = db.Users.ToList();
            foreach (var item in users)
            {
                var model = UserViewModel.MapFromUser(item);
                list.Add(model);
            }

            return Ok(list);
        }

        // GET: api/Users/5
        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(UserViewModel))]
        public IHttpActionResult GetUser(string id)
        {
            StoryDbContext db = new StoryDbContext();
            var user = db.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = UserViewModel.MapFromUser(user);
            return Ok(model);
        }

        // GET: api/Users/5
        /// <summary>
        /// Updates the user info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Update(string id, UserViewModel model)
        {
            var profile = db.UsersProfiles.Where(x => x.UserId == id).SingleOrDefault();
            if (profile == null)
            {
                return NotFound();
            };

            profile.DisplayName = model.DisplayName;
            profile.Avatar = model.Avatar;

            db.SaveChanges();

            return Ok("User info updated");
        }

        // Get: api/Users/Stories/5
        /// <summary>
        /// Get a user swifties
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<Story>))]
        [Route("Stories/{id}")]
        public IHttpActionResult GetStories(string id)
        {
            var stories = db.Stories.Where(x => x.User.Id == id).ToList();

            if (stories == null)
            {
                return NotFound();
            }

            return Ok(stories);
        }

        // Get: api/Users/Followers/5
        /// <summary>
        /// Get a user followers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<User>))]
        [Route("Followers/{id}")]
        public IHttpActionResult GetFollowers(string id)
        {
            var userProfile = db.UsersProfiles
                .Include(c => c.Followers)
                .Where(c => c.UserId == id)
                .SingleOrDefault();

            if (userProfile == null)
            {
                return NotFound();
            }

            List<UserViewModel> followers = new List<UserViewModel>();

            foreach (var item in userProfile.Followers)
            {
                var user = db.Users.Find(item.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                var model = UserViewModel.MapFromUser(user);
                followers.Add(model);
            }

            return Ok(followers);
        }

        /// <summary>
        /// Follow a user
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followId"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(void))]
        [Route("Follow")]
        public IHttpActionResult PostFollow(string followerId, string followId)
        {
            var followeeUser = db.Users.Find(followId);
            if (followeeUser == null)
            {
                return NotFound();
            }

            var followee = db.UsersProfiles
                .Include(c => c.Followers)
                .Where(c => c.UserId == followeeUser.Id).SingleOrDefault();
            if (followee == null)
            {
                return NotFound();
            }

            if (followee.Followers == null)
            {
                followee.Followers = new List<UserProfile>();
                return Ok();
            }

            if (followee.Followers.Any(c => c.UserId == followerId))
            {
                return Ok();
            }

            var followerUser = db.Users.Find(followerId);
            if (followerUser == null)
            {
                return NotFound();
            }

            var follower = db.UsersProfiles.Where(c => c.UserId == followerId).SingleOrDefault();
            if (follower == null)
            {
                return NotFound();
            }

            followee.Followers.Add(follower);

            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Follow a user
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="userToFollow"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("FollowUser")]
        public IHttpActionResult PostFollowUser(string followerId, string userToFollow)
        {
            var followeeUser = db.Users.Where(c => c.UserName == userToFollow).SingleOrDefault();
            if (followeeUser == null)
            {
                return NotFound();
            }

            var followee = db.UsersProfiles
                .Include(c => c.Followers)
                .Where(c => c.UserId == followeeUser.Id).SingleOrDefault();
            if (followee == null)
            {
                return NotFound();
            }

            if (followee.Followers == null)
            {
                followee.Followers = new List<UserProfile>();
                return Ok();
            }

            if (followee.Followers.Any(c => c.UserId == followerId))
            {
                return Ok();
            }

            var followerUser = db.Users.Find(followerId);
            if (followerUser == null)
            {
                return NotFound();
            }

            var follower = db.UsersProfiles.Where(c => c.UserId == followerId).SingleOrDefault();
            if (follower == null)
            {
                return NotFound();
            }

            followee.Followers.Add(follower);

            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Unfollow a user
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followId"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("Unfollow")]
        public IHttpActionResult PostUnfollow(string followerId, string followId)
        {
            var followerUser = db.Users.Find(followerId);
            if (followerUser == null)
            {
                return NotFound();
            }

            var follower = db.UsersProfiles.Where(c => c.UserId == followerId).SingleOrDefault();
            if (follower == null)
            {
                return NotFound();
            }

            var user = db.Users.Where(c => c.Id == followId).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            var followee = db.UsersProfiles
                .Include(c => c.Followers)
                .Where(c => c.UserId == user.Id).SingleOrDefault();

            if (followee == null)
            {
                return NotFound();
            }

            if (followee.Followers != null)
            {
                followee.Followers.Remove(follower);
                db.SaveChanges();
            }

            return Ok();
        }

        /// <summary>
        /// Dispose Method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(string id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}