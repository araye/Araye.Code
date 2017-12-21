using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Araye.Code.Extensions
{
    /// <summary>
    /// some codes from DNT Extenstions
    /// </summary>
    public static class RtlExtensions
    {
        private static readonly Regex MatchArabicHebrew =
                    new Regex(@"[\u0600-\u06FF,\u0590-\u05FF]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string CorrectRtl(this string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            const char rleChar = (char)0x202B;
            if (MatchArabicHebrew.IsMatch(title))
                return rleChar + title;
            return title;
        }

        public static string CorrectRtlBody(this string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return string.Empty;

            if (MatchArabicHebrew.IsMatch(body))
                return "<div style='text-align: right; font-family:tahoma; font-size:9pt;' dir='rtl'>" + body + "</div>";
            return "<div style='text-align: left; font-family:tahoma; font-size:9pt;' dir='ltr'>" + body + "</div>";
        }

        public static string CorrectRtlBody(this string body, string rtlStyle , string ltrStyle)
        {
            if (string.IsNullOrWhiteSpace(body)) return string.Empty;

            if (MatchArabicHebrew.IsMatch(body))
                return "<div style='"+ rtlStyle + "' dir='rtl'>" + body + "</div>";
            return "<div style='"+ltrStyle+"' dir='ltr'>" + body + "</div>";
        }
    }
}
