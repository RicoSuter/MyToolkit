//-----------------------------------------------------------------------
// <copyright file="IServiceLocator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Composition
{
    /// <summary>The interface for a service locator. </summary>
    public interface IServiceLocator
    {
        /// <summary>Registers a singleton service in the service locator where the service is lazily instantiated. </summary>
        /// <typeparam name="TInterface">The interface type of the service. </typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service. </typeparam>
        void RegisterSingleton<TInterface, TImplementation>();

        /// <summary>Registers a singleton service in the service locator. </summary>
        /// <typeparam name="TInterface">The interface type of the service. </typeparam>
        /// <typeparam name="TImplementation">The implementation type of the service. </typeparam>
        /// <param name="service">The service object. </param>
        void RegisterSingleton<TInterface, TImplementation>(TImplementation service);

        /// <summary>Returns a service object. </summary>
        /// <typeparam name="TInterface">The interface type of the service. </typeparam>
        /// <returns>The service object. </returns>
        TInterface Resolve<TInterface>();
    }
}