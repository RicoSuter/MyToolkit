using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyToolkit.Events;

#if WINDOWS_UAP
using Windows.UI.Core;
#endif

namespace MyToolkit.Paging.Handlers
{
    /// <summary>Registers for the hardware back key button on Windows Phone and calls the registered methods when the event occurs. </summary>
    public class BackKeyPressedHandler
    {
        private Type _hardwareButtonsType = null;
        private object _registrationToken;
        private bool _isEventRegistered = false;

        private readonly List<Tuple<MtPage, Func<object, bool>>> _handlers;

        public BackKeyPressedHandler()
        {
            _handlers = new List<Tuple<MtPage, Func<object, bool>>>();
        }
        
        /// <summary>Adds a back key handler for a given page. </summary>
        /// <param name="page">The page. </param>
        /// <param name="handler">The handler. </param>
        public void Add(MtPage page, Func<object, bool> handler)
        {
            if (!_isEventRegistered)
            {
#if WINDOWS_UAP
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackKeyPressed;
#else
                if (_hardwareButtonsType == null)
                {
                    _hardwareButtonsType = Type.GetType(
                        "Windows.Phone.UI.Input.HardwareButtons, " +
                        "Windows, Version=255.255.255.255, Culture=neutral, " +
                        "PublicKeyToken=null, ContentType=WindowsRuntime");
                }

                _registrationToken = EventUtilities.RegisterStaticEvent(_hardwareButtonsType, "BackPressed", OnBackKeyPressed);
#endif
                _isEventRegistered = true;
            }

            _handlers.Insert(0, new Tuple<MtPage, Func<object, bool>>(page, handler));
        }

        /// <summary>Removes a back key pressed handler for a given page. </summary>
        /// <param name="page">The page. </param>
        public void Remove(MtPage page)
        {
            _handlers.Remove(_handlers.Single(h => h.Item1 == page));

            if (_handlers.Count == 0)
            {
#if WINDOWS_UAP
                SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackKeyPressed;
#else
                EventUtilities.DeregisterStaticEvent(_hardwareButtonsType, "BackPressed", _registrationToken);
#endif
                _isEventRegistered = false; 
            }
        }

#if WINDOWS_UAP
        private void OnBackKeyPressed(object sender, BackRequestedEventArgs args)
        {
            var handled = args.Handled;
            if (handled)
                return;

            foreach (var item in _handlers)
            {
                handled = item.Item2(sender);
                args.Handled = handled;
                if (handled)
                    return;
            }
        }
#else
        private void OnBackKeyPressed(object sender, object args)
        {
            var property = args.GetType().GetRuntimeProperty("Handled");
            var handled = (bool)property.GetValue(args);
            if (handled)
                return;

            foreach (var item in _handlers)
            {
                handled = item.Item2(sender);
                property.SetValue(args, handled);
                if (handled)
                    return;
            }
        }
#endif
    }
}