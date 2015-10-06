//-----------------------------------------------------------------------
// <copyright file="PartDescription.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;

namespace MyToolkit.Composition
{
    internal class PartDescription
    {
        private readonly object _lock = new object();
        private object _part;

        /// <summary>Initializes a new instance of the <see cref="PartDescription" /> class.</summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        /// <param name="perRequest">If set to <c>true</c> the part is created for each request.</param>
        public PartDescription(Type interfaceType, Type implementationType, string name, bool perRequest)
        {
            Key = interfaceType;
            Name = name;
            Type = implementationType;
            PerRequest = perRequest;
        }

        /// <summary>Gets the key.</summary>
        public Type Key { get; private set; }

        /// <summary>Gets the name.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the type.</summary>
        public Type Type { get; private set; }

        /// <summary>Gets a value indicating whether the object is created per request.</summary>
        public bool PerRequest { get; private set; }

        /// <summary>Sets the part.</summary>
        /// <param name="ctx">The context.</param>
        /// <param name="part">The part.</param>
        public void SetPart(CompositionContext ctx, object part)
        {
            ctx.SatisfyImports(_part);
            lock (_lock)
                _part = part;
        }

        /// <summary>Gets the part.</summary>
        /// <param name="ctx">The context.</param>
        /// <returns>The part. </returns>
        public object GetPart(CompositionContext ctx)
        {
            if (PerRequest)
                return CreatePart(ctx);

            if (_part == null)
            {
                lock (_lock)
                {
                    if (_part == null)
                    {
                        var part = CreatePart(ctx);
                        SetPart(ctx, part);
                    }
                }
            }
            return _part;
        }

        private object CreatePart(CompositionContext ctx)
        {
#if !LEGACY
            var constructor = Type
                .GetTypeInfo()
                .DeclaredConstructors
                .First();
#else
            var constructor = Type
                .GetConstructors()
                .First();
#endif

            var parameters = constructor
                .GetParameters()
                .Select(p => ctx.GetPart(p.ParameterType, null))
                .ToArray();

            var part = constructor.Invoke(parameters);
            return part;
        }
    }
}