namespace Swiftie.Server.WebApi.Models
{
    /// <summary>
    /// Base Media object
    /// </summary>
    public abstract class Media : EntityBase
    {
        /// <summary>
        /// Media url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Media type
        /// </summary>
        public MediaType Type { get; set; }
        /// <summary>
        /// media title
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Image media class
    /// </summary>
    public class Image : Media
    {
        /// <summary>
        /// image Width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// image height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// image aspect
        /// </summary>
        public double Aspect { get; set; }
        /// <summary>
        /// image tye
        /// </summary>
        public string ImageType { get; set; }
    }

    /// <summary>
    /// Audio media class
    /// </summary>
    public class Audio : Media
    {
        /// <summary>
        /// audio duration
        /// </summary>
        public double Duration { get; set; }
    }

    /// <summary>
    /// Video media class
    /// </summary>
    public class Video : Media
    {
        /// <summary>
        /// Video duration
        /// </summary>
        public double Duration { get; set; }
        /// <summary>
        /// video width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// video height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// video aspect
        /// </summary>
        public double Aspect { get; set; }
        /// <summary>
        /// video type
        /// </summary>
        public string VideoType { get; set; }
    }
}