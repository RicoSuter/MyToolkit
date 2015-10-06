//-----------------------------------------------------------------------
// <copyright file="MtSuspensionManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MyToolkit.Paging
{
    public delegate void SessionStateRestoredEventHandler(object sender, Dictionary<string, object> e);

    /// <summary>Stores and loads global session state for application life cycle management. </summary>
    public static class MtSuspensionManager
    {
        /// <summary>Occurs when the session state has been restored.</summary>
        public static event SessionStateRestoredEventHandler SessionStateRestored;

        private const string SessionStateFilename = "_sessionState.xml";

        private static readonly HashSet<Type> _knownTypes = new HashSet<Type>();
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();

        private static readonly List<WeakReference<MtFrame>> _registeredFrames = new List<WeakReference<MtFrame>>();

        /// <summary>Gets the session state for the current session. 
        /// The objects must be serializable with the <see cref="DataContractSerializer"/> 
        /// and the types provided in <see cref="KnownTypes"/>. </summary>
        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        /// <summary>Gets a list of known types for the <see cref="DataContractSerializer"/> 
        /// to serialize the <see cref="SessionState"/>. </summary>
        public static HashSet<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        /// <summary>Saves the current session state. </summary>
        public static async Task SaveAsync()
        {
            foreach (var weakFrameReference in _registeredFrames)
            {
                MtFrame frame;
                if (weakFrameReference.TryGetTarget(out frame))
                    SaveFrameNavigationState(frame);
            }

            var sessionData = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
            serializer.WriteObject(sessionData, _sessionState);

            var folder = ApplicationData.Current.LocalFolder; 
            var file = await folder.CreateFileAsync(SessionStateFilename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(file, sessionData.GetWindowsRuntimeBuffer());
        }

        /// <summary>Restores the previously stored session state. </summary>
        public static async Task RestoreAsync()
        {
            var folder = ApplicationData.Current.LocalFolder;
            using (var stream = await folder.OpenStreamForReadAsync(SessionStateFilename))
            {
                var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                _sessionState = (Dictionary<string, object>)serializer.ReadObject(stream);

                var copy = SessionStateRestored;
                if (copy != null)
                    copy(null, _sessionState);
            }
            
            foreach (var weakFrameReference in _registeredFrames)
            {
                MtFrame frame;
                if (weakFrameReference.TryGetTarget(out frame))
                {
                    frame.ClearValue(FrameSessionStateProperty);
                    RestoreFrameNavigationState(frame);
                }
            }
        }

        private static readonly DependencyProperty FrameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(MtSuspensionManager), null);

        private static readonly DependencyProperty FrameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(MtSuspensionManager), null);
        
        /// <summary>Registers a frame so that its navigation state can be saved and restored. </summary>
        /// <param name="frame">The frame. </param>
        /// <param name="sessionStateKey">The session state key. </param>
        public static void RegisterFrame(MtFrame frame, String sessionStateKey)
        {
            if (frame.GetValue(FrameSessionStateKeyProperty) != null)
                throw new InvalidOperationException("Frames can only be registered to one session state key");

            if (frame.GetValue(FrameSessionStateProperty) != null)
                throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");

            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            _registeredFrames.Add(new WeakReference<MtFrame>(frame));

            RestoreFrameNavigationState(frame);
        }

        /// <summary>Deregisters a frame. </summary>
        /// <param name="frame">The frame. </param>
        public static void DeregisterFrame(MtFrame frame)
        {
            SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
            _registeredFrames.RemoveAll((weakFrameReference) =>
            {
                MtFrame testFrame;
                return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
            });
        }

        /// <summary>Gets the session state for a given frame. </summary>
        /// <param name="frame">The frame. </param>
        /// <returns>The session state. </returns>
        public static Dictionary<String, Object> SessionStateForFrame(MtFrame frame)
        {
            var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);
            if (frameState == null)
            {
                var frameSessionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    if (!_sessionState.ContainsKey(frameSessionKey))
                        _sessionState[frameSessionKey] = new Dictionary<String, Object>();
                    frameState = (Dictionary<String, Object>)_sessionState[frameSessionKey];
                }
                else
                    frameState = new Dictionary<String, Object>();
                frame.SetValue(FrameSessionStateProperty, frameState);
            }
            return frameState;
        }

        private static void RestoreFrameNavigationState(MtFrame frame)
        {
            var frameState = SessionStateForFrame(frame);
            if (frameState.ContainsKey("Navigation"))
                frame.SetNavigationState((String)frameState["Navigation"]);
        }

        private static void SaveFrameNavigationState(MtFrame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState["Navigation"] = frame.GetNavigationState();
        }
    }
}

#endif