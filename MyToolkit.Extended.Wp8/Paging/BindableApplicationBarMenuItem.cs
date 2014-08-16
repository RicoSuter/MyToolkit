using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Shell;

namespace MyToolkit.Paging
{
    /// <summary>
    /// An application bar menu item with bindable properties. 
    /// </summary>
    public class BindableApplicationBarMenuItem : FrameworkElement, IApplicationBarMenuItem
    {
		private readonly IApplicationBarMenuItem _internalItem;
        private BindableApplicationBar _applicationBar;
        private bool _isAttached;

		public BindableApplicationBarMenuItem()
		{
            if (this is BindableApplicationBarIconButton)
                _internalItem = new ApplicationBarIconButton { Text = "-" };
            else
                _internalItem = new ApplicationBarMenuItem { Text = "-" };
            
			_internalItem.Click += OnItemClick;
		}

        #region Dependency properties

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(BindableApplicationBarMenuItem),
            new PropertyMetadata(true, (d, e) => ((BindableApplicationBarMenuItem)d).IsVisibleChanged()));

        /// <summary>
        /// Gets or sets a value indicating whether the item is visible in the application bar. 
        /// </summary>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        private void IsVisibleChanged()
        {
            if (IsVisible)
            {
                if (this is BindableApplicationBarIconButton)
                {
                    if (!_applicationBar.InternalApplicationBar.Buttons.Contains(_internalItem))
                        _applicationBar.InternalApplicationBar.Buttons.Insert(Position, _internalItem);
                }
                else
                {
                    if (!_applicationBar.InternalApplicationBar.MenuItems.Contains(_internalItem))
                        _applicationBar.InternalApplicationBar.MenuItems.Insert(Position, _internalItem);
                }
            }
            else
            {
                if (this is BindableApplicationBarIconButton)
                {
                    if (_applicationBar.InternalApplicationBar.Buttons.Contains(_internalItem))
                        _applicationBar.InternalApplicationBar.Buttons.Remove(_internalItem);
                }
                else
                {
                    if (_applicationBar.InternalApplicationBar.MenuItems.Contains(_internalItem))
                        _applicationBar.InternalApplicationBar.MenuItems.Remove(_internalItem);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the item is enabled. 
        /// </summary>
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

        /// <summary>
        /// Gets or sets the item text. 
        /// </summary>
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

        /// <summary>
        /// Gets or sets the command (CanExecute updates the IsEnabled property). 
        /// </summary>
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
            IsEnabled = ((ICommand)sender).CanExecute(CommandParameter);
        }

        /// <summary>
        /// Gets or sets the command parameter. 
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(BindableApplicationBarMenuItem),
                new PropertyMetadata(null));


        #endregion

        /// <summary>
        /// Occurs when the user taps the menu item.
        /// </summary>
		public event EventHandler Click
		{
			add { _internalItem.Click += value; }
			remove { _internalItem.Click -= value; }
		}

        internal IApplicationBarMenuItem InternalItem
        {
            get { return _internalItem; }
        }

        internal void AddItem(BindableApplicationBar applicationBar)
        {
            if (!_isAttached)
            {
                _applicationBar = applicationBar;

                if (this is BindableApplicationBarIconButton)
                    _applicationBar.InternalApplicationBar.Buttons.Add(InternalItem);
                else
                    _applicationBar.InternalApplicationBar.MenuItems.Add(InternalItem);

                if (Command != null)
                    IsEnabled = Command.CanExecute(CommandParameter);

                _isAttached = true;
            }
        }

        internal void RemoveItem()
        {
            if (_isAttached)
            {
                if (this is BindableApplicationBarIconButton)
                    _applicationBar.InternalApplicationBar.Buttons.Remove(InternalItem);
                else
                    _applicationBar.InternalApplicationBar.MenuItems.Remove(InternalItem);

                _isAttached = false;
            }
        }

        private int Position
        {
            get
            {
                if (this is BindableApplicationBarIconButton)
                    return _applicationBar
                        .Buttons
                        .Where(i => i.IsVisible || i == this)
                        .ToList()
                        .IndexOf((BindableApplicationBarIconButton)this);

                return _applicationBar
                    .MenuItems
                    .Where(i => i.IsVisible || i == this)
                    .ToList()
                    .IndexOf(this);
            }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            if (Command != null)
            {
                var canExecute = Command.CanExecute(CommandParameter);
                IsEnabled = canExecute;
                if (canExecute)
                    Command.Execute(CommandParameter);
            }
        }
    }
}
