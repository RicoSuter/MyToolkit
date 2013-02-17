using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using MyToolkit.Multimedia;
using MyToolkit.Networking;

#if WINRT
using System.Threading.Tasks;
#elif WP7 || WP8
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyToolkit.Paging;
using MyToolkit.UI;
#endif

// developed by Rico Suter (http://rsuter.com), http://mytoolkit.codeplex.com
// this code only works with windows phone mango (video urls with query don't work in previous versions)

namespace MyToolkit.Multimedia
{
	public static class YouTube
	{

#if WINRT
		public static Task<YouTubeUri> GetVideoUriAsync(string youTubeId, YouTubeQuality maxQuality = YouTubeQuality.Quality480P)
		{
			var task = new TaskCompletionSource<YouTubeUri>();
			GetVideoUri(youTubeId, maxQuality, (u, e) =>
			{
				if (u != null)
					task.SetResult(u);
				else if (e == null)
					task.SetCanceled();
				else
					task.SetException(e);
			});
			return task.Task;
		}

		public static async Task<string> GetVideoTitleAsync(string youTubeId) // should be improved
		{
			var response = await Http.GetAsync("http://www.youtube.com/watch?v=" + youTubeId + "&nomobile=1");
			if (response != null)
			{
				var html = response.Response;
				var startIndex = html.IndexOf(" title=\"");
				if (startIndex != -1)
				{
					startIndex = html.IndexOf(" title=\"", startIndex + 1);
					if (startIndex != -1)
					{
						startIndex += 8;
						var endIndex = html.IndexOf("\">", startIndex);
						if (endIndex != -1)
							return html.Substring(startIndex, endIndex - startIndex);
					}
				}
			}
			return null;
		}

#endif

		public static Uri GetThumbnailUri(string youTubeId, YouTubeThumbnailSize size = YouTubeThumbnailSize.Medium)
		{
			switch (size)
			{
				case YouTubeThumbnailSize.Small:
					return new Uri("http://img.youtube.com/vi/" + youTubeId + "/default.jpg", UriKind.Absolute);
				case YouTubeThumbnailSize.Medium:
					return new Uri("http://img.youtube.com/vi/" + youTubeId + "/hqdefault.jpg", UriKind.Absolute);
				case YouTubeThumbnailSize.Large:
					return new Uri("http://img.youtube.com/vi/" + youTubeId + "/maxresdefault.jpg", UriKind.Absolute);
			}
			throw new Exception();
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

		public static HttpResponse GetVideoUri(string youTubeId, YouTubeQuality maxQuality, Action<YouTubeUri, Exception> completed)
		{
			return GetVideoUri(youTubeId, YouTubeQuality.Quality480P, maxQuality, completed);
		}

		public static HttpResponse GetVideoUri(string youTubeId, YouTubeQuality minQuality, YouTubeQuality maxQuality, 
			Action<YouTubeUri, Exception> completed)
		{
			return Http.Get("http://www.youtube.com/watch?v=" + youTubeId + "&nomobile=1", 
				r => OnHtmlDownloaded(r, minQuality, maxQuality, completed));
		}

		private static void OnHtmlDownloaded(HttpResponse response, YouTubeQuality minQuality, YouTubeQuality maxQuality, Action<YouTubeUri, Exception> completed)
		{
			if (response.Successful)
			{
				var urls = new List<YouTubeUri>();
				try
				{
					var match = Regex.Match(response.Response, "url_encoded_fmt_stream_map\": \"(.*?)\"");
					var data = Uri.UnescapeDataString(match.Groups[1].Value);

					var arr = Regex.Split(data, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); // split by comma but outside quotes
					foreach (var d in arr)
					{
						var url = "";
						var signature = "";
						var tuple = new YouTubeUri();
						foreach (var p in d.Replace("\\u0026", "\t").Split('\t'))
						{
							var index = p.IndexOf('=');
							if (index != -1 && index < p.Length)
							{
								try
								{
									var key = p.Substring(0, index);
									var value = Uri.UnescapeDataString(p.Substring(index + 1));
									if (key == "url")
										url = value;
									else if (key == "itag")
										tuple.Itag = int.Parse(value);
									else if (key == "type" && value.Contains("video/mp4"))
										tuple.Type = value;
									else if (key == "sig")
										signature = value;
								}
								catch { }
							}
						}

						tuple.url = url + "&signature=" + signature;
						if (tuple.IsValid)
							urls.Add(tuple);
					}

					var minTag = GetQualityIdentifier(minQuality);
					var maxTag = GetQualityIdentifier(maxQuality);
					foreach (var u in urls.Where(u => u.Itag < minTag || u.Itag > maxTag).ToArray())
					    urls.Remove(u);
				}
				catch (Exception ex)
				{
					if (completed != null)
						completed(null, ex);
					return; 
				}

				var entry = urls.OrderByDescending(u => u.Itag).FirstOrDefault();
				if (entry != null)
				{
					if (completed != null)
						completed(entry, null);
				}
				else if (completed != null)
					completed(null, new Exception("no_video_urls_found"));
			}
			else if (completed != null)
				completed(null, response.Exception);
		}

		public class YouTubeUri
		{
			internal string url;

			public Uri Uri { get { return new Uri(url, UriKind.Absolute); } }
			public int Itag { get; set; }
			public string Type { get; set; }

			public bool IsValid
			{
				get { return url != null && Itag > 0 && Type != null; }
			}
		}
		
		#region Phone

#if WP7 || WP8

		public static HttpResponse Play(string youTubeId, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> completed = null)
		{
			return GetVideoUri(youTubeId, maxQuality, (entry, e) =>
			{
				if (e != null)
			    {
					if (completed != null)
						completed(e);
			    }
				else
			    {
					if (completed != null)
						completed(null);

					if (entry != null)
					{
						 var launcher = new MediaPlayerLauncher
						 {
							 Controls = MediaPlaybackControls.All,
							 Media = entry.Uri
						 };
						 launcher.Show();
					}
			    }
			});
		}


		private static HttpResponse httpResponse;
		private static PageDeactivator oldState;

		/// <summary>
		/// This method disables the current page and shows a progress indicator until the youtube movie url has been loaded and starts
		/// </summary>
		/// <param name="youTubeId"></param>
		/// <param name="manualActivatePage">if true add YouTube.CancelPlay() in OnNavigatedTo() of the page (better)</param>
		/// <param name="maxQuality"></param>
		/// <param name="completed"></param>
		public static void Play(string youTubeId, bool manualActivatePage, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> completed = null)
		{
			lock (typeof(YouTube))
			{
				if (oldState != null)
					return;

				if (SystemTray.ProgressIndicator == null)
					SystemTray.ProgressIndicator = new ProgressIndicator();

				SystemTray.ProgressIndicator.IsVisible = true;
				SystemTray.ProgressIndicator.IsIndeterminate = true; 

				var page = PhonePage.CurrentPage;
				oldState = PageDeactivator.Inactivate();
				httpResponse = Play(youTubeId, YouTubeQuality.Quality480P, ex => Deployment.Current.Dispatcher.BeginInvoke(
					delegate
					{
						if (page == PhonePage.CurrentPage) // !user navigated away
						{
							if (ex == null)
								CancelPlay(manualActivatePage);
							else
								CancelPlay(false);
						}

						if (completed != null)
							completed(ex);
					}));
			}
		}

		/// <summary>
		/// call this in OnBackKeyPress() of the page: 
		/// e.Cancel = YouTube.CancelPlay();
		/// or in OnNavigatedTo() and use manualActivatePage = true
		/// </summary>
		/// <returns></returns>
		public static bool CancelPlay()
		{
			return CancelPlay(false);
		}

		private static bool CancelPlay(bool manualActivate)
		{
			lock (typeof(YouTube))
			{
				if (oldState == null && httpResponse == null)
					return false;

				if (httpResponse != null)
				{
					httpResponse.Abort();
					httpResponse = null;
				}

				if (!manualActivate && oldState != null)
				{
					oldState.Revert();
					SystemTray.ProgressIndicator.IsVisible = false;
					oldState = null;
				}

				return true;
			}
		}

#endif
		#endregion
	}
}
