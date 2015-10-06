//-----------------------------------------------------------------------
// <copyright file="AspNetMvcControllerFactory.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyToolkit.Composition
{
    /// <summary>A controller factory using the <see cref="CompositionContext"/>.</summary>
    public class AspNetMvcControllerFactory : DefaultControllerFactory
    {
        private readonly CompositionContext _compositionContext;

        /// <summary>Initializes a new instance of the <see cref="AspNetMvcControllerFactory"/> class.</summary>
        /// <param name="compositionContext">The composition context.</param>
        public AspNetMvcControllerFactory(CompositionContext compositionContext)
        {
            _compositionContext = compositionContext;
        }

        /// <summary>Retrieves the controller instance for the specified request context and controller type.</summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>The controller instance.</returns>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return null; 

            var constructor = controllerType.GetConstructors().First();

            var parameters = constructor
                .GetParameters()
                .Select(argument => _compositionContext.GetPart(argument.ParameterType, null))
                .ToArray();

            return (IController)constructor.Invoke(parameters);
        }
    }
}