//-----------------------------------------------------------------------
// <copyright file="AspNetMvcDependencyResolver.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyToolkit.Composition
{
    /// <summary>A dependency resolver using the <see cref="CompositionContext"/>.</summary>
    public class AspNetMvcDependencyResolver : IDependencyResolver
    {
        private readonly CompositionContext _compositionContext;

        /// <summary>Initializes a new instance of the <see cref="AspNetMvcDependencyResolver"/> class.</summary>
        /// <param name="compositionContext">The composition context.</param>
        public AspNetMvcDependencyResolver(CompositionContext compositionContext)
        {
            _compositionContext = compositionContext;

            var factory = new AspNetMvcControllerFactory(_compositionContext);
            _compositionContext.AddPart<IControllerFactory, AspNetMvcControllerFactory>(factory);
        }

        /// <summary>Resolves singly registered services that support arbitrary object creation.</summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>The requested service or object.</returns>
        public object GetService(Type serviceType)
        {
            return _compositionContext.GetPart(serviceType);
        }

        /// <summary>Resolves multiply registered services.</summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _compositionContext.GetParts(serviceType);
        }
    }
}