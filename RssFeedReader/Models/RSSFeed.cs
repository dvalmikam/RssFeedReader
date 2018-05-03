using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RssFeedReader.Models
{
    public class RSSFeed
    {
        public string FeedTitle { get; set; }
        public string PostTitle { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
    }
}