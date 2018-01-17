using Araye.Code.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Araye.Code.Persian
{
   /// <summary>
   /// some codes from DNT Extenstions
   /// </summary>
    public static class Extensions
    {
        #region Fields (4)

        public const char ArabicKeChar = (char)1603;
        public const char ArabicYeChar = (char)1610;
        public const char PersianKeChar = (char)1705;
        public const char PersianYeChar = (char)1740;

        #endregion Fields

        #region PersianProofWriter Methods (7)

        // Public Methods (7) 

        /// <summary>
        /// Adds zwnj char between word and prefix/suffix
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string ApplyHalfSpaceRule(this string text)
        {
            //put zwnj between word and prefix (mi* nemi*)
            var phase1 = Regex.Replace(text, @"\s+(ن?می)\s+", @" $1‌");

            //put zwnj between word and suffix (*tar *tarin *ha *haye)
            var phase2 = Regex.Replace(phase1, @"\s+(تر(ی(ن)?)?|ها(ی)?)\s+", @"‌$1 ");
            return phase2;
        }

        private static string ApplyModeratePersianRules(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.ContainsFarsi())
                return text;

            return text
                       .ApplyPersianYeKe()
                       .ApplyHalfSpaceRule()
                       .YeHeHalfSpace()
                       .CleanupZwnj()
                       .CleanupExtraMarks();
        }

        /// <summary>
        /// Fixes common writing mistakes caused by using a bad keyboard layout,
        /// such as replacing Arabic Ye with Persian one and so on ...
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string ApplyPersianYeKe(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace(ArabicYeChar, PersianYeChar).Replace(ArabicKeChar, PersianKeChar).Trim();
        }

        /// <summary>
        /// Replaces more than one ! or ? mark with just one
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string CleanupExtraMarks(this string text)
        {
            var phase1 = Regex.Replace(text, @"(!){2,}", "$1");
            var phase2 = Regex.Replace(phase1, "(؟){2,}", "$1");
            return phase2;
        }

        /// <summary>
        /// Removes unnecessary zwnj char that are succeeded/preceded by a space
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string CleanupZwnj(this string text)
        {
            return Regex.Replace(text, @"\s+‌|‌\s+", " ");
        }

        /// <summary>
        /// Does text contain Persian characters?
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>true/false</returns>
        public static bool ContainsFarsi(this string text)
        {
            return Regex.IsMatch(text, @"[\u0600-\u06FF]");
        }

        /// <summary>
        /// Converts ه ی to ه‌ی
        /// </summary>
        /// <param name="text">Text to process</param>
        /// <returns>Processed Text</returns>
        public static string YeHeHalfSpace(this string text)
        {
            return Regex.Replace(text, @"(\S)(ه[\s‌]+[یي])(\s)", "$1ه‌ی‌$3"); // fix zwnj
        }

        #endregion Methods

        /// <summary>
        /// Find arabic charecter and replace with valid and standard persian char.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToPersianContent(this object o)
        {
            return ToPersianContent(o, false);
        }

        /// <summary>
        /// Find arabic charecter and replace with valid and standard persian char.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="changeNumbers">If true I change latin numbers to persian.</param>
        /// <returns></returns>
        public static string ToPersianContent(this object o, bool changeNumbers)
        {
            var s = Convert.ToString(o);
            return changeNumbers ? ApplyModeratePersianRules(s).GetPersianNumbers() : ApplyModeratePersianRules(s);
        }

        /// <summary>
        /// Get Persian Numbers
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetPersianNumbers(this string s)
        {
            return s.Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴").Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹");
        }

        public static bool IsItMobileNumber(this string text)
        {
            if (text.Length != 11) return false;
            return text.StartsWith("090") || text.StartsWith("091") || text.StartsWith("092") || text.StartsWith("093");
        }

        #region ValidateNationalCode
        /// <summary>
        /// Validate IR National Code
        /// </summary>
        /// <param name="nationalcode">National Code</param>
        /// <returns></returns>
        public static bool IsValidNationalCode(this string nationalcode)
        {
            int last;
            return nationalcode.IsValidNationalCode(out last);
        }
        /// <summary>
        /// Validate IR National Code
        /// </summary>
        /// <param name="nationalcode">National Code</param>
        /// <param name="lastNumber">Last Number Of National Code</param>
        /// <returns></returns>
        public static bool IsValidNationalCode(this string nationalcode, out int lastNumber)
        {
            lastNumber = -1;
            if (!nationalcode.IsItNumber()) return false;
            var invalid = new[]
                                    {
                                        "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555",
                                        "6666666666", "7777777777", "8888888888", "9999999999"
                                    };
            if (invalid.Contains(nationalcode)) return false;
            var array = nationalcode.ToCharArray();
            if (array.Length != 10) return false;
            var j = 10;
            var sum = 0;
            for (var i = 0; i < array.Length - 1; i++)
            {
                sum += Int32.Parse(array[i].ToString(CultureInfo.InvariantCulture)) * j;
                j--;
            }

            var diff = sum % 11;

            if (diff < 2)
            {
                lastNumber = diff;
                return diff == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
            }
            var temp = Math.Abs(diff - 11);
            lastNumber = temp;
            return temp == Int32.Parse(array[9].ToString(CultureInfo.InvariantCulture));
        }

        public static bool IsMobileNumber(this string text)
        {
            if (text.Length <= 10) return false;
            return text.StartsWith("091") || text.StartsWith("092") || text.StartsWith("093");
        }

        #endregion
    }
}
