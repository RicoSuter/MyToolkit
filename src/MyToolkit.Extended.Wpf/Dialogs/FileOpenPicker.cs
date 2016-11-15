using Microsoft.Win32;
using NSwagStudio.Controls;

namespace MyToolkit.Dialogs
{
    public class FileOpenPicker : FilePickerBase
    {
        protected override void SelectFile()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = DefaultExtension;
            dlg.Filter = Filter;
            if (dlg.ShowDialog() == true)
                FilePath = dlg.FileName;
        }
    }
}