//-----------------------------------------------------------------------
// <copyright file="DefaultActions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Windows;

#if WP7 || WP8
using MyToolkit.Environment;
using MyToolkit.Paging;
#endif
#if WINRT
using MyToolkit.Paging;
using System.Threading.Tasks;
using MyToolkit.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
#endif

namespace MyToolkit.Messaging
{
    /// <summary>Provides default actions for some message lcasses to use with the <see cref="Messenger"/>. </summary>
    public static class DefaultActions
    {
#if WINRT

        /// <summary>Gets the default handling action of <see cref="GoBackMessage"/> objects. </summary>
        /// <param name="frame">The frame to call the navigation methods on. </param>
        /// <returns>The message action. </returns>
        public static Action<GoBackMessage> GetGoBackMessageAction(MtFrame frame)
        {
            return message => frame.GoBackAsync();
        }

        /// <summary>Gets the default handling action of <see cref="NavigateMessage"/> objects. </summary>
        /// <param name="mapper">The mapper which maps view model types to view types. </param>
        /// <param name="frame">The frame. </param>
        /// <returns>The message action. </returns>
        public static Action<NavigateMessage> GetNavigateMessageAction(IViewModelToViewMapper mapper, MtFrame frame)
        {
            return async message => await frame.NavigateAsync(mapper.Map(message.ViewModelType), message.Parameter);
        }

        /// <summary>Gets the default handling action of <see cref="NavigateMessage"/> objects. </summary>
        /// <param name="mapper">The mapper which maps view model types to view types. </param>
        /// <param name="frame">The frame. </param>
        /// <returns>The message action. </returns>
        public static Action<NavigateMessage> GetNavigateMessageAction(IViewModelToViewMapper mapper, Frame frame)
        {
            return message => frame.Navigate(mapper.Map(message.ViewModelType), message.Parameter);
        }

        /// <summary>Gets the default handling action of <see cref="TextMessage"/> objects. </summary>
        /// <returns>The message action. </returns>
        public static Action<TextMessage> GetTextMessageAction()
        {
            return message => GetTextMessageImplementation(message);
        }

        private static async Task GetTextMessageImplementation(TextMessage message)
        {
            if (message.Button == MessageButton.OK)
            {
                var msg = new MessageDialog(message.Text, message.Title);
                await msg.ShowAsync();
            }
            else
            {
                var msg = new MessageDialog(message.Text, message.Title);
                
                if (message.Button == MessageButton.OKCancel)
                {
                    msg.Commands.Add(new UICommand(Strings.ButtonOk));
                    msg.Commands.Add(new UICommand(Strings.ButtonCancel));
                }
                else if (message.Button == MessageButton.YesNoCancel || message.Button == MessageButton.YesNo)
                {
                    msg.Commands.Add(new UICommand(Strings.ButtonYes));
                    msg.Commands.Add(new UICommand(Strings.ButtonNo));
                    
                    if (message.Button == MessageButton.YesNoCancel)
                        msg.Commands.Add(new UICommand(Strings.ButtonCancel));
                }

                msg.DefaultCommandIndex = 0;
                msg.CancelCommandIndex = 1;

                var cmd = await msg.ShowAsync();

                var index = msg.Commands.IndexOf(cmd); 
                if (message.Button == MessageButton.OKCancel)
                    message.CallSuccessCallback(index == 0 ? MessageResult.OK : MessageResult.Cancel);
                else if (message.Button == MessageButton.YesNoCancel)
                    message.CallSuccessCallback(index == 0 ? MessageResult.Yes : 
                        (index == 1 ? MessageResult.No : MessageResult.Cancel));
                else if (message.Button == MessageButton.YesNo)
                    message.CallSuccessCallback(index == 0 ? MessageResult.Yes : MessageResult.No);
            }
        }

#else

        public static Action<TextMessage> GetTextMessageAction()
        {
            return message =>
            {
                var result = MessageBoxResult.OK;
                if (message.Button == MessageButton.OK)
                    result = MessageBox.Show(message.Text, message.Title, MessageBoxButton.OK);
                else if (message.Button == MessageButton.OKCancel)
                    result = MessageBox.Show(message.Text, message.Title, MessageBoxButton.OKCancel);
#if !WP8 && !WP7 && !SL5
                else if (message.Button == MessageButton.YesNo)
                    result = MessageBox.Show(message.Text, message.Title, MessageBoxButton.YesNo);
                else if (message.Button == MessageButton.YesNoCancel)
                    result = MessageBox.Show(message.Text, message.Title, MessageBoxButton.YesNoCancel);
#else
                else
                    throw new Exception(string.Format("The MessageButton type '{0}' is not available on this platform. ", message.Button));
#endif

                if (result == MessageBoxResult.Yes)
                    message.CallSuccessCallback(MessageResult.Yes);
                else if (result == MessageBoxResult.No)
                    message.CallSuccessCallback(MessageResult.No);
                else if (result == MessageBoxResult.OK)
                    message.CallSuccessCallback(MessageResult.OK);
                else if (result == MessageBoxResult.Cancel)
                    message.CallSuccessCallback(MessageResult.Cancel);
            };
        }

#endif

#if WP7 || WP8

        public static Action<GoBackMessage> GetGoBackMessageAction()
        {
            return message =>
            {
                var page = PhoneApplication.CurrentPage; 
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        page.NavigationService.GoBack();
                        if (message.Completed != null)
                            message.Completed(true);
                    }
                    catch
                    {
                        if (message.Completed != null)
                            message.Completed(false);
                    }
                });
            };
        }

#endif
    }
}
