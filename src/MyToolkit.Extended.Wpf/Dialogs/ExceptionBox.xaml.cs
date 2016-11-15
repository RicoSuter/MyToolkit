using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MyToolkit.Dialogs
{
    public partial class ExceptionBox
    {
        protected ExceptionBox()
        {
            InitializeComponent();

            var error = SystemIcons.Error;
            var image = Imaging.CreateBitmapSourceFromHIcon(error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Image.Source = image;
        }

        public static void Show(string title, Exception exception)
        {
            Show(title, null, exception, null);
        }

        public static void Show(string title, Exception exception, Window owner)
        {
            Show(title, null, exception, owner);
        }

        public static void Show(string title, string description, Exception exception)
        {
            Show(title, description, exception, null);
        }

        public static void Show(string title, string description, Exception exception, Window owner)
        {
            if (string.IsNullOrEmpty(description))
                description = exception.Message;

            var dlg = new ExceptionBox();
            dlg.Title = title; 
            dlg.Description.Text = description;
            dlg.Remarks.Text = exception.ToString();
            dlg.WindowStartupLocation = owner != null
                ? WindowStartupLocation.CenterOwner
                : WindowStartupLocation.CenterScreen;

            if (owner != null)
                dlg.Owner = owner;

            dlg.Button.Focus();
            dlg.Show();
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
