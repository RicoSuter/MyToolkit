//-----------------------------------------------------------------------
// <copyright file="Behavior.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace MyToolkit.Behaviors
{
    /// <summary>A typed XAML behavior. </summary>
    /// <typeparam name="T">The type of the attached object. </typeparam>
    public abstract class Behavior<T> : Behavior where T : DependencyObject
    {
        protected Behavior()
        {
            AssociatedType = typeof(T);
        }

        public new T Element
        {
            get { return (T)AssociatedObject; }
            internal set { AssociatedObject = value; }
        }
    }

    /// <summary>A XAML behavior. </summary>
    /// <typeparam name="T">The type of the attached object. </typeparam>
    public abstract class Behavior : FrameworkElement
    {
        protected Behavior()
        {
            AssociatedType = typeof(object);
        }

        /// <summary>Gets or sets the associated object. </summary>
        protected internal DependencyObject AssociatedObject { get; set; }

        /// <summary>Gets or sets the type of the associated object. </summary>
        protected internal Type AssociatedType { get; set; }

        public void Attach(DependencyObject dependencyObject)
        {
            if (AssociatedObject != null)
                throw new InvalidOperationException("The Behavior is already hosted on a different element.");

            AssociatedObject = dependencyObject;
            if (dependencyObject != null)
            {
#if SL5 || WP7
                if (!AssociatedType.IsInstanceOfType(dependencyObject))
                    throw new InvalidOperationException("dependencyObject does not satisfy the Behavior type constraint.");
#else
                if (!AssociatedType.GetTypeInfo().IsAssignableFrom(dependencyObject.GetType().GetTypeInfo()))
                    throw new InvalidOperationException("dependencyObject does not satisfy the Behavior type constraint.");
#endif

                var frameworkElement = AssociatedObject as FrameworkElement;
                if (frameworkElement != null)
                {
                    frameworkElement.Loaded += AssociatedObjectLoaded;
                    frameworkElement.Unloaded += AssociatedObjectUnloaded;
                }

                OnAttached();
            }
        }

        public void Detach()
        {
            if (AssociatedObject != null)
            {
                OnDetaching();

                var frameworkElement = AssociatedObject as FrameworkElement;
                if (frameworkElement != null)
                {
                    frameworkElement.Loaded -= AssociatedObjectLoaded;
                    frameworkElement.Unloaded -= AssociatedObjectUnloaded;
                }

                AssociatedObject = null;
            }
        }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        protected virtual void OnLoaded()
        {
        }
        
        protected virtual void OnUnloaded()
        {
        }
        
        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            var binding = new Binding
            {
                Path = new PropertyPath("DataContext"),
                Source = AssociatedObject
            };

            SetBinding(DataContextProperty, binding);
            OnLoaded();
        }

        private void AssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            OnUnloaded();
            ClearValue(DataContextProperty);
            DataContext = null;
        }
    }
}
