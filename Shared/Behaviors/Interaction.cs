//-----------------------------------------------------------------------
// <copyright file="Interaction.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT
using Windows.ApplicationModel;
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Behaviors
{
    public static class Interaction
    {
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction),
            new PropertyMetadata(new BehaviorCollection(), BehaviorsChanged));

        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            var behaviors = obj.GetValue(BehaviorsProperty) as BehaviorCollection;
            if (behaviors == null)
            {
                behaviors = new BehaviorCollection();
                SetBehaviors(obj, behaviors);
            }

            return behaviors;
        }

        private static void SetBehaviors(DependencyObject obj, BehaviorCollection value)
        {
            obj.SetValue(BehaviorsProperty, value);
        }

        private static void BehaviorsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var associatedObject = sender as DependencyObject;
            if (associatedObject != null)
            {
                var oldList = args.OldValue as BehaviorCollection;
                if (oldList != null)
                    oldList.Detach();

                var newList = args.NewValue as BehaviorCollection;
                if (newList != null)
                    newList.Attach(associatedObject);
            }
        }
    }
}
