using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyToolkit.Composition
{
    public class AspNetMvcControllerFactory : DefaultControllerFactory
    {
        private readonly CompositionContext _compositionContext;

        public AspNetMvcControllerFactory(CompositionContext compositionContext)
        {
            _compositionContext = compositionContext;
        }

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