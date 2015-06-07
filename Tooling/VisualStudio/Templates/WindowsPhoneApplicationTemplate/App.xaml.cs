using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using WindowsPhoneApplicationTemplate.Views;
using MyToolkit.Messaging;
using MyToolkit.Paging;

namespace WindowsPhoneApplicationTemplate
{
    public sealed partial class App : MtApplication
    {
        /// <summary>Gets the type of the start page (first page when launching application). </summary>
        public override Type StartPageType
        {
            get { return typeof (MainPage); }
        }

        /// <summary>Called when a new instance of the application has been created. </summary>
        /// <param name="frame">The frame. </param><param name="args">The launch arguments.</param>
        public override Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            // TODO: Add your initialization logic here. 

            var mapper = RegexViewModelToViewMapper.CreateDefaultMapper(GetType().GetTypeInfo().Assembly);
            Messenger.Default.Register(DefaultActions.GetNavigateMessageAction(mapper, frame));
            Messenger.Default.Register(DefaultActions.GetGoBackMessageAction(frame));
            Messenger.Default.Register(DefaultActions.GetTextMessageAction());

            return base.OnInitializedAsync(frame, args);
        }
    }
}