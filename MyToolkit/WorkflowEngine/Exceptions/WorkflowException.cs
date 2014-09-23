//-----------------------------------------------------------------------
// <copyright file="WorkflowException.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
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
    }
}
