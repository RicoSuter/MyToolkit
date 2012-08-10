using System;
using System.Windows;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{    
    public class BindableApplicationBarIconButton : BindableApplicationBarMenuItem, IApplicationBarIconButton
    {
		protected override IApplicationBarMenuItem Create()
		{
			return new ApplicationBarIconButton { Text = "-" };
		}

		protected override void OnItemAdded(IApplicationBar applicationBar)
        {
            applicationBar.Buttons.Add(InternalItem);
        }

		protected override void OnItemRemoved(IApplicationBar applicationBar)
        {
			applicationBar.Buttons.Remove(InternalItem);
        }



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
			((IApplicationBarIconButton)InternalItem).IconUri = iconUri;
		}
    }
}
