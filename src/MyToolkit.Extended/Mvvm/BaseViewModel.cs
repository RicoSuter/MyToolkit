//-----------------------------------------------------------------------
// <copyright file="DependencyPropertyHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.MVVM
{
	public class BaseViewModel<T> : NotifyPropertyChanged<T> where T : BaseViewModel<T>
	{
		private int _loadingCounter = 0;

		public bool IsLoading
		{
			get { return _loadingCounter > 0; }
			set 
			{
				if (value)
                    _loadingCounter++;
				else if (_loadingCounter > 0)
                    _loadingCounter--;

#if WINRT
				RaisePropertyChanged();
#else
				RaisePropertyChanged("IsLoading");
#endif
			}
		}
	}
}
