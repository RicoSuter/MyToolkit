#if WINRT

using System;
using System.Collections.Generic;
using MyToolkit.Serialization;

namespace MyToolkit.Paging
{
    public class MtSaveStateEventArgs : EventArgs
    {
        public MtSaveStateEventArgs(Dictionary<string, Object> pageState)
        {
            PageState = pageState;
        }

        /// <summary>A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited. </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>Gets a value indicating whether there is page state available. </summary>
        public bool HasPageState
        {
            get { return PageState != null; }
        }

        /// <summary>Adds an object which is serialized with <see cref="XmlSerialization"/>. </summary>
        /// <typeparam name="T">The type of the object. </typeparam>
        /// <param name="key">The key. </param>
        /// <param name="obj">The object. </param>
        /// <param name="extraTypes">The additional types. </param>
        public void SetWithXmlSerializer<T>(string key, T obj, Type[] extraTypes = null)
        {
            PageState[key] = XmlSerialization.Serialize(obj, extraTypes);
        }

        /// <summary>Adds an object which is serialized with <see cref="DataContractSerialization"/>. </summary>
        /// <typeparam name="T">The type of the object. </typeparam>
        /// <param name="key">The key. </param>
        /// <param name="obj">The object. </param>
        /// <param name="extraTypes">The additional types. </param>
        public void Set<T>(string key, T obj, Type[] extraTypes = null)
        {
            PageState[key] = DataContractSerialization.Serialize(obj, true, extraTypes);
        }
    }
}

#endif