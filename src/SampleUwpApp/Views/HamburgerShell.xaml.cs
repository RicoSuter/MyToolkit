using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using MyToolkit.Paging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SampleUwpApp.Views
{
    public sealed partial class HamburgerShell : UserControl
    {
        public HamburgerShell()
        {
            InitializeComponent();

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        public MtFrame Frame => RootFrame;
    }
}
