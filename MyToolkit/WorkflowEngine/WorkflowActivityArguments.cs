//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityArguments.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.WorkflowEngine
{
    /// <summary>A container for accessing typed activity arguments. </summary>
    public class WorkflowActivityArguments
    {
        private readonly object[] _args;

        /// <summary>Initializes a new instance of the <see cref="WorkflowActivityArguments"/> class. </summary>
        public WorkflowActivityArguments(object[] args)
        {
            _args = args; 
        }

        /// <summary>Gets a typed argument. </summary>
        /// <typeparam name="T">The argument type. </typeparam>
        /// <param name="index">The index. </param>
        /// <returns>The argument value. </returns>
        public T GetArgument<T>(int index)
        {
            return (T) _args[index];
        }
    }
}