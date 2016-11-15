using Microsoft.Win32;
using NSwagStudio.Controls;

namespace MyToolkit.Dialogs
{
    public class FileSavePicker : FilePickerBase
    {
        protected override void SelectFile()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = DefaultExtension;
            dlg.Filter = Filter;
            if (dlg.ShowDialog() == true)
                FilePath = dlg.FileName;
        }
    }
}