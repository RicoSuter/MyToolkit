using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyToolkit.Utilities;

namespace MyToolkit.UI.Popups
{
	public partial class MultiButtonPopup
	{
		public static async Task<int> ShowAsync(string[] buttons)
		{
			var popup = new MultiButtonPopup(buttons);
			await ShowAsync(popup, true, false);
			return popup.SelectedButton;
		}

		public MultiButtonPopup(string[] buttons)
		{
			InitializeComponent();
			Canceled = false;

			var index = 0;
			foreach (var button in buttons)
			{
				var localIndex = index;

				var b = new Button { Content = button };
				b.Click += delegate
				{
					SelectedButton = localIndex; 
					Close();
				};

				buttonPanel.Children.Add(b);
				index++;
			}
		}

		public override void GoBack()
		{
			Canceled = true;
			SelectedButton = -1; 
			Close();
		}

		public int SelectedButton { get; private set; }
		public bool Canceled { get; private set; }
	}
}
