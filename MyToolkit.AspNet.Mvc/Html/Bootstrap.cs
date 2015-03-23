//-----------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace MyToolkit.Html
{
    public static class Bootstrap
    {
        /// <summary>Creates the pagination bootstrap component.</summary>
        /// <param name="page">The <see cref="WebViewPage"/>. Use <c>this</c> in a Razor view.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="pageCount">The page count.</param>
        /// <param name="actionName">The name of the controller action.</param>
        /// <param name="pageKeyName">The page key name. </param>
        /// <param name="routeValues">The route values. </param>
        /// <returns>The HTML string. </returns>
        public static IHtmlString Pagination(WebViewPage page, int currentPage, int pageCount, string actionName, string pageKeyName = "page", object routeValues = null)
        {
            var html = new StringBuilder();
            html.AppendLine(@"<nav>");
            html.AppendLine(@" <ul class=""pagination"">");

            // previous page
            var previousValues = new RouteValueDictionary(routeValues);
            previousValues.Add(pageKeyName, currentPage - 1);

            html.AppendLine(@"  <li class=""" + (currentPage == 1 ? "disabled" : string.Empty) + @""">");
            html.AppendLine(@"   <a href=""" + (currentPage == 1 ? "#" : page.Url.Action(actionName, previousValues)) + @""" aria-label=""Previous"">");
            html.AppendLine(@"    <span aria-hidden=""true"">&laquo;</span>");
            html.AppendLine(@"   </a>");
            html.AppendLine(@"  </li>");

            // pages
            for (int i = 1; i <= pageCount; i++)
            {
                var values = new RouteValueDictionary(routeValues);
                values.Add(pageKeyName, i);

                html.AppendLine(@"  <li class=""" + (i == currentPage ? "active" : string.Empty) + @""">" + page.Html.ActionLink(i.ToString(), actionName, values) + @"</li>");
            }

            // next page
            var nextValues = new RouteValueDictionary(routeValues);
            nextValues.Add(pageKeyName, currentPage + 1);

            html.AppendLine(@"  <li class=""" + (currentPage == pageCount ? "disabled" : string.Empty) + @""">");
            html.AppendLine(@"   <a href=""" + (currentPage == pageCount ? "#" : page.Url.Action(actionName, nextValues)) + @""" aria-label=""Next"">");
            html.AppendLine(@"    <span aria-hidden=""true"">&raquo;</span>");
            html.AppendLine(@"   </a>");
            html.AppendLine(@"  </li>");

            html.AppendLine(@" </ul>");
            html.AppendLine(@"</nav>");

            return new HtmlString(html.ToString());
        }
    }
}
