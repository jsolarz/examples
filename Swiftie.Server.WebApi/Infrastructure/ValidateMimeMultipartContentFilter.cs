using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Swiftie.Server.WebApi.Infrastructure
{
    /// <summary>
    /// Validate Mime Multipart ContentFilter
    /// </summary>
    public class ValidateMimeMultipartContentFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Action Executing filter
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
        }

        /// <summary>
        /// Action Executed Filter
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

        }

    }
}