using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyToolkit.Environment;

namespace MyToolkit.Paging
{
	/// <summary>
	/// Important: Do not use BackKeyPress event but the AddBackKeyPressHandler or AddBackKeyPressAsyncHandler methods!
	/// </summary>
	public class ExtendedPage : PhoneApplicationPage
	{
		#region OnBackKeyPress extensions

		private readonly List<Func<CancelEventArgs, Task>> _backKeyPressActions = new List<Func<CancelEventArgs, Task>>();

		public Func<CancelEventArgs, Task> AddBackKeyPressHandler(Action<CancelEventArgs> action)
		{
			var func = new Func<CancelEventArgs, Task>(args =>
			{
				action(args);
				return null;
			});

			AddBackKeyPressAsyncHandler(func);
			return func;
		}

		public void AddBackKeyPressAsyncHandler(Func<CancelEventArgs, Task> action)
		{
			_backKeyPressActions.Add(action);
		}

		public void RemoveBackKeyPressAsyncHandler(Func<CancelEventArgs, Task> action)
		{
			_backKeyPressActions.Remove(action);
		}

		protected sealed override void OnBackKeyPress(CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			base.OnBackKeyPress(e);
			CallBackKeyPressActions(e, _backKeyPressActions);
		}

		private void CallBackKeyPressActions(CancelEventArgs e, List<Func<CancelEventArgs, Task>> actions)
		{
			Func<CancelEventArgs, Task> lastAction = null; 
			var copy = new CancelEventArgs();
			foreach (var action in actions)
			{
				lastAction = action;

				var task = action(copy);
				if (task != null && !task.IsCompleted)
				{
					e.Cancel = true;
					task.ContinueWith(t => { CheckNextBackKeyPressActions(actions, action, copy, true); });
					return;
				}

				if (copy.Cancel)
					break;
			}
			e.Cancel = copy.Cancel;
			CheckNextBackKeyPressActions(actions, lastAction, copy, false);
		}

		private void CheckNextBackKeyPressActions(List<Func<CancelEventArgs, Task>> actions, Func<CancelEventArgs, Task> action, CancelEventArgs copy, bool perform)
		{
			if (!copy.Cancel)
			{
				var nextActions = actions.Skip(actions.IndexOf(action) + 1).ToList();
				if (nextActions.Count == 0)
					PerformBackKeyPress(perform);
				else
				{
					CallBackKeyPressActions(copy, nextActions);
					if (!copy.Cancel)
						PerformBackKeyPress(perform);
				}
			}
		}

		private void PerformBackKeyPress(bool perform)
		{
			if (perform)
				Dispatcher.BeginInvoke(() => NavigationService.GoBack());
		}

		#endregion

		#region OnNavigatingFrom extensions

		private bool _ignoreNavigatingEvent = false;

		protected virtual Task OnNavigatingFromAsync(NavigatingCancelEventArgs e)
		{
			return null; 
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			PhoneApplication.IsNavigating = true; 

			base.OnNavigatingFrom(e);
			if (_ignoreNavigatingEvent)
			{
				_ignoreNavigatingEvent = false;
				return;
			}

			var copy = new NavigatingCancelEventArgs(e.Uri, e.NavigationMode, e.IsCancelable, e.IsNavigationInitiator);
			var task = OnNavigatingFromAsync(copy);
			if (task != null)
			{
				if (task.IsCompleted)
					e.Cancel = copy.Cancel;
				else
				{
					e.Cancel = true;
					task.ContinueWith(t =>
					{
						if (!copy.Cancel)
							PerformNavigatingFrom(copy);
					});
				}
			}
		}

		private void PerformNavigatingFrom(NavigatingCancelEventArgs args)
		{
			Dispatcher.BeginInvoke(() =>
			{
				_ignoreNavigatingEvent = true; 
				if (args.NavigationMode == NavigationMode.Back)
					NavigationService.GoBack();
				else if (args.NavigationMode == NavigationMode.Forward)
					NavigationService.GoForward();
				else if (args.NavigationMode == NavigationMode.New)
					NavigationService.Navigate(args.Uri);
			});
		}

		#endregion

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			PhoneApplication.IsNavigating = false;
		}

		private bool _isNewPage = true;
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			OnNavigatedTo(e, _isNewPage);
			_isNewPage = false; 
			base.OnNavigatedTo(e);
		}

		protected virtual void OnNavigatedTo(NavigationEventArgs e, bool isNewPage)
		{
			// must be empty
		}

		public bool TryNavigate(Uri uri)
		{
			return PhoneApplicationPageExtensions.TryNavigate(this, uri);
		}
	}
}
