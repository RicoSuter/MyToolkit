using MyToolkit.MVVM;
using MyToolkit.Mvvm;

namespace SampleWindowsStoreApp.ViewModels
{
    public class VariousSamplesPageModel : ViewModelBase
    {
		public string Html { get; private set; }

		public VariousSamplesPageModel()
		{
			Html = "<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor " +
					   "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
					   "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure " +
					   "dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
					   "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt " +
					   "mollit anim id est laborum. <br />a<br /><br /><br />" +
					   "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor " +
					   "incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud " +
					   "exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure " +
					   "dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. " +
					   "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt " +
					   "mollit anim id est laborum." +
				   "</p>" +
			       "<p>test <strong>test</strong> test</p>";
		}
	}
}
