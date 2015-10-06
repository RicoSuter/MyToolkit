//-----------------------------------------------------------------------
// <copyright file="WorkflowException.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.WorkflowEngine.Exceptions
{
    /// <summary>The workflow exception. </summary>
    public class WorkflowException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowException"/> class. </summary>
        public WorkflowException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="WorkflowException"/> class. </summary>
        public WorkflowException(string message, Exception innerException) : base(message, innerException) { }

    }
}
