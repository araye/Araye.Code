using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Araye.Code.Mvc.Helpers
{
    public static class GravatarHtmlHelper
    {
        public static Gravatar Gravatar(this HtmlHelper htmlHelper, string email)
        {
            return new Gravatar { Email = email, Size = 50, MaxAllowedRating = Helpers.Gravatar.RatingType.X };
        }

        public static Gravatar Gravatar(this HtmlHelper htmlHelper, string email, bool outputSiteLink)
        {
            return new Gravatar { Email = email, Size = 50, MaxAllowedRating = Helpers.Gravatar.RatingType.X, OutputGravatarSiteLink = outputSiteLink };
        }

        public static Gravatar Gravatar(this HtmlHelper htmlHelper, string email, short size)
        {
            return new Gravatar { Email = email, Size = size, MaxAllowedRating = Helpers.Gravatar.RatingType.X };
        }

        public static Gravatar Gravatar(this HtmlHelper htmlHelper, string email, short size, bool outputSiteLink)
        {
            return new Gravatar { Email = email, Size = size, MaxAllowedRating = Helpers.Gravatar.RatingType.X, OutputGravatarSiteLink = outputSiteLink };
        }

    }

    public class Gravatar
    {
        public enum RatingType { G, PG, R, X }

        private string _email;

        // outut gravatar site link true by default:

        // customise the link title:

        public Gravatar()
        {
            OutputGravatarSiteLink = true;
            LinkTitle = "Get your avatar";
        }

        /// <summary>
        /// The Email for the user
        /// </summary>

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value.ToLower();
            }
        }

        /// <summary>
        /// Size of Gravatar image.  Must be between 1 and 512.
        /// </summary>
        public short Size { get; set; }

        /// <summary>
        /// An optional "rating" parameter may follow with a value of [ G | PG | R | X ] that determines the highest rating (inclusive) that will be returned.
        /// </summary>
        public RatingType MaxAllowedRating { get; set; }

        /// <summary>
        /// Determines whether the image is wrapped in an anchor tag linking to the Gravatar sit
        /// </summary>
        public bool OutputGravatarSiteLink { get; set; }

        /// <summary>
        /// Optional property for link title for gravatar website link
        /// </summary>
        public string LinkTitle { get; set; }

        /// <summary>
        /// An optional "default" parameter may follow that specifies the full, URL encoded URL, protocol included, of a GIF, JPEG, or PNG image that should be returned if either the requested email address has no associated gravatar, or that gravatar has a rating higher than is allowed by the "rating" parameter.
        /// </summary>
        public string DefaultImage { get; set; }

        public override string ToString()
        {

            // if the size property has been specified, ensure it is a short, and in the range 
            // 1..512:
            try
            {
                // if it's not in the allowed range, throw an exception:
                if (Size < 1 || Size > 512)
                    throw new ArgumentOutOfRangeException();
            }
            catch
            {
                Size = 80;
            }

            // default the image url:
            string imageUrl = "http://www.gravatar.com/avatar.php?";

            if (!string.IsNullOrEmpty(Email))
            {
                // build up image url, including MD5 hash for supplied email:
                

                var encoder = new UTF8Encoding();
                var md5Hasher = new MD5CryptoServiceProvider();

                byte[] hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(Email));

                var sb = new StringBuilder(hashedBytes.Length * 2);
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    sb.Append(i.ToString("X2"));
                }

                // output parameters:
                imageUrl += "gravatar_id=" + sb.ToString().ToLower();
                imageUrl += "&rating=" + MaxAllowedRating;
                imageUrl += "&size=" + Size;
            }

            // output default parameter if specified
            if (!string.IsNullOrEmpty(DefaultImage))
            {
                imageUrl += "&default=" + HttpUtility.UrlEncode(DefaultImage);
            }



            var linkBuilder = new TagBuilder("a");
            // if we need to output the site link:
            if (OutputGravatarSiteLink)
            {
                linkBuilder.MergeAttribute("href", "http://www.gravatar.com");
                linkBuilder.MergeAttribute("title", LinkTitle);
            }

            // output required attributes/img tag:
            var builder = new TagBuilder("img");
            builder.MergeAttribute("width", Size.ToString());
            builder.MergeAttribute("height", Size.ToString());
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", "Gravatar");

            string output = builder.ToString(TagRenderMode.Normal);
            // if we need to output the site link:)
            if (!OutputGravatarSiteLink) return output;
            linkBuilder.InnerHtml = builder.ToString();
            output = linkBuilder.ToString(TagRenderMode.Normal);

            return output;

        }
    }
}