//-----------------------------------------------------------------------
// <copyright file="HttpUtilityExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyToolkit.Utilities
{
    /// <summary>Provides HTTP utility methods. </summary>
    public static class HttpUtilityExtensions
    {
        /// <summary>Parses a given HTTP query string into key-value pairs. </summary>
        /// <param name="queryString">The query string to parse. </param>
        /// <returns>The key-value pairs. </returns>
        public static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var dict = new Dictionary<string, string>();
            foreach (var s in queryString.Split('&'))
            {
                var index = s.IndexOf('=');
                if (index != -1 && index + 1 < s.Length)
                {
                    var key = s.Substring(0, index);
                    var value = Uri.UnescapeDataString(s.Substring(index + 1));
                    dict.Add(key, value);
                }
            }
            return dict;
        }
    }
}
