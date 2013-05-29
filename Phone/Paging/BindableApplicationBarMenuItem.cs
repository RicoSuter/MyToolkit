using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{
    public class BindableApplicationBarMenuItem : FrameworkElement, IApplicationBarMenuItem
    {
		private readonly IApplicationBarMenuItem internalItem;
		private bool isAttached;

		public BindableApplicationBarMenuItem()
		{
			internalItem = Create();
			internalItem.Click += OnItemClick;
		}

		protected virtual IApplicationBarMenuItem Create()
		{
			return new ApplicationBarMenuItem { Text = "-" };
		}

		protected virtual void OnItemAdded(IApplicationBar applicationBar)
		{
			applicationBar.MenuItems.Add(InternalItem);
		}

		protected virtual void OnItemRemoved(IApplicationBar applicationBar)
		{
			applicationBar.MenuItems.Remove(InternalItem);
		}

		internal void RemoveItem(IApplicationBar internalApplicationBar)
		{
			if (isAttached)
			{
				OnItemRemoved(internalApplicationBar);
				isAttached = false;
			}
		}

		internal void AddItem(IApplicationBar internalApplicationBar)
		{
			if (!isAttached)
			{
				OnItemAdded(internalApplicationBar);
				isAttached = true;
				//if (Command != null)
				//	IsEnabled = Command.CanExecute(CommandParameter);
			}
		}

		private void OnItemClick(object sender, EventArgs e)
		{
			if (Command != null)
			{
				var canExecute = Command.CanExecute(CommandParameter);
				//IsEnabled = canExecute;

				if (canExecute)
					Command.Execute(CommandParameter);
			}
		}




		internal bool IsAttached
		{
			get { return isAttached; }
		}

		protected IApplicationBarMenuItem InternalItem
		{
			get { return internalItem; }
		}



		public event EventHandler Click
		{
			add { internalItem.Click += value; }
			remove { internalItem.Click -= value; }
		}



		public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

		public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(BindableApplicationBarMenuItem),
                new PropertyMetadata(true, (d, e) => ((BindableApplicationBarMenuItem)d).IsEnabledChanged((bool)e.NewValue)));

        private void IsEnabledChanged(bool isEnabled)
        {
            InternalItem.IsEnabled = isEnabled;
        }

		public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BindableApplicationBarMenuItem),
                new PropertyMetadata(default(string), (d, e) => ((BindableApplicationBarMenuItem)d).TextChanged((string)e.NewValue)));

        private void TextChanged(string text)
        {
            InternalItem.Text = text;
        }

		public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(BindableApplicationBarMenuItem),
                new PropertyMetadata(default(ICommand), (d, e) => ((BindableApplicationBarMenuItem)d).CommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue)));

        private void CommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
                oldCommand.CanExecuteChanged -= OnCanExecuteChanged;
            if (newCommand != null)
                newCommand.CanExecuteChanged += OnCanExecuteChanged;                
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
			// TODO overwrites binding!!
			//IsEnabled = ((ICommand)sender).CanExecute(CommandParameter);
        }

		public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(BindableApplicationBarMenuItem),
                new PropertyMetadata(null));
    }
}
