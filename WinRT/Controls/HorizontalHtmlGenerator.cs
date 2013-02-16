using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public class HorizontalHtmlGenerator
	{
		private readonly FrameworkElement webView;
		private readonly MethodInfo method;

		public Color Foreground { get; set; }
		public Color Background { get; set; }

		public double HeaderWidth { get; set; }
		public double ColumnWidth { get; set; }
		public double RuleWidth { get; set; }
		public Thickness Padding { get; set; }

		public HorizontalHtmlGenerator(FrameworkElement webView)
		{
			this.webView = webView;

			method = webView.GetType().GetTypeInfo().GetDeclaredMethod("NavigateToString");

			Foreground = ((SolidColorBrush)webView.Resources["ApplicationForegroundThemeBrush"]).Color;
			Background = ((SolidColorBrush)webView.Resources["ApplicationPageBackgroundThemeBrush"]).Color;

			ColumnWidth = 400;
			RuleWidth = 40;
			HeaderWidth = 600; 
			
			Padding = new Thickness(140, 0, 40, 40);
			
			webView.SizeChanged += delegate { Update(); };
		}

		private string headerHtml;
		private string bodyHtml; 

		public void SetHtml(string headerHtml, string bodyHtml)
		{
			this.headerHtml = headerHtml;
			this.bodyHtml = bodyHtml;
			Update();
		}

		public void SetHtml(string bodyHtml)
		{
			SetHtml(null, bodyHtml);
		}

		public void Update()
		{
			if (webView.ActualHeight > 0)
			{
				var h = GenerateHtml();
				// used to avoid marketplace static analysis problems (webview always used)
				method.Invoke(webView, new object[] { h });
			}
		}

		private string GenerateHtml()
		{
			if (string.IsNullOrEmpty(headerHtml))
				return
					@"<!DOCTYPE html>
						<head>
							<style type=""text/css"">
								body {
									margin: 0; padding: 0;
									font-family: 'Segoe UI'; -ms-font-feature-settings: 'ss01' 1;
									font-size: 11pt;
									background-color: " + ColorUtility.ToHex(Background)+ @";
								}
							</style>
							<meta name=""viewport"" content=""initial-scale=1, maximum-scale=1, user-scalable=0""/>" +
							//"<script type=\"text/javascript\">document.documentElement.style.msScrollTranslation = 'vertical-to-horizontal';</script>" +
						"</head><html>" +
						"<body style='margin-left:-16px;margin-top:0px;'>" +
							"<div style='position: absolute; column-width: " + ColumnWidth + "px; column-fill: auto; " +
								//"column-rule-width: " + RuleWidth + "px; " +
								"height:" + (webView.ActualHeight - Padding.Bottom) + "px; " +
								"color:" + ColorUtility.ToHex(Foreground) + "; margin-left:" + Padding.Left + "px; " +
								"margin-right:" + Padding.Right + "px'>" +
									"<div style='margin-right:" + RuleWidth + "px'" +
									bodyHtml +
									"</div>" +
								"<div style='height:" + webView.ActualHeight + "px'>&nbsp;</div>" + // <= used to have margin right => find better solution
							"</div>" +
						"</body>" +
					"</html>";
			else
				return
					@"<!DOCTYPE html>
						<html style='background-color:" + ColorUtility.ToHex(Background) + "'>" +
						@"<style type=""text/css"">
							body{
								margin: 0; padding: 0;
								font-family: 'Segoe UI'; -ms-font-feature-settings: 'ss01' 1;
								font-size: 11pt;
							}
						</style>" +
						"<body style='margin-left:-16px;margin-top:0px;'>" +
							"<table>" +
								"<td width='" + HeaderWidth + "' style='width:" + HeaderWidth + "px'>" +
									"<div style='margin-left:" + Padding.Left + "px'>" + 
										headerHtml + 
									"</div>" +
								"</td>" +
								"<td>" +
									"<div style='position: absolute; column-width: " + ColumnWidth + "px; top:" + Padding.Top + "px; column-fill: auto; " +
										//"column-rule-width: " + RuleWidth + "px; " +
										"height:" + (webView.ActualHeight - Padding.Bottom - Padding.Top) + "px; " +
										"color:" + ColorUtility.ToHex(Foreground) + "; " +
										"margin-left:" + RuleWidth + "px'>" +
											"<div style='margin-right:" + RuleWidth + "px; " +
												"'>" +
												bodyHtml +
											"</div>" + 
								   "</div>" +
								"</td>" +
							"</table>" + 
						"</body>" +
					"</html>";
		}
	}
}
