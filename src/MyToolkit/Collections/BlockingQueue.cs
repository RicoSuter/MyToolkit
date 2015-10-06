//-----------------------------------------------------------------------
// <copyright file="BlockingQueue.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

namespace MyToolkit.Collections
{
    /// <summary>Thread-safe blocking queue. </summary>
    /// <typeparam name="T">The type of an item. </typeparam>
    public class BlockingQueue<T>
    {
        private bool _stop;
        private readonly int _queueSize;
        private readonly Queue<T> _queue;
        private readonly object _lock = new object();

        /// <summary>Initializes a new instance of the <see cref="BlockingQueue{T}"/> class. </summary>
        public BlockingQueue(int queueSize)
        {
            _queueSize = queueSize;
            _queue = new Queue<T>(queueSize);
        }

        /// <summary>Restarts the blocking queue. </summary>
        public void Restart()
        {
            lock (_lock)
                _stop = false;
        }

        /// <summary>Stops all waits in the <see cref="Enqueue"/> or <see cref="Dequeue"/> methods. </summary>
        public void Stop()
        {
            lock (_lock)
            {
                _stop = true;
                Monitor.PulseAll(_lock);
            }
        }

        /// <summary>Enqueues an item in the queue. </summary>
        /// <param name="item">The item to enqueue. </param>
        /// <param name="maximumWaitTime">The maximum wait time until free slot is available, 
        /// if exceeded then item is not added and false is returned. </param>
        /// <returns>True if successful. </returns>
        public bool Enqueue(T item, int maximumWaitTime = -1)
        {
            lock (_lock)
            {
                while (!_stop && _queue.Count >= _queueSize)
                {
                    if (!Monitor.Wait(_lock, maximumWaitTime))
                        return false;
                }

                if (_stop)
                    return false;

                _queue.Enqueue(item);
                Monitor.PulseAll(_lock);
            }
            return true;
        }

        /// <summary>Tries do dequeue an item. </summary>
        /// <param name="item">The dequeued item. </param>
        /// <param name="maximumWaitTime">The maximum wait time to wait until an item is 
        /// available if the queue is empty. Default is -1 (indefinitely). </param>
        /// <returns>True when an item could be dequeued during the maximum wait time; otherwise false. </returns>
        public bool Dequeue(out T item, int maximumWaitTime = -1)
        {
            item = default(T);
            lock (_lock)
            {
                while (!_stop && _queue.Count == 0)
                {
                    if (!Monitor.Wait(_lock, maximumWaitTime))
                        return false;
                }

                if (_queue.Count == 0)
                    return false;

                item = _queue.Dequeue();
                Monitor.PulseAll(_lock);
            }
            return true;
        }
    }
}
