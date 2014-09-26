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
    public abstract class Behavior : FrameworkElement
    {
        protected internal DependencyObject _associatedObject;
        protected internal Type _associatedType = typeof(object);

        protected DependencyObject AssociatedObject
        {
            get { return _associatedObject; }
        }

        protected Type AssociatedType
        {
            get { return _associatedType; }
        }

        public void Attach(DependencyObject dependencyObject)
        {
            if (AssociatedObject != null)
                throw new InvalidOperationException("The Behavior is already hosted on a different element.");

            _associatedObject = dependencyObject;
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

                _associatedObject = null;
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
                Source = _associatedObject
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
