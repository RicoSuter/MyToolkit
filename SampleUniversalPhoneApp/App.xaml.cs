using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using MyToolkit.Paging;
using MyToolkit.Paging.Animations;
using SampleUniversalPhoneApp.Views;

namespace SampleUniversalPhoneApp
{
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        /// <summary>Gets the type of the start page (first page when launching application). </summary>
        public override Type StartPageType
        {
            get { return typeof(MainPage); }
        }

        /// <summary>Called when a new instance of the application has been created. </summary>
        /// <param name="frame">The frame. </param>
        /// <param name="args">The launch arguments.</param>
        public override Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            //frame.PageAnimation = new PushPageAnimation();

            // TODO: Run when the app is started (not resumed)
            return null;
        }
    }
}