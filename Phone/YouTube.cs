using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyToolkit.Network;
using MyToolkit.UI.Popups;

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
		public static HttpResponse Play(string youTubeId, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> onFinished = null)
		{
			return Http.Get("http://www.youtube.com/watch?v=" + youTubeId, r => OnHtmlDownloaded(r, maxQuality, onFinished));
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

		private static void OnHtmlDownloaded(HttpResponse response, YouTubeQuality quality, Action<Exception> onFinished)
		{
			if (response.Successful)
			{
				var urls = new List<YouTubeUrl>();
				try
				{
					var match = Regex.Match(response.Response, "url_encoded_fmt_stream_map=(.*?)&");
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
					if (onFinished != null)
						onFinished(ex);
					return; 
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
			else if (onFinished != null)
				onFinished(response.Exception);
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
		
		#region Phone

		private static HttpResponse httpResponse;
		private static PageDeactivator oldState;

		/// <summary>
		/// This method disables the current page and shows a progress indicator until the youtube movie url has been loaded and starts
		/// </summary>
		/// <param name="youTubeId"></param>
		/// <param name="manualActivatePage">if true add YouTube.CancelPlay() in OnNavigatedTo() of the page (better)</param>
		/// <param name="maxQuality"></param>
		/// <param name="onFailure"></param>
		public static void Play(string youTubeId, bool manualActivatePage, YouTubeQuality maxQuality = YouTubeQuality.Quality480P, Action<Exception> onFailure = null)
		{
			lock (typeof(YouTube))
			{
				if (oldState != null)
					return;

				if (SystemTray.ProgressIndicator == null)
					SystemTray.ProgressIndicator = new ProgressIndicator();
				SystemTray.ProgressIndicator.IsVisible = true;

				var page = PhoneApplication.CurrentPage;
				oldState = PageDeactivator.Inactivate();
				httpResponse = Play(youTubeId, YouTubeQuality.Quality480P, ex => Deployment.Current.Dispatcher.BeginInvoke(
					delegate
					{
						if (page != PhoneApplication.CurrentPage) // user navigated away
							return;

						if (ex != null && onFailure != null)
						{
							onFailure(ex);
							CancelPlay(false);
						}
						else
							CancelPlay(manualActivatePage);
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
		
		#endregion
	}
}
