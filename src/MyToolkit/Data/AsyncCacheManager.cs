//-----------------------------------------------------------------------
// <copyright file="AsyncCacheManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyToolkit.Data
{
    /// <summary>A cache manager which supports asynchronous, task based item creation functions.</summary>
    /// <typeparam name="TKey">The type of the key/identifier of an item.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class AsyncCacheManager<TKey, TItem> : IAsyncCacheManager<TKey, TItem>
    {
        private readonly object _lock = new object();
        private readonly Dictionary<TKey, Task<TItem>> _cache = new Dictionary<TKey, Task<TItem>>();

        /// <summary>Gets an existing item or asynchronously creates a new one.</summary>
        /// <param name="key">The key of the item.</param>
        /// <param name="creationFunction">The item creation function.</param>
        /// <returns>The item.</returns>
        public Task<TItem> GetOrCreateAsync(TKey key, Func<Task<TItem>> creationFunction)
        {
            lock (_lock)
            {
                if (!_cache.ContainsKey(key))
                {
                    var task = creationFunction();
                    _cache[key] = task;
                    return task;
                }

                return _cache[key];
            }
        }
    }
}
