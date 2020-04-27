namespace Swiftie.Server.WebApi.Infrastructure
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Swiftie.Server.WebApi.Models;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Auth repository
    /// </summary>
    public class AuthRepository : IDisposable
    {
        private StoryDbContext _ctx;

        private UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthRepository()
        {
            _ctx = new StoryDbContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RegisterUser(User userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        /// <summary>
        /// Find a user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}