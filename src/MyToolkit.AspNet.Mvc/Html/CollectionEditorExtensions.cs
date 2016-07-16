//-----------------------------------------------------------------------
// <copyright file="CollectionEditorExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
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
    /// <summary>Provides extension methods to generate a collection editor. </summary>
    public static class CollectionEditorExtensions
    {
        /// <summary>Renders the collection editor for an enumerable.</summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="html">The HTML helper.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="partialViewName">The partial name of the editor view.</param>
        /// <param name="controllerActionPath">The controller action path to generate a new item editor.</param>
        /// <param name="addButtonTitle">The title of the add button.</param>
        /// <param name="addButtonHtmlAttributes">The HTML attributes of the add button.</param>
        /// <param name="viewDataDictionary">The ViewDataDictionary for the partial view.</param>
        /// <returns>The HTML string. </returns>
        public static IHtmlString CollectionEditorFor<TModel, TItem>(this HtmlHelper<TModel> html,
            Func<TModel, IEnumerable<TItem>> collection, string partialViewName,
            string controllerActionPath, string addButtonTitle, object addButtonHtmlAttributes = null, ViewDataDictionary viewDataDictionary = null)
        {
            var editorId = "CollectionEditor_" + Guid.NewGuid().ToString("N");
            var addButtonId = "CollectionEditorAdd_" + Guid.NewGuid().ToString("N");

            var output = new StringBuilder();

            RenderInitialCollection(output, html, collection, partialViewName, editorId, viewDataDictionary);
            RenderAddButton(output, addButtonId, addButtonTitle, addButtonHtmlAttributes);
            RenderEditorScript(controllerActionPath, output, editorId, addButtonId);

            return new HtmlString(output.ToString());
        }

        private static void RenderInitialCollection<TModel, TItem>(StringBuilder output, HtmlHelper<TModel> html,
            Func<TModel, IEnumerable<TItem>> collection, string partialViewName, string editorId, ViewDataDictionary viewDataDictionary = null)
        {
            output.AppendLine(@"<ul id=""" + editorId + @""" style=""list-style-type: none; padding: 0"">");
            var items = collection(html.ViewData.Model);
            if (items != null)
            {
                foreach (var item in collection(html.ViewData.Model))
                {
                    if (viewDataDictionary != null)
                        output.AppendLine(html.Partial(partialViewName, item, viewDataDictionary).ToString());
                    else
                        output.AppendLine(html.Partial(partialViewName, item).ToString());
                }
            }
            output.AppendLine(@"</ul>");
        }

        private static void RenderAddButton(StringBuilder output, string addButtonId, string addButtonTitle, object addButtonHtmlAttributes)
        {
            var inputTag = new TagBuilder("input");
            inputTag.MergeAttributes(new Dictionary<string, string> 
            {
                { "type", "button" },
                { "value", addButtonTitle },
                { "id", addButtonId },
            });
            inputTag.MergeAttributes(new RouteValueDictionary(addButtonHtmlAttributes));

            output.AppendLine(@"<p>");
            output.AppendLine(inputTag.ToString());
            output.AppendLine(@"</p>");
        }

        private static void RenderEditorScript(string controllerActionPath, StringBuilder output, string editorId, string addButtonId)
        {
            output.AppendLine(
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

        /// <summary>Begins the rendering of collection item editor.</summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML helper.</param>
        /// <param name="collectionPropertyName">The name of the collection property in the master view model.</param>
        /// <returns>The disposable. </returns>
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
            var fieldKey = "MyToolkit.CollectionEditorExtensions:" + collectionIndexFieldName;
            Queue<string> previousIndices = (Queue<string>)HttpContext.Current.Items[fieldKey];

            if (previousIndices == null)
            {
                previousIndices = new Queue<string>();
                HttpContext.Current.Items[fieldKey] = new Queue<string>();

                var previousIndicesValues = HttpContext.Current.Request[collectionIndexFieldName + ".Index"];
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