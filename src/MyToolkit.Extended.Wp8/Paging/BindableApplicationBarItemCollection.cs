using System;
using System.Linq;
using System.Windows;
using System.Collections.Specialized;

namespace MyToolkit.Paging
{
	public abstract class BindableApplicationBarItemCollection<T> : DependencyObjectCollection<T>
		where T : BindableApplicationBarMenuItem
	{
        protected BindableApplicationBar ApplicationBar { get; private set; }

        protected BindableApplicationBarItemCollection(BindableApplicationBar applicationBar)
		{
			ApplicationBar = applicationBar; 
			CollectionChanged += OnCollectionChanged;
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.NewItems != null)
			{
				foreach (var item in args.NewItems.OfType<BindableApplicationBarMenuItem>())
					item.AddItem(ApplicationBar);
			}

            if (args.OldItems != null)
            {
                foreach (var item in args.OldItems.OfType<BindableApplicationBarMenuItem>())
                    item.RemoveItem();
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
        public BindableApplicationBarMenuItemCollection(BindableApplicationBar applicationBar) 
			: base(applicationBar) { }
    }

    public class BindableApplicationBarIconButtonCollection : BindableApplicationBarItemCollection<BindableApplicationBarIconButton>
    {
    	public BindableApplicationBarIconButtonCollection(BindableApplicationBar applicationBar) 
			: base(applicationBar) { }
    }
}
