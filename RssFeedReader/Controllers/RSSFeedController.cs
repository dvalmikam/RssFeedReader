using Newtonsoft.Json;
using RssFeedReader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;


namespace RssFeedReader.Controllers
{
    public class RSSFeedController : Controller
    {
        // GET: RSSFeed
        public ActionResult Index()
        {
            ReadFeed();
            return View();
        }

        private void ReadFeed()
        {
            List<string> urls = new List<string>();
            List<RSSFeed> myFeed = new List<RSSFeed>();

            using (StreamReader file = System.IO.File.OpenText(Server.MapPath("~/JsonFiles/FeedProviders.json")))
            {
                JsonTextReader fileReader = new JsonTextReader(new StringReader(file.ReadToEnd()));
                fileReader.SupportMultipleContent = true;
                while (true)
                {
                    if (!fileReader.Read())
                    {
                        break;
                    }
                    JsonSerializer serializer = new JsonSerializer();
                    RSSFeedProvider feedProvider = serializer.Deserialize<RSSFeedProvider>(fileReader);
                    urls.Add(feedProvider.Url);
                }
            }

            foreach (string url in urls)
            {
                XmlReader feedReader = XmlReader.Create(url);
                SyndicationFeed originalFeed = SyndicationFeed.Load(feedReader);
                feedReader.Close();
                SyndicationItem item = originalFeed.Items.ToList()[0];

                //foreach (SyndicationItem item in originalFeed.Items)
                //{
                SyndicationLink lnk = item.Links[0];
                RSSFeed fd = new RSSFeed
                {
                    FeedTitle = originalFeed.Title.Text,
                    PostTitle = item.Title.Text,
                    Link = lnk.Uri.ToString(),
                    //Description = item.Summary.Text,
                    PubDate = item.PublishDate.ToString()
                };
                myFeed.Add(fd);
                //}
            }

            ViewBag.RSSFeed = myFeed.OrderByDescending(e=>e.PubDate);

            //WebClient wclient = new WebClient();
            //string RSSData = wclient.DownloadString(RSSURL);

            //XDocument xml = XDocument.Parse(RSSData);
            //var RSSFeedData = (from x in xml.Descendants("item")
            //                   select new RSSFeed
            //                   {
            //                       Title = ((string)x.Element("title")),
            //                       Link = ((string)x.Element("link")),
            //                       Description = ((string)x.Element("description")),
            //                       PubDate = ((string)x.Element("pubDate"))
            //                   });
            //ViewBag.RSSFeed = RSSFeedData;
            //ViewBag.URL = RSSURL;
        }

        [HttpPost]
        public ActionResult Index(string RSSURL)
        {
            RSSFeedProvider movie = new RSSFeedProvider
            {
                 Url=RSSURL,
                 FeedName=""
            };
            // serialize JSON to a string and then write string to a file
            System.IO.File.AppendAllText(Server.MapPath("~/JsonFiles/FeedProviders.json"), JsonConvert.SerializeObject(movie));
            ReadFeed();         
            return View();
        }
    }
}