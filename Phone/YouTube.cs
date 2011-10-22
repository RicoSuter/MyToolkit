using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using MyToolkit.Network;

namespace MyToolkit.Phone
{
	public enum YouTubeQuality
	{
		Quality480P,
		Quality720P,
		Quality1080P
	}

	public static class YouTube
	{
		public static void Play(string youTubeId, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> onFailure = null)
		{
			Http.Get("http://www.youtube.com/watch?v=" + youTubeId, (s, e) => OnHtmlDownloaded(s, e, maxQuality, onFailure));
		}
		
		private static int GetQualityIdentifier(YouTubeQuality quality)
		{
			switch (quality)
			{
				case YouTubeQuality.Quality480P: return 18;
				case YouTubeQuality.Quality720P: return 22;
				case YouTubeQuality.Quality1080P: return 37;
			}
			throw new ArgumentException("maxQuality");
		}

		private static void OnHtmlDownloaded(string s, Exception e, YouTubeQuality quality, Action<Exception> onFailure)
		{
			if (e == null)
			{
				try
				{
					var match = Regex.Match(s, "url_encoded_fmt_stream_map=(.*?)&");
					var data = Uri.UnescapeDataString(match.Groups[1].Value);

					//match = Regex.Match(data, "^(.*?)\\\\u0026"); // TODO: what for?
					//if (match.Success)
					//	data = match.Groups[1].Value;

					var urls = new List<YouTubeUrl>();
					var arr = data.Split(',');
					foreach (var d in arr)
					{
						var tuple = new YouTubeUrl();
						foreach (var p in d.Split('&'))
						{
							var index = p.IndexOf('=');
							if (index != -1 && index < p.Length)
							{
								var key = p.Substring(0, index);
								var value = Uri.UnescapeDataString(p.Substring(index + 1));
								if (key == "url")
									tuple.Url = value;
								else if (key == "itag")
									tuple.Itag = int.Parse(value);
								else if (key == "type" && value.Contains("video/mp4"))
									tuple.Type = value; 
							}
						}
						
						if (tuple.IsValid)
							urls.Add(tuple);
					}

					var itag = GetQualityIdentifier(quality); 
					foreach (var u in urls.Where(u => u.Itag > itag).ToArray())
						urls.Remove(u);

					var entry = urls.OrderByDescending(u => u.Itag).FirstOrDefault();
					if (entry != null)
					{
						var url = entry.Url;
						var launcher = new MediaPlayerLauncher
						{
							Controls = MediaPlaybackControls.All,
							Media = new Uri(url, UriKind.Absolute)
						};
						launcher.Show();
					}
				}
				catch (Exception ex)
				{
					e = ex;
				}
			}

			if (e != null && onFailure != null)
				onFailure(e);
		}

		private class YouTubeUrl
		{
			public string Url { get; set; }
			public int Itag { get; set; }
			public string Type { get; set; }

			public bool IsValid
			{
				get { return Url != null && Itag > 0 && Type != null; }
			}
		}
	}
}
