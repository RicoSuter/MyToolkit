//-----------------------------------------------------------------------
// <copyright file="DisposableProvider.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
    /// <summary>A helper class to create custom using regions.</summary>
    public class DisposableProvider : IDisposable
    {
        private readonly Action _endAction;

        /// <summary>Initializes a new instance of the <see cref="DisposableProvider"/> class. </summary>
        /// <param name="beginAction">The begin action. </param>
        /// <param name="endAction">The end action. </param>
        public DisposableProvider(Action beginAction, Action endAction)
        {
            _endAction = endAction;
            beginAction();
        }

        /// <summary>Initializes a new instance of the <see cref="DisposableProvider"/> class. </summary>
        /// <param name="endAction">The end action. </param>
        public DisposableProvider(Action endAction)
        {
            _endAction = endAction;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _endAction();
        }
    }
}