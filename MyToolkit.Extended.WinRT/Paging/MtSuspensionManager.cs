//-----------------------------------------------------------------------
// <copyright file="MtSuspensionManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MyToolkit.Paging
{
    /// <summary>Stores and loads global session state for application life cycle management. </summary>
    public sealed class MtSuspensionManager
    {
        private const string ApplicationStateFilename = "MtSuspensionManager_ApplicationState.xml";

        private static readonly object _lock = new object();
        private static readonly List<WeakReference<MtSuspensionManager>> _managers = new List<WeakReference<MtSuspensionManager>>();
        private static readonly HashSet<Type> _knownTypes = new HashSet<Type>();
        private static Dictionary<string, Dictionary<string, object>> _applicationState = new Dictionary<string, Dictionary<string, object>>();

        private readonly MtFrame _frame;
        private readonly string _sessionStateKey;
        
        public MtSuspensionManager(MtFrame frame, string sessionStateKey)
        {
            _frame = frame;
            _sessionStateKey = sessionStateKey;

            AddManager(this);
            Restore();
        }

        /// <summary>Gets a list of known types for the <see cref="DataContractSerializer"/> 
        /// to serialize the <see cref="SessionState"/>. </summary>
        public static Type[] KnownTypes
        {
            get { return _knownTypes.ToArray(); }
        }

        /// <summary>Gets the session state for the current session. 
        /// The objects must be serializable with the <see cref="DataContractSerializer"/> 
        /// and the types provided in <see cref="KnownTypes"/>. </summary>
        public Dictionary<string, object> SessionState
        {
            get
            {
                lock (_lock)
                {
                    if (!_applicationState.ContainsKey(_sessionStateKey))
                        _applicationState[_sessionStateKey] = new Dictionary<string, object>();
                    return _applicationState[_sessionStateKey]; 
                }
            }
            set { _applicationState[_sessionStateKey] = value; }
        }

        public static void AddKnownType(Type type)
        {
            _knownTypes.Add(type);
        }

        private void Save()
        {
            SessionState["Navigation"] = _frame.GetNavigationState();
        }

        private void Restore()
        {
            if (SessionState.ContainsKey("Navigation"))
                _frame.SetNavigationState((String)SessionState["Navigation"]);
        }

        /// <summary>Restores the previously stored session state. </summary>
        public static async Task RestoreAsync()
        {
            var folder = ApplicationData.Current.LocalFolder;
            using (var stream = await folder.OpenStreamForReadAsync(ApplicationStateFilename))
            {
                var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                _applicationState = (Dictionary<string, Dictionary<string, object>>)serializer.ReadObject(stream);
            }

            foreach (var manager in GetSuspensionManagerEnumerator())
                manager.Restore();
        }

        public static async Task SaveAsync()
        {
            foreach (var manager in GetSuspensionManagerEnumerator())
                manager.Save();

            var stream = new MemoryStream();
            var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
            serializer.WriteObject(stream, _applicationState);

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(ApplicationStateFilename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(file, stream.GetWindowsRuntimeBuffer());
        }

        private static void AddManager(MtSuspensionManager addedManager)
        {
            lock (_lock)
                _managers.Add(new WeakReference<MtSuspensionManager>(addedManager));
        }

        private static List<MtSuspensionManager> GetSuspensionManagerEnumerator()
        {
            var managers = new List<MtSuspensionManager>();
            lock (_lock)
            {
                foreach (var reference in _managers.ToArray())
                {
                    MtSuspensionManager manager;
                    if (reference.TryGetTarget(out manager))
                        managers.Add(manager);
                    else
                        _managers.Remove(reference);
                }
            }
            return managers;
        }
    }
}
