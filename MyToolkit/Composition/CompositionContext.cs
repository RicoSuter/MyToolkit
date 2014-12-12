//-----------------------------------------------------------------------
// <copyright file="CompositionContext.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
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
    /// <summary>Provides the ability to store, retrieve and assemble parts. </summary>
    public class CompositionContext : ICompositionContext
    {
        private readonly List<PartDescription> _parts;
        private Dictionary<Type, Dictionary<PropertyInfo, Attribute>> _typeMappingCache = null;

        /// <summary>Initializes a new instance of the <see cref="CompositionContext"/> class. </summary>
        public CompositionContext()
        {
            _parts = new List<PartDescription>();
        }

        /// <summary>Adds parts from a given assembly. </summary>
        /// <param name="assembly">The assembly. </param>
        /// <returns>The number of added parts. </returns>
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

        /// <summary>Adds a part and automatically resolves are imports. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <typeparam name="TImplementation">The instance type of the part. </typeparam>
        /// <param name="part">The part. </param>
        /// <returns>True if the part has been added. </returns>
        public bool AddPart<TInterface, TImplementation>(TImplementation part)
        {
            return AddPart(typeof(TInterface), typeof(TImplementation), null, part);
        }

        /// <summary>Adds a part and automatically resolves are imports. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <typeparam name="TImplementation">The instance type of the part. </typeparam>
        /// <param name="name">The name of the part. </param>
        /// <param name="part">The part. </param>
        /// <returns>True if the part has been added. </returns>
        public bool AddPart<TInterface, TImplementation>(TImplementation part, string name)
        {
            return AddPart(typeof(TInterface), part.GetType(), name, part);
        }

        /// <summary>Adds a lazy instantiated part. The imports are resolved when the part is instantiated. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <typeparam name="TImplementation">The instance type of the part. </typeparam>
        /// <returns>True if the part has been added. </returns>
        public bool AddPart<TInterface, TImplementation>()
        {
            return AddPart(typeof(TInterface), typeof(TImplementation), null, null);
        }

        /// <summary>Adds a lazy instantiated part. The imports are resolved when the part is instantiated. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <typeparam name="TImplementation">The instance type of the part. </typeparam>
        /// <param name="name">The name of the part. </param>
        /// <returns>True if the part has been added. </returns>
        public bool AddPart<TInterface, TImplementation>(string name)
        {
            return AddPart(typeof(TInterface), typeof(TImplementation), name, null);
        }

        /// <summary>Adds a part and automatically resolves are imports. </summary>
        /// <param name="interfaceType">The type of the part. </param>
        /// <param name="name">The name of the part. </param>
        /// <param name="part"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        protected bool AddPart(Type interfaceType, Type implementationType, string name, object part)
        {
            RemovePart(implementationType, name);

            var p = new PartDescription
            {
                Key = interfaceType,
                Type = implementationType,
                Name = name,
            };

            p.SetPart(this, part);

            _parts.Add(p);
            return true;
        }

        /// <summary>Removes a part. </summary>
        /// <typeparam name="TInterface">The type of the part. </typeparam>
        public void RemovePart<TInterface>()
        {
            RemovePart(typeof(TInterface), null);
        }

        /// <summary>Removes a part. </summary>
        /// <typeparam name="TInterface">The type of the part. </typeparam>
        /// <param name="name">The name of the part. </param>
        public void RemovePart<TInterface>(string name)
        {
            RemovePart(typeof(TInterface), name);
        }

        /// <summary>Removes a part. </summary>
        /// <param name="interfaceType">The interface type of the part. </param>
        /// <param name="name">The name of the part. </param>
        protected void RemovePart(Type interfaceType, string name)
        {
            var oldPart = _parts.SingleOrDefault(p => p.Type == interfaceType && p.Name == name);
            if (oldPart != null)
                _parts.Remove(oldPart);
        }

        /// <summary>Gets a single part. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <exception cref="Exception">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        public TInterface GetPart<TInterface>()
        {
            return (TInterface)GetPart(typeof(TInterface), null);
        }

        /// <summary>Gets a single part. </summary>
        /// <typeparam name="TInterface">The interface type of the part. </typeparam>
        /// <param name="name">The name of the part. </param>
        /// <exception cref="Exception">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        public TInterface GetPart<TInterface>(string name)
        {
            return (TInterface)GetPart(typeof(TInterface), name);
        }

        /// <summary>Gets a single part. </summary>
        /// <param name="interfaceType">The type of the part. </param>
        /// <param name="name">The name of the part. </param>
        /// <exception cref="Exception">Multiple parts found. </exception>
        /// <returns>The part. </returns>
        internal protected object GetPart(Type interfaceType, string name)
        {
            var part = _parts.SingleOrDefault(p => p.Key == interfaceType && p.Name == name);
            if (part != null)
                return part.GetPart(this);
            return null;
        }

        /// <summary>Gets multiple parts of a given type. </summary>
        /// <param name="interfaceType">The interface type of the part. </param>
        /// <returns>The list of parts. </returns>
        public IList GetParts(Type interfaceType)
        {
            var listType = typeof(List<>);
            listType = listType.MakeGenericType(interfaceType);

            var list = (IList)Activator.CreateInstance(listType);
            foreach (var part in _parts.Where(p => p.Key == interfaceType).Select(p => p.GetPart(this)))
                list.Add(part);

            return list;
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
                if (pair.Value is ImportAttribute)
                {
                    var attribute = (ImportAttribute)pair.Value;
                    var part = GetPart(attribute.Type, attribute.Name);
                    if (part != null)
                        property.SetValue(obj, part, null);
                    else
                        Debug.WriteLine("CompositionContext (Import): Part for property = " + property.Name + " not found!");
                }
                else if (pair.Value is ImportManyAttribute)
                {
                    var manyAttribute = (ImportManyAttribute)pair.Value;
                    var parts = GetParts(manyAttribute.Type);
                    if (parts != null)
                        property.SetValue(obj, parts, null);
                    else
                        Debug.WriteLine("CompositionContext (ImportMany): Part for property = " + property.Name + " not found!");
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

    internal class PartDescription
    {
        private object _part;

        public Type Key;
        public string Name;
        public Type Type;

        public void SetPart(CompositionContext ctx, object part)
        {
            ctx.SatisfyImports(_part);

            lock (this)
                _part = part;
        }

        public object GetPart(CompositionContext ctx)
        {
            if (_part == null)
            {
                lock (this)
                {
                    if (_part == null)
                    {
#if !LEGACY
                        var ctor = Type
                            .GetTypeInfo()
                            .DeclaredConstructors
                            .First();
#else
                        var ctor = Type
                            .GetConstructors()
                            .First();
#endif

                        var arguments = ctor.GetParameters()
                            .Select(argument => ctx.GetPart(argument.ParameterType, null))
                            .ToArray();

                        SetPart(ctx, ctor.Invoke(arguments));
                    }
                }
            }

            return _part;
        }
    }
}