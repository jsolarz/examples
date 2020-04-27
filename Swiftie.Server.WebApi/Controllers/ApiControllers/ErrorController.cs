namespace Swiftie.Server.WebApi.Controllers.ApiControllers
{
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// error controller
    /// </summary>
    public class ErrorController : ApiController
    {
        /// <summary>
        /// method to catch wrong paths
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions]
        public IHttpActionResult NotFound(string path)
        {
            // log error to ELMAH
            Elmah.ErrorSignal.FromCurrentContext().Raise(new HttpException(404, "404 Not Found: /" + path));

            // return 404
            return NotFound();
        }
    }
}
