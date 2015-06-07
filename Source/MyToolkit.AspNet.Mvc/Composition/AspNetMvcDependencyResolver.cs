using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MyToolkit.Composition
{
    public class AspNetMvcDependencyResolver : IDependencyResolver
    {
        private readonly CompositionContext _compositionContext;

        public AspNetMvcDependencyResolver(CompositionContext compositionContext)
        {
            _compositionContext = compositionContext;

            var factory = new AspNetMvcControllerFactory(_compositionContext);
            _compositionContext.AddPart<IControllerFactory, AspNetMvcControllerFactory>(factory);
        }

        public object GetService(Type serviceType)
        {
            return _compositionContext.GetPart(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _compositionContext.GetParts(serviceType);
        }
    }
}