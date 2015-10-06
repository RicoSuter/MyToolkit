//-----------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyToolkit.Collections;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for dictionaries. </summary>
    public static class DictionaryExtensions
    {
        /// <summary>Recursively copies a dictionary. </summary>
        /// <param name="dictionary">The dictionary to copy. </param>
        /// <returns>The copied dictionary. </returns>
        public static Dictionary<string, object> DeepCopy(this Dictionary<string, object> dictionary)
        {
            var output = new Dictionary<string, object>();
            foreach (var p in dictionary)
            {
                if (p.Value is Dictionary<string, object>)
                    output[p.Key] = DeepCopy((Dictionary<string, object>)p.Value);
                else if (p.Value is List<Dictionary<string, object>>)
                {
                    var list = (List<Dictionary<string, object>>)p.Value;
                    output[p.Key] = list.Select(DeepCopy).ToList();
                }
                else
                    output[p.Key] = p.Value;
            }
            return output;
        }

        /// <summary>Recursively copies an observable dictionary. </summary>
        /// <param name="dictionary">The observable dictionary to copy. </param>
        /// <returns>The copied observable dictionary. </returns>
        public static ObservableDictionary<string, object> DeepCopy(this ObservableDictionary<string, object> dictionary)
        {
            var output = new ObservableDictionary<string, object>();
            foreach (var p in dictionary)
            {
                if (p.Value is ObservableDictionary<string, object>)
                    output[p.Key] = DeepCopy((ObservableDictionary<string, object>)p.Value);
                else if (p.Value is ObservableCollection<ObservableDictionary<string, object>>)
                {
                    var list = (ObservableCollection<ObservableDictionary<string, object>>)p.Value;
                    output[p.Key] = new ObservableCollection<ObservableDictionary<string, object>>(list.Select(DeepCopy));
                }
                else
                    output[p.Key] = p.Value;
            }
            return output;
        }
    }
}
