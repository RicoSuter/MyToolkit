//-----------------------------------------------------------------------
// <copyright file="CollectionEditorExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace MyToolkit.Html
{
    public static class CollectionEditorExtensions
    {
        public static IHtmlString CollectionEditorFor<TModel, TItem>(this HtmlHelper<TModel> htmlHelper,
            Func<TModel, IEnumerable<TItem>> collection, string partialViewName,
            string controllerActionPath, string addButtonTitle, object addButtonHtmlAttributes = null)
        {
            var editorId = "CollectionEditor_" + Guid.NewGuid().ToString("N");
            var addButtonId = "CollectionEditorAdd_" + Guid.NewGuid().ToString("N");

            var html = new StringBuilder();

            RenderInitialCollection(html, htmlHelper, collection, partialViewName, editorId);
            RenderAddButton(html, addButtonId, addButtonTitle, addButtonHtmlAttributes);
            RenderEditorScript(controllerActionPath, html, editorId, addButtonId);

            return new HtmlString(html.ToString());
        }

        private static void RenderInitialCollection<TModel, TItem>(StringBuilder html, HtmlHelper<TModel> htmlHelper,
            Func<TModel, IEnumerable<TItem>> collection, string partialViewName, string editorId)
        {
            html.AppendLine(@"<ul id=""" + editorId + @""" style=""list-style-type: none; padding: 0"">");
            var items = collection(htmlHelper.ViewData.Model);
            if (items != null)
            {
                foreach (var item in collection(htmlHelper.ViewData.Model))
                    html.AppendLine(htmlHelper.Partial(partialViewName, item).ToString());
            }
            html.AppendLine(@"</ul>");
        }

        private static void RenderAddButton(StringBuilder html, string addButtonId, string addButtonTitle, object addButtonHtmlAttributes)
        {
            var inputTag = new TagBuilder("input");
            inputTag.Attributes.Add("type", "button");
            inputTag.Attributes.Add("value", addButtonTitle);
            inputTag.Attributes.Add("id", addButtonId);
            inputTag.MergeAttributes(new RouteValueDictionary(addButtonHtmlAttributes));

            html.AppendLine(@"<p>");
            html.AppendLine(inputTag.ToString());
            html.AppendLine(@"</p>");
        }

        private static void RenderEditorScript(string controllerActionPath, StringBuilder html, string editorId, string addButtonId)
        {
            html.AppendLine(
                @"<script type=""text/javascript"">
                    $(function() {
                        $(""#" + editorId + @""").sortable();
                        $(""#" + addButtonId + @""").click(function() {
                            $.get('" + controllerActionPath + @"', { '_': $.now() }, function (template) {
                                var itemList = $(""#" + editorId + @""");
                                itemList.append(template);

                                var form = itemList.closest(""form"");
                                form.removeData(""validator"");
                                form.removeData(""unobtrusiveValidation"");
                                $.validator.unobtrusive.parse(form);
                                form.validate();
                            });
                        });
                    });
                </script>");
        }

        public static IDisposable BeginCollectionItem<TModel>(this HtmlHelper<TModel> html, string collectionPropertyName)
        {
            var itemIndex = GetCollectionItemIndex(collectionPropertyName);
            var collectionItemName = String.Format("{0}[{1}]", collectionPropertyName, itemIndex);

            var hiddenInput = new TagBuilder("input");
            hiddenInput.MergeAttributes(new Dictionary<string, string> 
            {
                { "name", String.Format("{0}.Index", collectionPropertyName) },
                { "value", itemIndex },
                { "type", "hidden" },
                { "autocomplete", "off" }
            });

            html.ViewContext.Writer.WriteLine(hiddenInput.ToString(TagRenderMode.SelfClosing));
            return new CollectionItemNamePrefixScope(html.ViewData.TemplateInfo, collectionItemName);
        }

        private static string GetCollectionItemIndex(string collectionIndexFieldName)
        {
            Queue<string> previousIndices = (Queue<string>)HttpContext.Current.Items[collectionIndexFieldName];

            if (previousIndices == null)
            {
                previousIndices = new Queue<string>();
                HttpContext.Current.Items[collectionIndexFieldName] = new Queue<string>();

                var previousIndicesValues = HttpContext.Current.Request[collectionIndexFieldName];
                if (!String.IsNullOrWhiteSpace(previousIndicesValues))
                {
                    foreach (var index in previousIndicesValues.Split(','))
                        previousIndices.Enqueue(index);
                }
            }

            return previousIndices.Count > 0 ? previousIndices.Dequeue() : Guid.NewGuid().ToString();
        }

        private class CollectionItemNamePrefixScope : IDisposable
        {
            private readonly TemplateInfo _templateInfo;
            private readonly string _previousPrefix;

            public CollectionItemNamePrefixScope(TemplateInfo templateInfo, string collectionItemName)
            {
                _templateInfo = templateInfo;
                _previousPrefix = templateInfo.HtmlFieldPrefix;

                templateInfo.HtmlFieldPrefix = collectionItemName;
            }

            public void Dispose()
            {
                _templateInfo.HtmlFieldPrefix = _previousPrefix;
            }
        }
    }
}
