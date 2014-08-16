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
    public class WorkflowException : Exception
    {
        public WorkflowException(string message) : base(message) { }
    }
}
