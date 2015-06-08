//-----------------------------------------------------------------------
// <copyright file="FileName.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Contains the activity output. </summary>
    public class ActivityData
    {
        /// <summary>Gets the activity ID. </summary>
        public string ActivityId { get; set; }

        /// <summary>Gets the last output of the activity. </summary>
        public WorkflowActivityOutput Output { get; set; }
    }
}