using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web;

namespace Swiftie.Server.WebApi.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class StoryDataStreamProvider : MultipartFormDataStreamProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        public StoryDataStreamProvider(string rootPath) : base(rootPath)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            base64Guid = rgx.Replace(base64Guid, "");
            return string.Format("{0}.{1}", base64Guid, Extension);
        }

        internal string Extension { get; set; }
    }
}