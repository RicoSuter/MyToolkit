//-----------------------------------------------------------------------
// <copyright file="MtPivotItem.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Controls
{
    [ContentProperty(Name = "Content")]
    public class MtPivotItem : FrameworkElement
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(MtPivotItem), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(MtPivotItem), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty PreloadProperty =
            DependencyProperty.Register("Preload", typeof(bool), typeof(MtPivotItem), new PropertyMetadata(true));

        public bool Preload
        {
            get { return (bool)GetValue(PreloadProperty); }
            set { SetValue(PreloadProperty, value); }
        }
    }

    [Obsolete("Use MtPivotItem instead. 8/31/2014")]
    public class PivotItem : MtPivotItem
    {
    }
}

#endif