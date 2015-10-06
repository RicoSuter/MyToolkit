//-----------------------------------------------------------------------
// <copyright file="HorizontalHtmlGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using MyToolkit.Utilities;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
    public class HorizontalHtmlGenerator
    {
        private readonly WebView _webView;
        //private readonly MethodInfo method;

        public Color Foreground { get; set; }
        public Color Background { get; set; }

        public double HeaderWidth { get; set; }
        public double ColumnWidth { get; set; }
        public double RuleWidth { get; set; }
        public Thickness Padding { get; set; }

        public bool LaunchExternalBrowser { get; set; }

        public HorizontalHtmlGenerator(WebView webView)
        {
            this._webView = webView;

            Foreground = ((SolidColorBrush)webView.Resources["ApplicationForegroundThemeBrush"]).Color;
            Background = ((SolidColorBrush)webView.Resources["ApplicationPageBackgroundThemeBrush"]).Color;

            ColumnWidth = 400;
            RuleWidth = 40;
            HeaderWidth = 600;

            Padding = new Thickness(140, 0, 40, 40);

            LaunchExternalBrowser = true;

            webView.SizeChanged += delegate { Update(); };
            webView.ScriptNotify += OnScriptNotify;
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
            if (_webView.ActualHeight > 0)
            {
                var html = GenerateHtml();
                _webView.NavigateToString(html);
            }
        }

        private async void OnScriptNotify(object sender, NotifyEventArgs e)
        {
            if (LaunchExternalBrowser)
            {
                try
                {
                    var link = e.Value;
                    if (link.StartsWith("LaunchLink: "))
                    {
                        var uri = new Uri(link.Substring("LaunchLink: ".Length), UriKind.RelativeOrAbsolute);
                        await Launcher.LaunchUriAsync(uri);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private string GenerateHtml()
        {
            var linkTransform = @"<script type=""text/javascript"">
for (var i = 0; i < document.links.length; i++) { 
    document.links[i].onclick = function() { 
        window.external.notify('LaunchLink: ' + this.href); return false; 
    } 
}</script>";

            var head = @"<!DOCTYPE html>
                <head>
                    <style type=""text/css"">
                        body {
                            margin: 0; padding: 0;
                            font-family: 'Segoe UI'; -ms-font-feature-settings: 'ss01' 1;
                            font-size: 11pt;
                            background-color: " + ColorUtilities.ToHex(Background) + @";
                        }
                    </style>
                    <meta name=""viewport"" content=""initial-scale=1, maximum-scale=1, user-scalable=0""/>" +
                "<script type=\"text/javascript\">document.documentElement.style.msScrollTranslation = 'vertical-to-horizontal';</script>" +
                "</head>";

            if (string.IsNullOrEmpty(headerHtml))
                return head +
                    @"<html>" +
                        "<body style='margin-left:-16px;margin-top:0px;'>" +
                            "<div style='position: absolute; column-width: " + ColumnWidth + "px; column-fill: auto; " +
                    //"column-rule-width: " + RuleWidth + "px; " +
                                "height:" + (_webView.ActualHeight - Padding.Bottom) + "px; " +
                                "color:" + ColorUtilities.ToHex(Foreground) + "; margin-left:" + Padding.Left + "px; " +
                                "margin-right:" + Padding.Right + "px'>" +
                                    "<div style='margin-right:" + RuleWidth + "px'" +
                                    bodyHtml +
                                    "</div>" +
                                "<div style='height:" + _webView.ActualHeight + "px'>&nbsp;</div>" + // <= used to have margin right => find better solution
                            "</div>" +
                        "</body>" +
                (LaunchExternalBrowser ? linkTransform : "") +
                    "</html>";

            return head +
                "<html>" +
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
                                    "height:" + (_webView.ActualHeight - Padding.Bottom - Padding.Top) + "px; " +
                                    "color:" + ColorUtilities.ToHex(Foreground) + "; " +
                                    "margin-left:" + RuleWidth + "px'>" +
                                        "<div style='margin-right:" + RuleWidth + "px; " +
                                            "'>" +
                                            bodyHtml +
                                        "</div>" +
                                "</div>" +
                            "</td>" +
                        "</table>" +
                    "</body>" +
                (LaunchExternalBrowser ? linkTransform : "") +
                "</html>";
        }
    }
}

#endif