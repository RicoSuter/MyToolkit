//-----------------------------------------------------------------------
// <copyright file="IPageAnmiation.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyToolkit.Paging.Animations
{
    public interface IPageAnimation
    {
        Task NavigatingFromForward(FrameworkElement previousPage, FrameworkElement nextPage);
        Task NavigatedToForward(FrameworkElement previousPage, FrameworkElement nextPage);

        Task NavigatingFromBackward(FrameworkElement previousPage, FrameworkElement nextPage);
        Task NavigatedToBackward(FrameworkElement previousPage, FrameworkElement nextPage);
    }
}