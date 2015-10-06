//-----------------------------------------------------------------------
// <copyright file="WelcomePage.xaml.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.UI.Xaml.Controls;
using SampleWindowsStoreApp.ViewModels;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class WelcomePage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (NavigationItem)e.ClickedItem;
            await Frame.NavigateAsync(item.PageType);
        }
    }
}
