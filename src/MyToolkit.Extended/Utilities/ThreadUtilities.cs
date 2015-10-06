//-----------------------------------------------------------------------
// <copyright file="FileName.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Utilities
{
    /// <summary>Provides thread related utility methods. </summary>
    public class ThreadUtilities
    {
        /// <summary>Blocks the thread for multiple milliseconds. </summary>
        /// <param name="ms">The wait time in milliseconds. </param>
        public static void Sleep(int ms)
        {
            new System.Threading.ManualResetEvent(false).WaitOne(ms);
        }
    }
}
