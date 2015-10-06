//-----------------------------------------------------------------------
// <copyright file="CallbackMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace MyToolkit.Messaging
{
    /// <summary>Provides a message with a callback. </summary>
    public abstract class CallbackMessage
    {
        private readonly TaskCompletionSource<bool> _taskSource = new TaskCompletionSource<bool>();

        /// <summary>Gets the task to await for the callback call. </summary>
        public Task Task
        {
            get { return _taskSource.Task; }
        }

        /// <summary>Gets or sets the callback which is called when the processing of the message was successful. </summary>
        public Action SuccessCallback { get; set; }

        /// <summary>Gets or sets the callback which is called when the processing of the message failed. </summary>
        public Action FailCallback { get; set; }

        /// <summary>Calls the success callback. </summary>
        public void CallSuccessCallback()
        {
            if (SuccessCallback != null)
                SuccessCallback();

            _taskSource.SetResult(true);
        }

        /// <summary>Calls the fail callback. </summary>
        public void CallFailCallback()
        {
            if (FailCallback != null)
                FailCallback();

            _taskSource.SetResult(false);
        }
    }

    /// <summary>Provides a message with a callback with argument. </summary>
    /// <typeparam name="T">The type of the first parameter of the callback. </typeparam>
    public abstract class CallbackMessage<T>
    {
        private readonly TaskCompletionSource<CallbackMessageResult<T>> _taskSource = new TaskCompletionSource<CallbackMessageResult<T>>();

        /// <summary>Gets the task to await for the callback call. </summary>
        public Task<CallbackMessageResult<T>> Task
        {
            get { return _taskSource.Task; }
        }

        /// <summary>Gets or sets the callback which is called when the processing of the message was successful. </summary>
        public Action<T> SuccessCallback { get; set; }
        
        /// <summary>Gets or sets the callback which is called when the processing of the message failed. </summary>
        public Action<T> FailCallback { get; set; }

        /// <summary>Calls the success callback. </summary>
        public void CallSuccessCallback(T result)
        {
            if (SuccessCallback != null)
                SuccessCallback(result);

            _taskSource.SetResult(new CallbackMessageResult<T>(true, result));
        }

        /// <summary>Calls the fail callback. </summary>
        public void CallFailCallback(T result)
        {
            if (FailCallback != null)
                FailCallback(result);

            _taskSource.SetResult(new CallbackMessageResult<T>(false, result));
        }
    }

    /// <summary>Provides a message with a callback two arguments. </summary>
    /// <typeparam name="TFirst">The type of the first parameter of the callback. </typeparam>
    /// <typeparam name="TSecond">The type of the second parameter of the callback. </typeparam>
    public abstract class CallbackMessage<TFirst, TSecond>
    {
        private readonly TaskCompletionSource<CallbackMessageResult<TFirst, TSecond>> _taskSource =
            new TaskCompletionSource<CallbackMessageResult<TFirst, TSecond>>();

        /// <summary>
        /// Gets the task to await for the callback call. 
        /// </summary>
        public Task<CallbackMessageResult<TFirst, TSecond>> Task
        {
            get { return _taskSource.Task; }
        }

        /// <summary>Gets or sets the callback which is called when the processing of the message was successful. </summary>
        public Action<TFirst, TSecond> SuccessCallback { get; set; }

        /// <summary>Gets or sets the callback which is called when the processing of the message failed. </summary>
        public Action<TFirst, TSecond> FailCallback { get; set; }

        /// <summary>Calls the success callback. </summary>
        public void CallSuccessCallback(TFirst first, TSecond second)
        {
            if (SuccessCallback != null)
                SuccessCallback(first, second);

            _taskSource.SetResult(new CallbackMessageResult<TFirst, TSecond>(true, first, second));
        }

        /// <summary>Calls the fail callback. </summary>
        public void CallFailCallback(TFirst first, TSecond second)
        {
            if (FailCallback != null)
                FailCallback(first, second);

            _taskSource.SetResult(new CallbackMessageResult<TFirst, TSecond>(false, first, second));
        }
    }

    /// <summary>Provides a message with a callback with three arguments. </summary>
    /// <typeparam name="TFirst">The type of the first parameter of the callback. </typeparam>
    /// <typeparam name="TSecond">The type of the second parameter of the callback. </typeparam>
    /// <typeparam name="TThird">The type of the third parameter of the callback. </typeparam>
    public abstract class CallbackMessage<TFirst, TSecond, TThird>
    {
        private readonly TaskCompletionSource<CallbackMessageResult<TFirst, TSecond, TThird>> _taskSource =
            new TaskCompletionSource<CallbackMessageResult<TFirst, TSecond, TThird>>();

        /// <summary>
        /// Gets the task to await for the callback call. 
        /// </summary>
        public Task<CallbackMessageResult<TFirst, TSecond, TThird>> Task
        {
            get { return _taskSource.Task; }
        }

        /// <summary>Gets or sets the callback which is called when the processing of the message was successful. </summary>
        public Action<TFirst, TSecond, TThird> SuccessCallback { get; set; }

        /// <summary>Gets or sets the callback which is called when the processing of the message failed. </summary>
        public Action<TFirst, TSecond, TThird> FailCallback { get; set; }

        /// <summary>Calls the success callback. </summary>
        public void CallSuccessCallback(TFirst first, TSecond second, TThird third)
        {
            if (SuccessCallback != null)
                SuccessCallback(first, second, third);

            _taskSource.SetResult(new CallbackMessageResult<TFirst, TSecond, TThird>(true, first, second, third));
        }

        /// <summary>Calls the fail callback. </summary>
        public void CallFailCallback(TFirst first, TSecond second, TThird third)
        {
            if (FailCallback != null)
                FailCallback(first, second, third);

            _taskSource.SetResult(new CallbackMessageResult<TFirst, TSecond, TThird>(false, first, second, third));
        }
    }

    public class CallbackMessageResult<T>
    {
        public CallbackMessageResult(bool success, T result)
        {
            Success = success;
            Result = result;
        }

        public bool Success { get; private set; }
        public T Result { get; private set; }
    }

    public class CallbackMessageResult<TFirst, TSecond>
    {
        public CallbackMessageResult(bool success, TFirst result1, TSecond result2)
        {
            Success = success;
            Result1 = result1;
            Result2 = result2;
        }

        public bool Success { get; private set; }

        public TFirst Result1 { get; private set; }
        public TSecond Result2 { get; private set; }
    }

    public class CallbackMessageResult<TFirst, TSecond, TThird>
    {
        public CallbackMessageResult(bool success, TFirst result1, TSecond result2, TThird result3)
        {
            Success = success;
            Result1 = result1;
            Result2 = result2;
            Result3 = result3;
        }

        public bool Success { get; private set; }

        public TFirst Result1 { get; private set; }
        public TSecond Result2 { get; private set; }
        public TThird Result3 { get; private set; }
    }
}
