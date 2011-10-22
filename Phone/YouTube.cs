using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyToolkit.Network;

// developed by Rico Suter (rsuter.com)
// this code only works with mango (video urls with query don't work in previous versions)

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
		public static void Play(string youTubeId, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> onFinished = null)
		{
			Http.Get("http://www.youtube.com/watch?v=" + youTubeId, (s, e) => OnHtmlDownloaded(s, e, maxQuality, onFinished));
		}

		/// <summary>
		/// This method disables the current page and shows a progress indicator until the youtube movie has been loaded and will start soon
		/// </summary>
		/// <param name="youTubeId"></param>
		/// <param name="maxQuality"></param>
		/// <param name="onFinished"></param>
		public static void PlayWithProgressIndicator(string youTubeId, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> onFailure = null)
		{
			PhoneApplication.CurrentPage.IsEnabled = false;
			if (SystemTray.ProgressIndicator == null)
				SystemTray.ProgressIndicator = new ProgressIndicator();
			SystemTray.ProgressIndicator.IsVisible = true; 

			Play(youTubeId, YouTubeQuality.Quality480P, ex => Deployment.Current.Dispatcher.BeginInvoke(
				delegate
				{
					if (ex != null && onFailure != null)
						onFailure(ex);

					PhoneApplication.CurrentPage.IsEnabled = true;
					SystemTray.ProgressIndicator.IsVisible = false;
				}));
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

		private static void OnHtmlDownloaded(string s, Exception e, YouTubeQuality quality, Action<Exception> onFinished)
		{
			if (e == null)
			{
				var urls = new List<YouTubeUrl>();
				try
				{
					var match = Regex.Match(s, "url_encoded_fmt_stream_map=(.*?)&");
					var data = Uri.UnescapeDataString(match.Groups[1].Value);

					//match = Regex.Match(data, "^(.*?)\\\\u0026"); // TODO: what for?
					//if (match.Success)
					//	data = match.Groups[1].Value;

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

				}
				catch (Exception ex)
				{
					e = ex;
				}

				var entry = urls.OrderByDescending(u => u.Itag).FirstOrDefault();
				if (entry != null)
				{
					if (onFinished != null)
						onFinished(null);

					var url = entry.Url;
					var launcher = new MediaPlayerLauncher
					{
						Controls = MediaPlaybackControls.All,
						Media = new Uri(url, UriKind.Absolute)
					};
					launcher.Show();
				}
			}

			if (e != null && onFinished != null)
				onFinished(e);
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
