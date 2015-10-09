//-----------------------------------------------------------------------
// <copyright file="AdditionalStreamExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WPF

using System;

namespace MyToolkit.Extended.Utilities
{
    /// <summary>Creates an instance of the generic object type in a new 
    /// AppDomain so that methods on the instance can be executed in isolation.</summary>
    /// <typeparam name="TObject">The object type.</typeparam>
    public class AppDomainIsolation<TObject> : IDisposable where TObject : MarshalByRefObject
    {
        private AppDomain _domain;
        private readonly TObject _object;

        /// <summary>Initializes a new instance of the <see cref="AppDomainIsolation{TObject}"/> class.</summary>
        public AppDomainIsolation()
        {
            var setup = new AppDomainSetup { ShadowCopyFiles = "true" };
            _domain = AppDomain.CreateDomain("AppDomainIsolation:" + Guid.NewGuid(), null, setup);

            var type = typeof(TObject);

            try
            {
                _object = (TObject)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            }
            catch
            {
                _object = (TObject)_domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
            }
        }

        /// <summary>Gets the object.</summary>
        public TObject Object
        {
            get { return _object; }
        }

        /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
        public void Dispose()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);
                _domain = null;
            }
        }
    }
}

#endif