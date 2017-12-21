using Araye.Code.Extensions;
using Araye.Code.Security;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Araye.Code.Content
{
    public class FeedResult : ActionResult
    {
        readonly string _feedTitle;
        readonly List<SyndicationItem> _allItems;
        readonly string _language;
        public FeedResult(string feedTitle, IList<FeedItem> rssItems, string language = "fa-IR")
        {
            _feedTitle = feedTitle;
            _allItems = MapToSyndicationItem(rssItems);
            _language = language;
        }

        private static List<SyndicationItem> MapToSyndicationItem(IList<FeedItem> rssItems)
        {
            var results = new List<SyndicationItem>();
            foreach (var item in rssItems)
            {
                var uri = new Uri(item.Url);
                var feedItem = new SyndicationItem(
                        title: item.Title.CorrectRtl(),
                        content: item.Content.CorrectRtlBody(),
                        itemAlternateLink: uri,
                        id: item.Url.SHA1(),
                        lastUpdatedTime: item.LastUpdatedTime
                        );
                feedItem.Authors.Add(new SyndicationPerson(item.AuthorName, item.AuthorName, uri.Host));
                results.Add(feedItem);
            }
            return results;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;
            WriteToResponse(response);
        }

        private void WriteToResponse(HttpResponseBase response)
        {
            var feed = new SyndicationFeed
            {
                Title = new TextSyndicationContent(_feedTitle.CorrectRtl()),
                Language = _language,
                Items = _allItems
            };
            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "application/rss+xml";
            using (var rssWriter = XmlWriter.Create(response.Output, new XmlWriterSettings { Indent = true }))
            {
                var formatter3 = new Rss20FeedFormatter(feed);
                formatter3.WriteTo(rssWriter);
                rssWriter.Close();
                response.End();
            }
        }
    }
}
