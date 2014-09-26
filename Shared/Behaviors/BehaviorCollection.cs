using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Behaviors
{
    public class BehaviorCollection : ObservableCollection<Behavior>
    {
        protected DependencyObject Element { get; private set; }

        public void Attach(DependencyObject dependencyObject)
        {
            if (Element != null)
                throw new InvalidOperationException("The BehaviorCollection is already attached to a different object.");

            Element = dependencyObject;

            foreach (var behavior in this)
                behavior.Attach(dependencyObject);

            CollectionChanged += OnCollectionChanged;
            OnAttached();
        }

        public void Detach()
        {
            OnDetaching();

            foreach (var behavior in this)
                behavior.Detach();

            CollectionChanged -= OnCollectionChanged;
            Element = null;
        }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetaching()
        {
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Behavior behavior in e.NewItems)
                        behavior.Attach(Element);
                    break;
                    
                case NotifyCollectionChangedAction.Reset: // TODO: This may be a problem
                case NotifyCollectionChangedAction.Remove:
                    foreach (Behavior behavior in e.OldItems)
                        behavior.Detach();
                    break;
            }
        }
    }
}
