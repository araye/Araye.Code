using Araye.Code.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Araye.Code.Content
{
    public class Utils
    {
        public static string GetUniqueFilePath(string directoryName, string fileName)
        {
            var validatedName = Guid.NewGuid().ToStringSafe() + fileName;
            while (File.Exists(String.Format("{0}/{1}", directoryName, validatedName)))
            {
                validatedName = string.Format("{0}_{1}", Guid.NewGuid().ToStringSafe(), fileName);
            }
            return validatedName;
        }

        public static string GetSlug(string title, int length)
        {
            var end = title.Length > length ? length : title.Length;
            var slug = title.Substring(0, end);
            return slug = slug.Replace(" ", "-").Replace(":", "-").Replace(".", "-");
        }

        public static string GetSlug(string title)
        {
            return GetSlug(title, title.Length);
        }

        public static string UnSlug(string tilte)
        {
            return tilte.Replace("-", " ");
        }

        public static string GetUploadedImage(string code, string path, string defaultImagePath)
        {
            try
            {
                var urlPath = String.Format("{0}/{1}", path, code);
                var uploadPath = HttpContext.Current.Server.MapPath(urlPath);
                List<string> files = null;
                if (Directory.Exists(uploadPath))
                    files = Directory.EnumerateFiles(uploadPath).ToList();
                if (files != null)
                    return files.Select(a => urlPath.TrimStart('~') + "/" + Path.GetFileName(a)).FirstOrDefault();
            }
            catch { }
            return defaultImagePath;
        }

        
    }
}
