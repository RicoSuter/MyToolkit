//-----------------------------------------------------------------------
// <copyright file="AppBarButton.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    public class AppBarButton : Button
    {
        public AppBarButton()
        {
            DefaultStyleKey = typeof(AppBarButton);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(String), typeof(AppBarButton), new PropertyMetadata(default(String)));

        public String Header
        {
            get { return (String)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}

#endif