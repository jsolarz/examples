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
    [RoutePrefix("api/stories/comments")]
    public class CommentsController : ApiController
    {
        private StoryDbContext db = new StoryDbContext();

        // GET : api/Stories/Comments/List/5
        /// <summary>
        /// Find comments of a story
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("List/{id}")]
        [ResponseType(typeof(StoryViewModel))]
        public IHttpActionResult GetComments(int id)
        {
            Story story = db.Stories
                .Include("Comments")
                .Include("Comments.Image")
                .Include("Comments.Audio")
                .Include("Comments.Video")
                .Include("Comments.User")
                .SingleOrDefault(c => c.Id == id);
            if (story == null)
            {
                return NotFound();
            }

            var comments = story.Comments.OrderByDescending(c => c.DateCreated).ToList();
            var model = comments.Select(c => StoryViewModel.MapToViewModel(c)).ToList();

            return Ok(model);
        }

        // POST: api/Comments
        /// <summary>
        /// Creates a new comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [ResponseType(typeof(CommentViewModel))]
        public IHttpActionResult PostComment(CommentViewModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = db.Users.Find(comment.User.Id);
            if (user == null)
            {
                return NotFound();
            }

            var profile = db.UsersProfiles.Where(c => c.UserId == comment.User.Id).SingleOrDefault();


            Story story = db.Stories.Find(comment.StoryId);
            if (story == null)
            {
                return NotFound();
            }


            if (story.Comments == null)
            {
                story.Comments = new List<Comment>();
            }

            Comment model = new Comment
            {
                User = user,
                StoryId = comment.StoryId,
                Title = comment.Title,
                Description = comment.Description,
                Image = comment.Image,
                Audio = comment.Audio,
                Video = comment.Video
            };

            story.Comments.Add(model);
            db.SaveChanges();

            comment.Id = model.Id;
            comment.User.Avatar = profile.Avatar;
            comment.User.Name = profile.DisplayName;

            return CreatedAtRoute("DefaultApi", new
            {
                id = comment.Id
            }, comment);
        }

        // DELETE: api/comment/5
        /// <summary>
        /// Deletes a comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Comment))]
        public IHttpActionResult DeleteComment(int id)
        {
            var comment = db.Comments.Find(id);
            if (comment == null)
            {
                return Ok();
            }

            db.Comments.Remove(comment);
            db.SaveChanges();

            return Ok();
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
    }
}