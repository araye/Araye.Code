using System.IO;
using System.Linq;

namespace Araye.Code.Extensions
{
    public static class IoExtensions
    {
        /// <summary>
        /// Delete files in a folder that are like the searchPattern, don't include subfolders.
        /// Example : 
        /// DirectoryInfo di = new DirectoryInfo(@"c:\temp");
        /// di.DeleteFiles("*.xml");  // Delete all *.xml files 
        /// </summary>
        /// <param name="di"></param>
        /// <param name="searchPattern">DOS like pattern (example: *.xml, ??a.txt)</param>
        /// <returns>Number of files that have been deleted.</returns>
        public static int DeleteFiles(this DirectoryInfo di, string searchPattern)
        {
            return DeleteFiles(di, searchPattern, false);
        }

        /// <summary>
        /// Delete files in a folder that are like the searchPattern
        /// Example : 
        /// DirectoryInfo di = new DirectoryInfo(@"c:\temp");
        /// di.DeleteFiles("*.xml", true);  // Delete all, recursively
        /// </summary>
        /// <param name="di"></param>
        /// <param name="searchPattern">DOS like pattern (example: *.xml, ??a.txt)</param>
        /// <param name="includeSubdirs"></param>
        /// <returns>Number of files that have been deleted.</returns>
        /// <remarks>
        /// This function relies on DirectoryInfo.GetFiles() which will first get all the FileInfo objects in memory. This is good for folders with not too many files, otherwise
        /// an implementation using Windows APIs can be more appropriate. I didn't need this functionality here but if you need it just let me know.
        /// </remarks>
        public static int DeleteFiles(this DirectoryInfo di, string searchPattern, bool includeSubdirs)
        {
            int deleted = 0;
            foreach (FileInfo fi in di.GetFiles(searchPattern, includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                fi.Delete();
                deleted++;
            }

            return deleted;
        }

        /// <summary>
        /// Get Size Of Directory.
        /// Example : 
        /// DirectoryInfo WindowsDir = new DirectoryInfo(@"C:\WINDOWS");
        /// long WindowsSize = WindowsDir.GetSize();
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>Size Of Directory.</returns>
        public static long GetSize(this DirectoryInfo dir)
        {
            var length = dir.GetFiles().Sum(nextfile => nextfile.Exists ? nextfile.Length : 0);
            length += dir.GetDirectories().Sum(nextdir => nextdir.Exists ? GetSize(nextdir) : 0);
            return length;
        }

        /// <summary>
        /// Recursively create directory.
        /// Raya Farayand Method, Example : 
        /// string path = @"C:\temp\one\two\three";
        /// var dir = new DirectoryInfo(path);
        /// dir.CreateDirectory();
        /// </summary>
        /// <param name="dirInfo">Folder path to create.</param>
        public static void CreateDirectory(this DirectoryInfo dirInfo)
        {
            if (dirInfo.Parent != null) CreateDirectory(dirInfo.Parent);
            if (!dirInfo.Exists) dirInfo.Create();
        }


        /// <summary>
        /// Get the file size of a given filename.
        /// Example:
        /// string path = @"D:\WWW\Proj\web.config";
        /// Console.WriteLine("File Size is: {0} bytes.", path.FileSize());
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long FileSize(this string filePath)
        {
            var oFileInfo = new FileInfo(filePath);
            var bytes = oFileInfo.Length;
            return bytes;
        }

        /// <summary>
        /// Nicely formatted file size. This method will return file size with bytes, KB, MB and GB in it. 
        /// You can use this alongside the Extension method named FileSize.
        /// Example: 
        /// Using another Extension Method: FileSize to get the size of the file
        /// string path = @"D:\WWW\Proj\web.config";
        /// Console.WriteLine("File Size is: {0}.", path.FileSize().FormatSize());
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public static string FormatFileSize(this long fileSize)
        {
            string[] suffix = { "bytes", "KB", "MB", "GB" };
            long j = 0;

            while (fileSize > 1024 && j < 4)
            {
                fileSize = fileSize / 1024;
                j++;
            }
            return (fileSize + " " + suffix[j]);
        }

        
    }
}
