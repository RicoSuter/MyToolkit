using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using MyToolkit.Networking;
using MyToolkit.Serialization;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class RssReaderPage
    {
        public RssReaderPage()
        {
            InitializeComponent();
        }

        private async void OnLoadRssFeed(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await Http.GetAsync(RssFeedBox.Text);
                var feed = await RssFeed.FromXmlAsync(response.Response);

                RssFeedListBox.ItemsSource = feed.Items; 

                // TODO: Move to VM and show in view
            }
            catch (Exception exception)
            {
                // TODO: Add exception handling
            }
        }
    }

    [XmlRoot("rss")]
    public class RssFeedRoot
    {
        [XmlElement("channel")]
        public RssFeed Channel { get; set; }
    }

    [XmlRoot("channel")]
    public class RssFeed
    {
        public static async Task<RssFeed> FromXmlAsync(string xml)
        {
            var root = await XmlSerialization.DeserializeAsync<RssFeedRoot>(xml);
            return root.Channel;
        }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("language")]
        public string Language { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("item")]
        public List<RssItem> Items { get; set; } 
    }

    public class RssItem
    {
        [XmlElement("guid")]
        public string Id { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("pubDate")]
        public string Date { get; set; }

        [XmlElement("category")]
        public string[] Categories { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("encoded", Namespace = "http://purl.org/rss/1.0/modules/content/")]
        public string Content { get; set; }
    }
}
