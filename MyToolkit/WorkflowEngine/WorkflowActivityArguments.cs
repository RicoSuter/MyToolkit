//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityArguments.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.WorkflowEngine
{
    public class WorkflowActivityArguments
    {
        private readonly object[] _args;

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