//-----------------------------------------------------------------------
// <copyright file="MtLoadStateEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.Generic;
using MyToolkit.Serialization;

namespace MyToolkit.Paging
{
    public class MtLoadStateEventArgs : EventArgs
    {
        public MtLoadStateEventArgs(object navigationParameter, Dictionary<string, object> pageState)
        {
            NavigationParameter = navigationParameter;
            PageState = pageState;
        }

        /// <summary>A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited. </summary>
        public Dictionary<string, Object> PageState { get; private set; }

        /// <summary>The parameter value passed to <see cref="MtFrame.Navigate"/> 
        /// when this page was initially requested. </summary>
        public Object NavigationParameter { get; private set; }

        /// <summary>Gets an object which is deserialized with <see cref="XmlSerialization"/>. </summary>
        /// <typeparam name="T">The type of the object. </typeparam>
        /// <param name="key">The key. </param>
        /// <returns>The object. </returns>
        public T GetWithXmlSerializer<T>(string key)
        {
            if (PageState != null)
                return XmlSerialization.Deserialize<T>((string)PageState[key]);
            return default(T);
        }

        /// <summary>Gets an object which is deserialized with <see cref="XmlSerialization"/>. </summary>
        /// <param name="key">The key. </param>
        /// <param name="type">The type of the object. </param>
        /// <returns>The object. </returns>
        public object GetWithXmlSerializer(string key, Type type)
        {
            if (PageState != null)
                return XmlSerialization.Deserialize<object>((string)PageState[key], new[] { type });
            return null;
        }

        /// <summary>Gets an object which is deserialized with <see cref="DataContractSerialization"/>. </summary>
        /// <typeparam name="T">The type of the object. </typeparam>
        /// <param name="key">The key. </param>
        /// <returns>The object. </returns>
        public T Get<T>(string key)
        {
            if (PageState != null)
                return DataContractSerialization.Deserialize<T>((string)PageState[key]);
            return default(T);
        }

        ///// <summary>Gets an object which is deserialized with <see cref="DataContractSerialization"/>. </summary>
        ///// <param name="type">The type of the object. </param>
        ///// <param name="key">The key. </param>
        ///// <returns>The object. </returns>
        //public object Get(string key, Type type)
        //{
        //    if (PageState != null)
        //        return DataContractSerialization.Deserialize((string)PageState[key], type);
        //    return null;
        //}
    }
}

#endif