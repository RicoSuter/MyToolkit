//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityInput.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>A container for accessing typed activity arguments. </summary>
    public class WorkflowActivityInput
    {
        /// <summary>Gets the workflow instance.</summary>
        [XmlIgnore]
        public WorkflowInstance Instance { get; internal set; }
    }
}