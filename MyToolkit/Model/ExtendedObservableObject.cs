//-----------------------------------------------------------------------
// <copyright file="ExtendedObservableObject.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using MyToolkit.Collections;
using MyToolkit.Utilities;

namespace MyToolkit.Model
{
    /// <summary>An <see cref="ObservableObject"/> with graph property changed event and extended 
    /// changed event (including old and new value). </summary>
    public class ExtendedObservableObject : ObservableObject
    {
        private readonly List<object> _registeredChilds = new List<object>(); 
        private readonly Dictionary<object, List<object>> _registeredCollections = new Dictionary<object, List<object>>();
        private readonly List<Type> _excludedChildTypes = new List<Type>(); 

        private bool _suppressGraphPropertyChanged = false;
        
        /// <summary>Occurs when a property value of the object or any child changes. </summary>
        public event PropertyChangedEventHandler GraphPropertyChanged;

        /// <summary>Gets the child types which are excluded for graph tracking (direct references or in collections).</summary>
        protected List<Type> ExcludedChildTypes
        {
            get { return _excludedChildTypes; }
        }

        /// <summary>Updates the property and raises the changed event, but only if the new value does not equal the old value. </summary>
        /// <param name="propertyName">The property name as lambda. </param>
        /// <param name="oldValue">A reference to the backing field of the property. </param>
        /// <param name="newValue">The new value. </param>
        /// <returns>True if the property has changed. </returns>
        public override bool Set<T>(string propertyName, ref T oldValue, T newValue)
        {
            if (Equals(oldValue, newValue))
                return false;

            UnregisterChild(oldValue);
            RegisterChild(newValue);
            
            var args = new ExtendedPropertyChangedEventArgs(propertyName, oldValue, newValue);

            oldValue = newValue;
            RaisePropertyChanged(args);
            return true;
        }

        /// <summary>Raises the property changed event with <see cref="ExtendedPropertyChangedEventArgs"/> arguments. </summary>
        /// <param name="oldValue">The old value. </param>
        /// <param name="newValue">The new value. </param>
        /// <param name="propertyName">The property name. </param>
        public void RaisePropertyChanged(object oldValue, object newValue, [CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(new ExtendedPropertyChangedEventArgs(propertyName, oldValue, newValue));
        }

        /// <summary>Raises the property changed event with <see cref="ExtendedPropertyChangedEventArgs"/> arguments. </summary>
        /// <param name="oldValue">The old value. </param>
        /// <param name="newValue">The new value. </param>
        /// <param name="propertyNameExpression">The property name as lambda. </param>
        public void RaisePropertyChanged(Expression<Func<object>> propertyNameExpression, object oldValue, object newValue)
        {
            RaisePropertyChanged(new ExtendedPropertyChangedEventArgs(ExpressionHelper.GetName(propertyNameExpression), oldValue, newValue));
        }

        /// <summary>Raises the property changed event with <see cref="ExtendedPropertyChangedEventArgs"/> arguments. </summary>
        /// <param name="oldValue">The old value. </param>
        /// <param name="newValue">The new value. </param>
        /// <typeparam name="TClass">The type of the class with the property. </typeparam>
        /// <param name="propertyNameExpression">The property name as lambda. </param>
        public void RaisePropertyChanged<TClass>(Expression<Func<TClass, object>> propertyNameExpression, object oldValue, object newValue)
        {
            RaisePropertyChanged(new ExtendedPropertyChangedEventArgs(ExpressionHelper.GetName(propertyNameExpression), oldValue, newValue));
        }

        /// <summary>Registers a child to receive property changes. </summary>
        /// <param name="child">The child object. </param>
        protected void RegisterChild(object child)
        {
            if (child == null)
                return;

#if LEGACY
            var childTypeInfo = child.GetType();
            if (_registeredChilds.Contains(child) || ExcludedChildTypes.Any(t => t.IsAssignableFrom(childTypeInfo)))
                return;
#else
            var childTypeInfo = child.GetType().GetTypeInfo();
            if (_registeredChilds.Contains(child) || ExcludedChildTypes.Any(t => t.GetTypeInfo().IsAssignableFrom(childTypeInfo)))
                return;
#endif

            if (child is ExtendedObservableObject)
            {
                ((ExtendedObservableObject)child).GraphPropertyChanged += RaiseGraphPropertyChanged;
                _registeredChilds.Add(child);
            }
            else if (child is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)child).PropertyChanged += RaiseGraphPropertyChanged;
                _registeredChilds.Add(child);

                if (child is ICollection)
                {
                    var list = child as ICollection;
                    foreach (var item in list)
                        RegisterChild(item);

                    if (child is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)child).CollectionChanged += OnCollectionChanged;
                        _registeredCollections.Add(child, list.OfType<object>().ToList());
                    }
                }
            }
            else if (child.GetType().Name == "KeyValuePair`2")
            {
                // TODO: [PERF] add cache
#if LEGACY
                var value = child.GetType().GetProperty("Value").GetValue(child, null); 
#else
                var value = child.GetType().GetRuntimeProperty("Value").GetValue(child); 
#endif
                RegisterChild(value);
            }
        }

        /// <summary>Unregisters a child. </summary>
        /// <param name="child">The child object. </param>
        protected void UnregisterChild(object child)
        {
            if (child == null || !_registeredChilds.Contains(child))
                return;

            if (child is ExtendedObservableObject)
            {
                ((ExtendedObservableObject)child).GraphPropertyChanged -= RaiseGraphPropertyChanged;
                _registeredChilds.Remove(child);
            }
            else if (child is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)child).PropertyChanged -= RaiseGraphPropertyChanged;
                _registeredChilds.Remove(child);

                if (child is ICollection)
                {
                    var list = child as ICollection;
                    foreach (var item in list)
                        UnregisterChild(item);

                    if (child is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)child).CollectionChanged -= OnCollectionChanged;
                        _registeredCollections.Remove(child);
                    }
                }
            }
            else if (child.GetType().Name == "KeyValuePair`2")
            {
                // TODO: [PERF] add cache
#if LEGACY
                var value = child.GetType().GetProperty("Value").GetValue(child, null); 
#else
                var value = child.GetType().GetRuntimeProperty("Value").GetValue(child); 
#endif
                UnregisterChild(value);
            }
        }

        /// <summary>Raises the property changed event. </summary>
        /// <param name="args">The arguments. </param>
        protected override void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            base.RaisePropertyChanged(args);
            RaiseGraphPropertyChanged(this, args);
        }

        private void RaiseGraphPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (_suppressGraphPropertyChanged)
                return;

            _suppressGraphPropertyChanged = true; // used to avoid multiple calls in cyclic graphs

            var copy = GraphPropertyChanged;
            if (copy != null)
                copy(sender, args);

            _suppressGraphPropertyChanged = false; 
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var oldCollection = _registeredCollections[sender];
            var oldCollectionCopy = oldCollection.ToList();

            var addedItems = new List<object>();
            foreach (var item in ((ICollection)sender).OfType<object>().Where(i => !oldCollection.Contains(i))) // new items
            {
                addedItems.Add(item);
                oldCollection.Add(item);
                RegisterChild(item);
            }

            var removedItems = new List<object>();
            var currentItems = ((ICollection) sender).OfType<object>().ToArray();
            foreach (var item in oldCollection.Where(i => !currentItems.Contains(i)).ToArray()) // deleted items
            {
                removedItems.Add(item);
                oldCollection.Remove(item);
                UnregisterChild(item);
            }

            RaiseGraphPropertyChanged(sender, new ExtendedNotifyCollectionChangedEventArgs<object>(addedItems, removedItems, oldCollectionCopy));
        }
    }
}
