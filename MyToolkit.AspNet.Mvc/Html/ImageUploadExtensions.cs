//-----------------------------------------------------------------------
// <copyright file="CollectionEditorExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MyToolkit.Html
{
    public static class ImageUploadExtensions
    {
        /// <summary>Renders the image upload control to upload a required image.</summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper.</param>
        /// <param name="imageProperty">The image property.</param>
        /// <returns>The HTML string. </returns>
        public static IHtmlString RequiredImageUploadFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> imageProperty)
        {
            return RequiredImageUploadFor(html, imageProperty, null);
        }

        /// <summary>Renders the image upload control to upload a required image.</summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper.</param>
        /// <param name="imageProperty">The image property.</param>
        /// <param name="thumbnailPath">The image thumbnail path.</param>
        /// <param name="maxThumbnailWidth">The maximum thumbnail width (0 = no maximum).</param>
        /// <returns>The HTML string. </returns>
        public static IHtmlString RequiredImageUploadFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> imageProperty,
            string thumbnailPath, int maxThumbnailWidth = 0)
        {
            var imagePathId = "ImagePath_" + Guid.NewGuid().ToString("N");

            var imagePropertyMeta = ModelMetadata.FromLambdaExpression(imageProperty, html.ViewData);
            var imagePropertyName = imagePropertyMeta.PropertyName;

            var output = new StringBuilder();
            output.AppendLine(@"<div>");

            if (!string.IsNullOrEmpty(thumbnailPath))
            {
                RenderImage(output, thumbnailPath, maxThumbnailWidth);
                RenderButton(output, imagePropertyName, imagePathId, "Change image...");
            }
            else
                RenderButton(output, imagePropertyName, imagePathId, "Choose image...");

            output.AppendLine(@"</div>");
            return new HtmlString(output.ToString());
        }

        /// <summary>Renders the image upload control to upload an optional image.</summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper.</param>
        /// <param name="imageProperty">The image property.</param>
        /// <param name="hasImageProperty">The has image property.</param>
        /// <returns>The HTML string. </returns>
        public static IHtmlString ImageUploadFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> imageProperty,
            Expression<Func<TModel, bool>> hasImageProperty)
        {
            return ImageUploadFor(html, imageProperty, hasImageProperty, null);
        }

        /// <summary>Renders the image upload control to upload an optional image.</summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper.</param>
        /// <param name="imageProperty">The image property.</param>
        /// <param name="hasImageProperty">The has image property.</param>
        /// <param name="thumbnailPath">The image thumbnail path.</param>
        /// <param name="maxThumbnailWidth">The maximum thumbnail width (0 = no maximum).</param>
        /// <returns>The HTML string. </returns>
        public static IHtmlString ImageUploadFor<TModel, TValue>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> imageProperty,
            Expression<Func<TModel, bool>> hasImageProperty,
            string thumbnailPath, int maxThumbnailWidth = 0)
        {
            var imagePathId = "ImagePath_" + Guid.NewGuid().ToString("N");

            var imagePropertyMeta = ModelMetadata.FromLambdaExpression(imageProperty, html.ViewData);
            var hasImagePropertyMeta = ModelMetadata.FromLambdaExpression(hasImageProperty, html.ViewData);

            var imagePropertyName = imagePropertyMeta.PropertyName;
            var hasImagePropertyName = hasImagePropertyMeta.PropertyName;

            var output = new StringBuilder();
            output.AppendLine(@"<div>");

            var hasImage = hasImageProperty != null && hasImageProperty.Compile().Invoke(html.ViewData.Model);
            if (hasImage)
            {
                var noImageRadioId = "NoImageRadioId_" + Guid.NewGuid().ToString("N");
                var currentImageRadioId = "CurrentImageRadioId_" + Guid.NewGuid().ToString("N");

                if (!imagePropertyMeta.IsRequired)
                {

                    output.AppendLine(@"<div>");
                    output.AppendLine(@"  <input type=""radio"" id=""" + noImageRadioId + @""" name=""" + hasImagePropertyName + @""" value=""false"">");
                    output.AppendLine(@"  <label for=""" + noImageRadioId + @""">No image</label>");
                    output.AppendLine(@"</div>");

                    output.AppendLine(@"<div>");
                    output.AppendLine(@"  <input type=""radio"" id=""" + currentImageRadioId + @""" name=""" + hasImagePropertyName + @""" value=""true"" checked=""checked"">");
                    output.AppendLine(@"  <label for=""" + currentImageRadioId + @""">Current image:</label><br />");
                    output.AppendLine(@"</div>");
                }

                RenderImage(output, thumbnailPath, maxThumbnailWidth);
                RenderButton(output, imagePropertyName, imagePathId, "Change image...");
            }
            else
            {
                if (!imagePropertyMeta.IsRequired)
                    output.AppendLine(@"<input type=""hidden"" name=""" + hasImagePropertyName + @""" value=""true"" />");

                RenderButton(output, imagePropertyName, imagePathId, "Choose image...");
            }

            output.AppendLine(@"</div>");
            return new HtmlString(output.ToString());
        }

        private static void RenderImage(StringBuilder output, string thumbnailPath, int maxThumbnailWidth)
        {
            output.AppendLine(@"<p>");

            if (maxThumbnailWidth == 0)
                output.AppendLine(@"  <img src=""" + thumbnailPath + @""" alt=""Thumbnail"" />");
            else
            {
                output.AppendLine(@"  <img src=""" + thumbnailPath + @""" alt=""Thumbnail"" " +
                                        @" style=""max-width: " + maxThumbnailWidth + @"px; height: auto"" />");
            }

            output.AppendLine(@"</p>");
        }

        private static void RenderButton(StringBuilder output, string imagePropertyName, string imagePathId, string buttonText)
        {
            output.AppendLine(@"<div style=""position: relative"">");
            output.AppendLine(@"  <a class=""btn btn-default"" href=""javascript:;"">" + buttonText);
            output.AppendLine(@"    <input type=""file"" name=""" + imagePropertyName + @""" size=""40"" ");
            output.AppendLine(@"           onchange='$(""#" + imagePathId + @""").html($(this).val());' ");
            output.AppendLine(@"           style=""position: absolute; z-index: 2; top: 0; left: 0; filter: alpha(opacity=0);" +
                              @"                   opacity: 0; background-color: transparent; color: transparent;""/>");
            output.AppendLine(@"  </a>");
            output.AppendLine(@"  <span class=""label label-info"" id=""" + imagePathId + @"""></span>");
            output.AppendLine(@"</div>");
        }
    }
}
