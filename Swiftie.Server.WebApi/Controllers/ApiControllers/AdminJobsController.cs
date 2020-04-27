using Swiftie.Server.WebApi.Infrastructure;
using Swiftie.Server.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Swiftie.Server.WebApi.Controllers.ApiControllers
{
    /// <summary>
    /// manage jobs to perform various operations on sys objects
    /// </summary>
    [RoutePrefix("api/admin/jobs")]
    public class AdminJobsController : ApiController
    {
        private StoryDbContext db = new StoryDbContext();

        // GET: api/Stories
        // GET: api/Stories/pageSize/pageNumber/orderBy(optional) 
        /// <summary>
        /// Returns a list of stories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("RemoveFeatured")]
        public IHttpActionResult GetFeatured()
        {
            var featured = db.Stories
                .Where(x => x.IsFeatured && (int)(DateTime.Now - x.DateCreated).TotalDays > 20)
                .ToList();

            foreach (var item in featured)
            {
                item.IsFeatured = false;
            }

            db.SaveChanges();

            return Ok(featured);
        }

        // DELETE: api/admin/jobs/removestories
        /// <summary>
        /// Returns a list of stories
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("RemoveStories")]
        public IHttpActionResult DeleteStories()
        {
            db.Stories.Clear();
            db.SaveChanges();

            return Ok("Stories removed");
        }
    }
}
