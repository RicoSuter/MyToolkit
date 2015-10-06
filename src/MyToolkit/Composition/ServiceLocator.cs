//-----------------------------------------------------------------------
// <copyright file="ServiceLocator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Composition
{
    /// <summary>A service locator implementation. </summary>
    public class ServiceLocator : IServiceLocator
    {
        private static IServiceLocator _default;
        private readonly CompositionContext _context = new CompositionContext();

        /// <summary>Gets or sets the default service locator. </summary>
        public static IServiceLocator Default
        {
            get
            {
                if (_default == null)
                {
                    lock (typeof(ServiceLocator))
                    {
                        if (_default == null)
                            _default = new ServiceLocator();
                    }
                }
                return _default;
            }
            set { _default = value; }
        }

        /// <summary>Registers a singleton service in the service locator where the service is lazily instantiated. </summary>
        /// <typeparam name="TInterface">The interface type of the service. </typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service. </typeparam>
        public void RegisterSingleton<TInterface, TImplementation>()
        {
            _context.AddPart<TInterface, TImplementation>();
        }

        /// <summary>Registers a singleton service in the service locator. </summary>
        /// <typeparam name="TInterface">The interface type of the service. </typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service. </typeparam>
        /// <param name="service">The service object. </param>
        public void RegisterSingleton<TInterface, TImplementation>(TImplementation service)
        {
            _context.AddPart<TInterface, TImplementation>(service);
        }

        /// <summary>Returns a service object. </summary>
        /// <typeparam name="TInterface">The interface type of the service. </typeparam>
        /// <returns>The service object. </returns>
        public TInterface Resolve<TInterface>()
        {
            return _context.GetPart<TInterface>();
        }
    }
}
