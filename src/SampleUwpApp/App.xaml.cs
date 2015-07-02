using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.ApplicationInsights;
using MyToolkit.Paging;
using SampleUwpApp.Views;

namespace SampleUwpApp
{
    sealed partial class App : MtApplication
    {
        public static TelemetryClient TelemetryClient;

        public App()
        {
            TelemetryClient = new TelemetryClient();
            InitializeComponent();
        }

        public override Type StartPageType => typeof (MainPage);

        public override UIElement CreateWindowContentElement()
        {
            return new HamburgerShell();
        }

        public override MtFrame GetFrame(UIElement windowContentElement)
        {
            return ((HamburgerShell)windowContentElement).Frame;
        }

        public override Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            return base.OnInitializedAsync(frame, args);
        }
    }
}
