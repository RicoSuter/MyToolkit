using System;
using System.Windows;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{    
    /// <summary>
    /// An application bar icon button with bindable properties. 
    /// </summary>
    public class BindableApplicationBarIconButton : BindableApplicationBarMenuItem, IApplicationBarIconButton
    {
        /// <summary>
        /// Gets or sets the icon URI. 
        /// </summary>
		public Uri IconUri
		{
			get { return (Uri)GetValue(IconUriProperty); }
			set { SetValue(IconUriProperty, value); }
		}

		public static readonly DependencyProperty IconUriProperty =
			DependencyProperty.Register("IconUri", typeof(Uri), typeof(BindableApplicationBarIconButton),
				new PropertyMetadata(default(Uri), (d, e) => ((BindableApplicationBarIconButton)d).IconUriChanged((Uri)e.NewValue)));

		private void IconUriChanged(Uri iconUri)
		{
            if (InternalItem != null)
    			((IApplicationBarIconButton)InternalItem).IconUri = iconUri;
		}
    }
}
