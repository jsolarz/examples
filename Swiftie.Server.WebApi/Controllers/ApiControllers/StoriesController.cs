namespace Swiftie.Server.WebApi.Controllers
{
    using Infrastructure;
    using Swiftie.Server.WebApi.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using ViewModels;
    /// <summary>
    /// Stories controller
    /// </summary>
    [RoutePrefix("api/stories")]
    public class StoriesController : ApiController
    {
        private StoryDbContext db = new StoryDbContext();

        // GET: api/Stories
        // GET: api/Stories/pageSize/pageNumber/orderBy(optional) 
        /// <summary>
        /// Returns a list of stories
        /// </summary>
        /// <returns></returns>
        [Route("{pageSize:int}/{pageNumber:int}/{orderBy:alpha?}")]
        public IHttpActionResult GetStories(int pageSize, int pageNumber, string orderBy = "")
        {
            var totalCount = db.Stories.Count();
            var totalPages = Math.Ceiling((double)totalCount / pageSize);

            var clubQuery = db.Stories
                .Include(b => b.Audio)
                .Include(v => v.Video)
                .Include(i => i.Image)
                .Include(c => c.User)
                .AsQueryable();

            if (QueryHelper.PropertyExists<Story>(orderBy))
            {
                var orderByExpression = QueryHelper.GetPropertyExpression<Story>(orderBy);
                clubQuery = clubQuery.OrderBy(orderByExpression);
            }
            else
            {
                clubQuery = clubQuery
                                .OrderByDescending(c => c.IsFeatured)
                                .ThenByDescending(c => c.DateCreated);
            }

            var stories = clubQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new
            {
                TotalCount = totalCount,
                totalPages = totalPages,
                Stories = stories.Select(c => StoryViewModel.MapToViewModel(c)).ToList()
            };

            return Ok(result);
        }

        // GET: api/Stories/5
        /// <summary>
        /// Finds a story by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Story))]
        public IHttpActionResult GetStory(int id)
        {
            Story story = db.Stories.Find(id);
            if (story == null)
            {
                return NotFound();
            }

            return Ok(story);
        }

        // PUT: api/Stories/5
        /// <summary>
        /// Updates a story
        /// </summary>
        /// <param name="id"></param>
        /// <param name="story"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStory(int id, StoryViewModel story)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != story.Id)
            {
                return BadRequest();
            }

            db.Entry(story).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stories
        /// <summary>
        /// Creates a new story
        /// </summary>
        /// <param name="story"></param>
        /// <returns></returns>
        [ResponseType(typeof(StoryViewModel))]
        public IHttpActionResult PostStory(StoryViewModel story)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isFeatured = false;
            var user = db.Users.Find(story.User.Id);
            if (user == null)
            {
                return NotFound();
            }

            var profile = db.UsersProfiles.Where(c => c.UserId == story.User.Id).SingleOrDefault();

            if (profile.RemainingFeatureStars > 0 && story.IsFeatured)
            {
                profile.RemainingFeatureStars = profile.RemainingFeatureStars - 1;
                isFeatured = true;
            }
            else
            {
                isFeatured = false;
            }

            Story model = new Story
            {
                User = user,
                Title = story.Title,
                Description = story.Description,
                IsDeleted = false,
                Image = story.Image,
                Audio = story.Audio,
                Video = story.Video,
                IsFeatured = isFeatured,
                FeatureLink = story.FeatureLink,
                Comments = new List<Comment>()
            };

            db.Stories.Add(model);
            db.SaveChanges();

            story.Id = model.Id;
            story.User.Avatar = profile.Avatar;
            story.User.Name = profile.DisplayName;

            return CreatedAtRoute("DefaultApi", new
            {
                id = story.Id
            }, story);
        }

        // DELETE: api/Stories/5
        /// <summary>
        /// Deletes a story
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Story))]
        public IHttpActionResult DeleteStory(int id)
        {
            Story story = db.Stories.Find(id);
            if (story == null)
            {
                return NotFound();
            }

            db.Stories.Remove(story);
            db.SaveChanges();

            return Ok(story);
        }

        /// <summary>
        /// Dispose the controller object
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

        private bool StoryExists(int id)
        {
            return db.Stories.Count(e => e.Id == id) > 0;
        }
    }
}