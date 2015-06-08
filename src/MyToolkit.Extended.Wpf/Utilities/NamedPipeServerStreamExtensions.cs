//-----------------------------------------------------------------------
// <copyright file="NamedPipeServerStreamExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;

namespace MyToolkit.Utilities
{
    public static class NamedPipeServerStreamExtensions
    {
        [DebuggerStepThrough]
        public static void WaitForConnection(this NamedPipeServerStream stream, ManualResetEvent cancelEvent)
        {
            Exception exception = null;

            var connectEvent = new AutoResetEvent(false);
            stream.BeginWaitForConnection(result =>
            {
                try { stream.EndWaitForConnection(result); }
                catch (Exception ex) { exception = ex; }

                connectEvent.Set();
            }, null);

            if (WaitHandle.WaitAny(new WaitHandle[] { connectEvent, cancelEvent }) == 1)
                stream.Close();

            if (exception != null)
                throw exception;
        }
    }
}