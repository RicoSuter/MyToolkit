//-----------------------------------------------------------------------
// <copyright file="WorkflowInstanceData.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Contains the workflow instance state. </summary>
    [XmlType("InstanceData")]
    public class WorkflowInstanceData : ActivityDataBase
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowInstanceData"/> class. </summary>
        public WorkflowInstanceData()
        {
            CurrentActivityIds = new List<string>();
        }

        /// <summary>Gets or sets the current activity IDs. </summary>
        [XmlArray("CurrentActivities"), XmlArrayItem("Activity")]
        public List<string> CurrentActivityIds { get; set; }
    }
}