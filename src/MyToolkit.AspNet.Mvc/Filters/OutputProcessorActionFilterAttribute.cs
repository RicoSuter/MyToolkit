//-----------------------------------------------------------------------
// <copyright file="OutputProcessorActionFilterAttribute.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Web.Mvc;

// ReSharper disable ExceptionNotDocumented

namespace MyToolkit.Filters
{
    /// <summary>Processes the output of an action before it is transmitted to the client.</summary>
    public abstract class OutputProcessorActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>Processes the output data.</summary>
        /// <param name="data">The data.</param>
        /// <returns>The processed data.</returns>
        protected abstract string Process(string data);

        /// <summary>Called by the ASP.NET MVC framework after the action result executes.</summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.Filter = new OutputProcessorStream(response.Filter, response.ContentEncoding, response.ContentEncoding, Process);
        }
    }
}