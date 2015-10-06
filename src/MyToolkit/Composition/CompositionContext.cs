//-----------------------------------------------------------------------
// <copyright file="CompositionContext.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MyToolkit.Composition
{
    /// <summary>Provides the ability to store, retrieve and assemble parts.</summary>
    public class CompositionContext : ICompositionContext
    {
        private readonly List<PartDescription> _parts;
        private Dictionary<Type, Dictionary<PropertyInfo, Attribute>> _typeMappingCache;

        /// <summary>Initializes a new instance of the <see cref="CompositionContext" /> class.</summary>
        public CompositionContext()
        {
            _parts = new List<PartDescription>();
        }

        /// <summary>Adds parts from a given assembly.</summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The number of added parts.</returns>
        public int AddPartsFromAssembly(Assembly assembly)
        {
#if !LEGACY

            var count = 0;
            foreach (var type in assembly.ExportedTypes)
            {
                var attr = (ExportAttribute)type.GetTypeInfo().GetCustomAttributes(typeof(ExportAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    AddPart(attr.Type, type, attr.Name, null);
                    count++;
                }
            }
            return count;

#else

            var count = 0;
            foreach (var type in assembly.GetExportedTypes())
            {
                var attr = (ExportAttribute)type.GetCustomAttributes(typeof(ExportAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    AddPart(attr.Type, type, attr.Name, null);
                    count++;
                }
            }
            return count;

#endif
        }

        /// <summary>Adds an existing part for a given interface and implementation type.</summary>
        /// <typeparam name="TInterface">The interface type of the part.</typeparam>
        /// <typeparam name="TImplementation">The instance type of the part.</typeparam>
        /// <param name="part">The part.</param>
        /// <returns><c>true</c> if the part has been added.</returns>
        public bool AddPart<TInterface, TImplementation>(TImplementation part)
        {
            return AddPart(typeof(TInterface), typeof(TImplementation), null, part);
        }

        /// <summary>Adds an existing part for a given interface, implementation type and name.</summary>
        /// <typeparam name="TInterface">The interface type of the part.</typeparam>
        /// <typeparam name="TImplementation">The instance type of the part.</typeparam>
        /// <param name="part">The part.</param>
        /// <param name="name">The name of the part.</param>
        /// <returns>True if the part has been added.</returns>
        public bool AddPart<TInterface, TImplementation>(TImplementation part, string name)
        {
            return AddPart(typeof(TInterface), part.GetType(), name, part);
        }

        /// <summary>Adds a part for a given interface and implementation type which is instantiated when first requested.</summary>
        /// <typeparam name="TInterface">The interface type of the part.</typeparam>
        /// <typeparam name="TImplementation">The instance type of the part.</typeparam>
        /// <returns><c>true</c> if the part has been added.</returns>
        public bool AddPart<TInterface, TImplementation>()
        {
            return AddPart(typeof(TInterface), typeof(TImplementation), null, null);
        }

        /// <summary>Adds a part for a given interface, implementation type and name which is instantiated when first requested.</summary>
        /// <typeparam name="TInterface">The interface type of the part.</typeparam>
        /// <typeparam name="TImplementation">The instance type of the part.</typeparam>
        /// <param name="name">The name of the part.</param>
        /// <returns>True if the part has been added.</returns>
        public bool AddPart<TInterface, TImplementation>(string name)
        {
            return AddPart(typeof(TInterface), typeof(TImplementation), name, null);
        }

        /// <summary>Adds a part for a given interface and implementation type which is instantiated for each request.</summary>
        /// <typeparam name="TInterface">The interface type of the part.</typeparam>
        /// <typeparam name="TImplementation">The instance type of the part.</typeparam>
        /// <returns><c>true</c> if the part has been added.</returns>
        public bool AddPerRequestPart<TInterface, TImplementation>()
        {
            return AddPerRequestPart(typeof(TInterface), typeof(TImplementation), null);
        }

        /// <summary>Adds a part for a given interface, implementation type and name which is instantiated for each request.</summary>
        /// <typeparam name="TInterface">The interface type of the part.</typeparam>
        /// <typeparam name="TImplementation">The instance type of the part.</typeparam>
        /// <param name="name">The name of the part.</param>
        /// <returns><c>true</c> if the part has been added.</returns>
        public bool AddPerRequestPart<TInterface, TImplementation>(string name)
        {
            return AddPerRequestPart(typeof(TInterface), typeof(TImplementation), name);
        }

        /// <summary>Adds the per request part.</summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the part has been added.</returns>
        protected bool AddPerRequestPart(Type interfaceType, Type implementationType, string name)
        {
            RemovePart(implementationType, name);

            var description = new PartDescription(interfaceType, implementationType, name, true);
            _parts.Add(description);

            return true;
        }

        /// <summary>Adds a part for a given interface, implementation type and name.</summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="name">The name.</param>
        /// <param name="part">The part.</param>
        /// <returns><c>true</c> if the part has been added.</returns>
        protected bool AddPart(Type interfaceType, Type implementationType, string name, object part)
        {
            RemovePart(implementationType, name);

            var description = new PartDescription(interfaceType, implementationType, name, false);
            if (part != null)
                description.SetPart(this, part);
            _parts.Add(description);

            return true;
        }

        /// <summary>Removes a part. </summary>
        /// <typeparam name="TInterface">The type of the part. </typeparam>
        /// <returns><c>true</c> if the part has been found and removed; otherwise, <c>false</c>. </returns>
        public bool RemovePart<TInterface>()
        {
            return RemovePart(typeof(TInterface), null);
        }

        /// <summary>Removes a part. </summary>
        /// <typeparam name="TInterface">The type of the part. </typeparam>
        /// <param name="name">The name of the part. </param>
        /// <returns><c>true</c> if the part has been found and removed; otherwise, <c>false</c>. </returns>
        public bool RemovePart<TInterface>(string name)
        {
            return RemovePart(typeof(TInterface), name);
        }

        /// <summary>Removes a part. </summary>
        /// <param name="interfaceType">The interface type of the part. </param>
        /// <param name="name">The name of the part. </param>
        /// <returns><c>true</c> if the part has been found and removed; otherwise, <c>false</c>. </returns>
        protected bool RemovePart(Type interfaceType, string name)
        {
            var oldPart = _parts.SingleOrDefault(p => p.Type == interfaceType && p.Name == name);
            if (oldPart != null)
            {
                _parts.Remove(oldPart);
                return true;
            }
            return false;
        }

        /// <summary>Gets a single part. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <exception cref="InvalidOperationException">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        public TInterface GetPart<TInterface>()
        {
            return (TInterface)GetPart(typeof(TInterface), null);
        }

        /// <summary>Gets a single part. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <param name="name">The name of the part. </param>
        /// <exception cref="InvalidOperationException">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        public TInterface GetPart<TInterface>(string name)
        {
            return (TInterface)GetPart(typeof(TInterface), name);
        }

        /// <summary>Gets a single part. </summary>
        /// <param name="interfaceType">The type of the part. </param>
        /// <exception cref="InvalidOperationException">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        public object GetPart(Type interfaceType)
        {
            return GetPart(interfaceType, null);
        }

        /// <summary>Gets a single part. </summary>
        /// <param name="interfaceType">The type of the part. </param>
        /// <param name="name">The name of the part. </param>
        /// <exception cref="InvalidOperationException">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        public object GetPart(Type interfaceType, string name)
        {
            var part = _parts.SingleOrDefault(p => p.Key == interfaceType && p.Name == name);
            if (part != null)
                return part.GetPart(this);
            return null;
        }

        /// <summary>Gets multiple parts of a given type. </summary>
        /// <param name="interfaceType">The interface type of the part. </param>
        /// <returns>The list of parts. </returns>
        public IList<object> GetParts(Type interfaceType)
        {
            var listType = typeof(List<>);
            listType = listType.MakeGenericType(interfaceType);

            var list = (IList)Activator.CreateInstance(listType);
            foreach (var part in _parts.Where(p => p.Key == interfaceType).Select(p => p.GetPart(this)))
                list.Add(part);
            return list.OfType<object>().ToList();
        }

        /// <summary>Gets multiple parts of a given interface type. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <returns>The list of parts. </returns>
        public IList<TInterface> GetParts<TInterface>()
        {
            return _parts
                .Where(p => p.Key == typeof(TInterface))
                .Select(p => (TInterface)p.GetPart(this))
                .ToList();
        }

        /// <summary>Resolves all property injection annotations for the given object. </summary>
        /// <param name="obj">The object. </param>
        public void SatisfyImports<T>(T obj)
        {
            if (_typeMappingCache == null)
                _typeMappingCache = new Dictionary<Type, Dictionary<PropertyInfo, Attribute>>();

            BuildCache<T>();

            foreach (var pair in _typeMappingCache[typeof(T)])
            {
                var property = pair.Key;
                var importAttribute = pair.Value as ImportAttribute;
                if (importAttribute != null)
                {
                    var part = GetPart(importAttribute.Type, importAttribute.Name);
                    if (part != null)
                        property.SetValue(obj, part, null);
                    else
                        Debug.WriteLine("CompositionContext (Import): Part for property = " + property.Name + " not found!");
                }
                else
                {
                    var importManyAttribute = pair.Value as ImportManyAttribute;
                    if (importManyAttribute != null)
                    {
                        var manyAttribute = importManyAttribute;
                        var parts = GetParts(manyAttribute.Type);
                        if (parts != null)
                            property.SetValue(obj, parts, null);
                        else
                            Debug.WriteLine("CompositionContext (ImportMany): Part for property = " + property.Name + " not found!");
                    }
                }
            }
        }

        /// <summary>Builds the resolving cache for the given type. </summary>
        public void BuildCache<T>()
        {
            var type = typeof(T);
            if (!_typeMappingCache.ContainsKey(type))
            {
                var list = new Dictionary<PropertyInfo, Attribute>();
#if !LEGACY
                foreach (var p in type.GetRuntimeProperties())
#else
                foreach (var p in type.GetProperties())
#endif
                {
                    var importAttribute = (ImportAttribute)p.GetCustomAttributes(typeof(ImportAttribute), true).FirstOrDefault();
                    if (importAttribute != null)
                        list[p] = importAttribute;
                    else
                    {
                        var importManyAttribute = (ImportManyAttribute)p.GetCustomAttributes(typeof(ImportManyAttribute), true).FirstOrDefault();
                        if (importManyAttribute != null)
                            list[p] = importManyAttribute;
                    }
                }

                _typeMappingCache[type] = list;
            }
        }

        /// <summary>Removes all parts. </summary>
        public void Clear()
        {
            _parts.Clear();
        }
    }
}