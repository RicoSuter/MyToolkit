//-----------------------------------------------------------------------
// <copyright file="DisposableProvider.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
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

        public void Dispose()
        {
            _endAction();
        }
    }
}