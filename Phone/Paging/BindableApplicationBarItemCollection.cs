using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{
	public abstract class BindableApplicationBarItemCollection<T> : DependencyObjectCollection<T>
		where T : BindableApplicationBarMenuItem
	{
		protected IApplicationBar ApplicationBar { get; private set; }

		protected BindableApplicationBarItemCollection(IApplicationBar applicationBar)
		{
			ApplicationBar = applicationBar; 
			CollectionChanged += OnCollectionChanged;
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.OldItems != null)
			{
				foreach (var item in args.OldItems.OfType<BindableApplicationBarMenuItem>())
					item.RemoveItem(ApplicationBar);
			}

			if (args.NewItems != null)
			{
				foreach (var item in args.NewItems.OfType<BindableApplicationBarMenuItem>())
					item.AddItem(ApplicationBar, this);
			}
		}

		internal void UpdateDataContext(object dataContext)
		{
			foreach (var item in this)
				item.DataContext = dataContext;
		}
	}

	public class BindableApplicationBarMenuItemCollection : BindableApplicationBarItemCollection<BindableApplicationBarMenuItem>
    {
		public BindableApplicationBarMenuItemCollection(IApplicationBar applicationBar) 
			: base(applicationBar) { }
    }

    public class BindableApplicationBarIconButtonCollection : BindableApplicationBarItemCollection<BindableApplicationBarIconButton>
    {
    	public BindableApplicationBarIconButtonCollection(IApplicationBar applicationBar) 
			: base(applicationBar) { }
    }
}
