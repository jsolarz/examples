namespace Swiftie.Server.WebApi.Controllers.ApiControllers
{
    using Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    /// <summary>
    /// Media controller
    /// </summary>
    [RoutePrefix("api/Media")]
    public class MediaController : ApiController
    {
        private static readonly string ServerUploadFolder = ConfigurationManager.AppSettings["FileProvider.FilesLocation"];
        private static readonly string mediaServerUrl = ConfigurationManager.AppSettings["mediaServerUrl"];

        private static readonly FileProvider _fileProvider = new FileProvider();

        // GET: api/Media/upload/userid/extension
        /// <summary>
        /// Upload a file
        /// </summary>
        /// <returns></returns>
        [Route("upload/{userId}/{extension}")]
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task<FileResult> PostUploadFile(string userId, string extension)
        {
            var streamProvider = new StoryDataStreamProvider(ServerUploadFolder);
            streamProvider.Extension = extension;
            await Request.Content.ReadAsMultipartAsync(streamProvider);

            var filename = streamProvider.FileData.Select(entry => entry.LocalFileName).First();
            var file = Path.GetFileNameWithoutExtension(filename);
            var downloadLink = string.Format("{0}/show/{1}/{2}", mediaServerUrl, file, extension);

            return new FileResult
            {
                FileName = filename,
                ContentTypes = streamProvider.FileData.Select(entry => entry.Headers.ContentType.MediaType),
                CreatedTimestamp = DateTime.UtcNow,
                DownloadLink = downloadLink
            };
        }

        /// <summary>
        /// Show a file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("show/{id}/{extension}")]
        public HttpResponseMessage Show(string id, string extension)
        {
            string fileName = string.Format("{0}.{1}", id, extension);
            if (!_fileProvider.Exists(fileName))
                throw new HttpResponseException(HttpStatusCode.NotFound);

            string mimeType = MimeMapping.GetMimeMapping(fileName);

            if (mimeType.Equals("audio/wav"))
            {
                mimeType = "audio/x-wav";
            }

            FileStream fileStream = _fileProvider.Open(fileName);
            HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(fileStream) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            response.Content.Headers.ContentLength = _fileProvider.GetLength(fileName);
            return response;

        }

        private StreamContent StreamConversion()
        {
            Stream reqStream = Request.Content.ReadAsStreamAsync().Result;
            var tempStream = new MemoryStream();
            reqStream.CopyTo(tempStream);

            tempStream.Seek(0, SeekOrigin.End);
            var writer = new StreamWriter(tempStream);
            writer.WriteLine();
            writer.Flush();
            tempStream.Position = 0;

            var streamContent = new StreamContent(tempStream);
            foreach (var header in Request.Content.Headers)
            {
                streamContent.Headers.Add(header.Key, header.Value);
            }
            return streamContent;
        }
    }

    /// <summary>
    /// File result
    /// </summary>
    public class FileResult
    {
        /// <summary>
        /// Files list
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Created timestmap
        /// </summary>
        public DateTime CreatedTimestamp { get; set; }
        /// <summary>
        /// Download link
        /// </summary>
        public string DownloadLink { get; set; }
        /// <summary>
        /// Content types
        /// </summary>
        public IEnumerable<string> ContentTypes { get; set; }
    }
}
