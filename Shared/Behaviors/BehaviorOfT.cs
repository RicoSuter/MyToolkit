#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Behaviors
{
    public abstract class Behavior<T> : Behavior where T : DependencyObject
    {
        protected Behavior()
        {
            _associatedType = typeof(T);
        }

        public new T Element
        {
            get { return (T)_associatedObject; }
            internal set { _associatedObject = value; }
        }
    }
}
