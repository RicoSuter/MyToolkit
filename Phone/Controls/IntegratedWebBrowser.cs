using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using MyToolkit.Animations;
using MyToolkit.Utilities;

namespace MyToolkit.Controls
{
	public class IntegratedWebBrowser : Control
	{
		public IntegratedWebBrowser()
		{
			FontSize = (double)Resources["PhoneFontSizeMedium"];
			DefaultStyleKey = typeof(IntegratedWebBrowser);
		}

		private WebBrowser browser;
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			Background = (Brush)Resources["PhoneBackgroundBrush"];

			browser = (WebBrowser)GetTemplateChild("browser");
			browser.Background = (Brush)Resources["PhoneBackgroundBrush"];
			browser.SizeChanged += delegate { UpdateHtml(); };
			browser.Opacity = 0.0;
			browser.LoadCompleted += OnLoadCompleted; 
			browser.Navigating += OnNavigating;

			//timer = new Timer(OnTimer, null, 500, ConnectionTimeout.Infinite);

			//Update();
		}

		private bool isFading = false; 
		private void OnTimer(object state)
		{
			Dispatcher.BeginInvoke(delegate
			{
				FadeIn();
				timer.Dispose();
			});
		}

		private void FadeIn()
		{
			if (!isFading)
			{
				isFading = true;
				Fading.FadeIn(browser, 200);
			}
		}

		private Timer timer; 

		public event LoadCompletedEventHandler LoadCompleted;

		private void OnLoadCompleted(object sender, NavigationEventArgs e)
		{
			FadeIn();
			var copy = LoadCompleted;
			if (copy != null)
				copy(this, e);
		}

		public static readonly DependencyProperty BaseUriProperty =
			DependencyProperty.Register("BaseUri", typeof (Uri), typeof (IntegratedWebBrowser), new PropertyMetadata(default(Uri)));

		public Uri BaseUri
		{
			get { return (Uri) GetValue(BaseUriProperty); }
			set { SetValue(BaseUriProperty, value); }
		}

		private void OnNavigating(object sender, NavigatingEventArgs e)
		{
			e.Cancel = true;

			var link = e.Uri.ToString();
			var uri = link.StartsWith("http://") || link.StartsWith("https://") ? e.Uri : new Uri(BaseUri, link);
			new WebBrowserTask { Uri = uri }.Show();
		}

		public static readonly DependencyProperty HtmlProperty =
			DependencyProperty.Register("Html", typeof (string), typeof (IntegratedWebBrowser), 
			new PropertyMetadata(default(string), OnHtmlChanged));

		private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (IntegratedWebBrowser)d;
			ctrl.UpdateHtml();
		}

		public string Html
		{
			get { return (string) GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		public static readonly DependencyProperty CssProperty =
			DependencyProperty.Register("Css", typeof (string), typeof (IntegratedWebBrowser), new PropertyMetadata(default(string)));

		public string Css
		{
			get { return (string) GetValue(CssProperty); }
			set { SetValue(CssProperty, value); }
		}

		private void UpdateHtml()
		{
			if (!(ActualWidth > 0))
				return;
			
			var fontColor = ColorUtility.ToHex((Color)Resources["PhoneForegroundColor"]);
			var backgroundColor = ColorUtility.ToHex((Color)Resources["PhoneBackgroundColor"]);

			//var script = "<script>function getDocHeight() { " +
			//  "return document.getElementById('pageWrapper').offsetHeight; }" +
			//  "function SendDataToPhoneApp() {" +
			//  "window.external.Notify('' + getDocHeight());" +
			//  "}</script>";

			//var script = "<script>window.external.Notify('loaded');</script>";
			var html = "<html>" +
					"<head>" +
					"<style>" +
						"body { font-family: " + Resources["PhoneFontFamilyNormal"] + ";width:" + ActualWidth.ToString() + "px;" +
						"margin:0px; padding: 0px; background-color:" + backgroundColor + "; " +
						"font-size: 17pt }\n" +

						"a { color:" + fontColor + "; }\n" +
						"ul { margin:0; padding-left:40px; }\n" +
						"p { padding:0; margin-left:0px; margin-top:0px; margin-right:0px; margin-bottom:12px; }\n" +

						"h1 { font-family: " + Resources["PhoneFontFamilySemiLight"] + "; " +
						"font-size: 54pt; " +
						"padding:0; margin-left:0px; margin-top:12px; margin-right:0px; margin-bottom:12px;}\n" +
					
						"h2 { font-family: " + Resources["PhoneFontFamilySemiLight"] + "; " +
						"font-size: 24pt; " +
						"padding:0; margin-left:0px; margin-top:12px; margin-right:0px; margin-bottom:12px;}\n" +

						"small { font-family: " + Resources["PhoneFontFamilyNormal"] + "; " +
						"font-size: 15pt; }\n" +
						Css + "\n" +
					"</style>" +
					"<meta name=\"viewport\" content=\"initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0\" />" +
					"<meta name=\"viewport\" content=\"user-scalable=no\" />" +
					"<meta name=\"viewport\" content=\"width=" + ActualWidth + "\" />" +
					"</head>" +

					"<body onLoad=\"SendDataToPhoneApp()\">" +
					"<div id=\"pageWrapper\" style=\"width: 100%; color:" + fontColor + ";\" >" +
					Html.Replace("%width%", ActualWidth.ToString()) +
					"</div></body></html>";

			browser.IsScriptEnabled = true;
			//browser.ScriptNotify += OnScriptNotify;
			browser.NavigateToString(html);
		}

		//private void OnScriptNotify(object sender, NotifyEventArgs e)
		//{
		//    Fading.FadeIn(browser, 200);

		//    //fullHeight = Convert.ToDouble(e.Value);
		//    //Update();
		//}

		//public static readonly DependencyProperty FullHeightProperty =
		//    DependencyProperty.Register("FullHeight", typeof (bool), typeof (IntegratedWebBrowser), 
		//    new PropertyMetadata(default(bool), OnFullHeightChanged));

		//private static void OnFullHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		//{
		//    var ctrl = (IntegratedWebBrowser) d;
		//    ctrl.Update();
		//}

		//public bool FullHeight
		//{
		//    get { return (bool) GetValue(FullHeightProperty); }
		//    set { SetValue(FullHeightProperty, value); }
		//}

		//private double fullHeight = 0.0;

		//private void Update()
		//{
		//    if (browser != null)
		//    {
		//        browser.IsHitTestVisible = FullHeight;
		//        if (FullHeight)
		//            browser.Height = fullHeight;
		//    }
		//}
	}
}
