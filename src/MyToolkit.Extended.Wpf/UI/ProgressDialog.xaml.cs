using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.UI
{
    public partial class ProgressDialog : Window
    {
        public ProgressDialog(string title, string label)
        {
            InitializeComponent();
            Title = title; 
            TextBlock.Text = label; 
        }

        public bool IsIndeterminate
        {
            get { return ProgressBar.IsIndeterminate; }
            set { ProgressBar.IsIndeterminate = value; }
        }

        public double Minimum
        {
            get { return ProgressBar.Minimum; }
            set { ProgressBar.Minimum = value; }
        }

        public double Maximum
        {
            get { return ProgressBar.Maximum; }
            set { ProgressBar.Maximum = value; }
        }

        public double Value
        {
            get { return ProgressBar.Value; }
            set { ProgressBar.Value = value; }
        }
    }
}
