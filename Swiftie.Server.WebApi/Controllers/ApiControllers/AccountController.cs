namespace Swiftie.Server.WebApi.Controllers.ApiControllers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Swiftie.Server.WebApi.Infrastructure;
    using Swiftie.Server.WebApi.Models;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Account controller
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;
        private StoryDbContext db = new StoryDbContext();

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountController()
        {
            _repo = new AuthRepository();
        }

        // POST api/Account/Register
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(User userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            IdentityUser user = await _repo.FindUser(userModel.UserName, userModel.Password);

            db.SaveChanges();

            UserProfile profile = new UserProfile
            {
                UserId = user.Id,
                RemainingFeatureStars = 5
            };

            db.UsersProfiles.Add(profile);
            db.SaveChanges();

            return Ok(user.Id);
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
