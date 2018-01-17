using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Araye.Code.Extensions
{
    /// <summary>
    /// some codes from DNT Extenstions
    /// </summary>
    public static class HtmlExtensions
    {
        private static readonly Regex ChromeWhiteSpace = new Regex(@"style=""white-space\s*:\s*pre\s*;\s*""", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex HtmlComments = new Regex("((<!-- )((?!<!-- ).)*( -->))(\r\n)*", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex HtmlTagExpression = new Regex(@"(?'tag_start'</?)(?'tag'\w+)((\s+(?'attr'(?'attr_name'\w+)(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+)))?)+\s*|\s*)(?'tag_end'/?>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Dictionary<string, List<string>> ValidHtmlTags = new Dictionary<string, List<string>> {
        { "p", new List<string>() },
        { "table", new List<string> { "height", "width", "style" } },
        { "tr", new List<string> { "style" } },
        { "td", new List<string> { "style" } },
        { "tbody", new List<string> { "style" } },
        { "tfoot", new List<string> {"style", "class"}},
        { "th", new List<string> { "style" } },
        { "div", new List<string> { "dir", "align", "style" } },
        { "span", new List<string> { "dir", "color", "align", "style" } },
        { "pre", new List<string> { "language", "name" } },
        { "strong", new List<string>() },
        { "br", new List<string>() },
        { "label", new List<string> {"style", "class"}},
        { "font", new List<string> {"style", "class", "color", "face", "size"}},
        { "h1", new List<string>() },
        { "h2", new List<string>() },
        { "h3", new List<string>() },
        { "h4", new List<string>() },
        { "h5", new List<string>() },
        { "h6", new List<string>() },
        { "blockquote", new List<string> {"style", "class"}},
        { "b", new List<string>() },
        { "hr", new List<string>() },
        { "em", new List<string>() },
        { "i", new List<string>() },
        { "u", new List<string>() },
        { "strike", new List<string>() },
        { "ol", new List<string>() },
        { "ul", new List<string>() },
        { "li", new List<string>() },
        { "a", new List<string> { "href", "style" } },
        { "img", new List<string> { "src", "height", "width", "alt", "style" } },
        { "q", new List<string> { "cite" } },
        { "cite", new List<string>() },
        { "abbr", new List<string>() },
        { "acronym", new List<string>() },
        { "del", new List<string>() },
        { "ins", new List<string>() }};

        private static string CleanHtml(this string html)
        {
            html = ChromeWhiteSpace.Replace(html, string.Empty);
            html = HtmlComments.Replace(html, string.Empty);
            return html;
        }

        /// <summary>
        /// To the safe HTML.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ToSafeHtml(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = text.CleanHtml();
            text = text.RemoveInvalidHtmlTags();
            return text;
        }
        // Private Methods (1) 

        /// <summary>
        /// Removes the invalid HTML tags.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private static string RemoveInvalidHtmlTags(this string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return HtmlTagExpression.Replace(text, m =>
            {
                if (!ValidHtmlTags.ContainsKey(m.Groups["tag"].Value.ToLowerInvariant()))
                    return String.Empty;

                var generatedTag = new StringBuilder(m.Length);

                var tagStart = m.Groups["tag_start"];
                var tagEnd = m.Groups["tag_end"];
                var tag = m.Groups["tag"];
                var tagAttributes = m.Groups["attr"];

                generatedTag.Append(tagStart.Success ? tagStart.Value : "<");
                generatedTag.Append(tag.Value);

                foreach (Capture attr in tagAttributes.Captures)
                {
                    var indexOfEquals = attr.Value.IndexOf('=');

                    // don't proceed any futurer if there is no equal sign or just an equal sign
                    if (indexOfEquals < 1)
                        continue;

                    var attrName = attr.Value.Substring(0, indexOfEquals).ToLowerInvariant();

                    // check to see if the attribute name is allowed and write attribute if it is
                    if (!ValidHtmlTags[tag.Value.ToLowerInvariant()].Contains(attrName)) continue;

                    generatedTag.Append(' ');
                    generatedTag.Append(attr.Value);
                }

                generatedTag.Append(tagEnd.Success ? tagEnd.Value : ">");

                return generatedTag.ToString();
            });
        }

        /// <summary>
        /// Truncates a string containing HTML to a number of text characters, keeping whole words.
        /// The result contains HTML and any tags left open are closed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TruncateHtml(this string html, int maxCharacters, string trailingText)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // find the spot to truncate
            // count the text characters and ignore tags
            var textCount = 0;
            var charCount = 0;
            var ignore = false;
            foreach (char c in html)
            {
                charCount++;
                if (c == '<')
                    ignore = true;
                else if (!ignore)
                    textCount++;

                if (c == '>')
                    ignore = false;

                // stop once we hit the limit
                if (textCount >= maxCharacters)
                    break;
            }

            // Truncate the html and keep whole words only
            var trunc = new StringBuilder(html.TruncateWords(charCount));

            // keep track of open tags and close any tags left open
            var tags = new Stack<string>();
            var matches = Regex.Matches(trunc.ToString(),
                @"<((?<tag>[^\s/>]+)|/(?<closeTag>[^\s>]+)).*?(?<selfClose>/)?\s*>",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var tag = match.Groups["tag"].Value;
                    var closeTag = match.Groups["closeTag"].Value;

                    // push to stack if open tag and ignore it if it is self-closing, i.e. <br />
                    if (!string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(match.Groups["selfClose"].Value))
                        tags.Push(tag);

                    // pop from stack if close tag
                    else if (!string.IsNullOrEmpty(closeTag))
                    {
                        // pop the tag to close it.. find the matching opening tag
                        // ignore any unclosed tags
                        while (tags.Pop() != closeTag && tags.Count > 0)
                        { }
                    }
                }
            }

            if (html.Length > charCount)
                // add the trailing text
                trunc.Append(trailingText);

            // pop the rest off the stack to close remainder of tags
            while (tags.Count > 0)
            {
                trunc.Append("</");
                trunc.Append(tags.Pop());
                trunc.Append('>');
            }

            return trunc.ToString();
        }

        /// <summary>
        /// Truncates a string containing HTML to a number of text characters, keeping whole words.
        /// The result contains HTML and any tags left open are closed.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TruncateHtml(this string html, int maxCharacters)
        {
            return html.TruncateHtml(maxCharacters, null);
        }

        /// <summary>
        /// Strips all HTML tags from a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StripHtml(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            return Regex.Replace(html, @"<(.|\n)*?>", string.Empty);
        }

        /// <summary>
        /// Truncates text to a number of characters
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxCharacters"></param>
        /// <param name="trailingText"></param>
        /// <returns></returns>
        public static string Truncate(this string text, int maxCharacters)
        {
            return text.Truncate(maxCharacters, null);
        }

        /// <summary>
        /// Truncates text to a number of characters and adds trailing text, i.e. elipses, to the end
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxCharacters"></param>
        /// <param name="trailingText"></param>
        /// <returns></returns>
        public static string Truncate(this string text, int maxCharacters, string trailingText)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0 || text.Length <= maxCharacters)
                return text;
            else
                return text.Substring(0, maxCharacters) + trailingText;
        }


        /// <summary>
        /// Truncates text and discars any partial words left at the end
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxCharacters"></param>
        /// <param name="trailingText"></param>
        /// <returns></returns>
        public static string TruncateWords(this string text, int maxCharacters)
        {
            return text.TruncateWords(maxCharacters, null);
        }

        /// <summary>
        /// Truncates text and discars any partial words left at the end
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxCharacters"></param>
        /// <param name="trailingText"></param>
        /// <returns></returns>
        public static string TruncateWords(this string text, int maxCharacters, string trailingText)
        {
            if (string.IsNullOrEmpty(text) || maxCharacters <= 0 || text.Length <= maxCharacters)
                return text;

            // trunctate the text, then remove the partial word at the end
            return Regex.Replace(text.Truncate(maxCharacters),
                @"\s+[^\s]+$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Compiled) + trailingText;
        }

        public static string RemoveHtmlTags(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }


        public static List<Uri> ImagesInHtml(this string htmlSource, HttpContext context, string replaceValue)
        {
            var links = new List<Uri>();
            const string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            var matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {
                try
                {
                    var href = m.Groups[1].Value;
                    if (href.StartsWith(replaceValue))
                    {
                        href = String.Format("{0}://{1}{2}", context.Request.Url.Scheme, context.Request.Url.Authority, href);
                    }
                    links.Add(new Uri(href));
                }
                catch { }
            }
            return links;
        }
    }
}
