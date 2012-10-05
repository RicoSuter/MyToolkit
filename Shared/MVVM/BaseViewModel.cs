using MyToolkit.MVVM;

namespace MyToolkit.MVVM
{
	public class BaseViewModel<T> : NotifyPropertyChanged<T> where T : BaseViewModel<T>
	{
		private int loadingCounter = 0;
		public bool IsLoading
		{
			get { return loadingCounter > 0; }
			set 
			{
				if (value)
					loadingCounter++;
				else if (loadingCounter > 0)
					loadingCounter--;

#if METRO
				RaisePropertyChanged();
#else
				RaisePropertyChanged("IsLoading");
#endif
			}
		}
	}
}
