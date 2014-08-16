//-----------------------------------------------------------------------
// <copyright file="WorkflowTransition.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Xml.Serialization;
using MyToolkit.WorkflowEngine.Activities;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>
    /// Describes the transition between two activities. 
    /// </summary>
    public class WorkflowTransition
    {
        public WorkflowTransition()
        {
        }

        public WorkflowTransition(WorkflowActivityBase from, WorkflowActivityBase to)
        {
            From = from.Id;
            To = to.Id;
        }

        /// <summary>
        /// Gets or sets the start of the transition. 
        /// </summary>
        [XmlAttribute]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the end of the transition. 
        /// </summary>
        [XmlAttribute]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the condition for the transition. 
        /// </summary>
        [XmlAttribute]
        public string Condition { get; set; }

        /// <summary>
        /// Gets a value indicating whether the transition is conditional. 
        /// </summary>
        [XmlIgnore]
        public bool IsConfitional
        {
            get { return !string.IsNullOrEmpty(Condition); }
        }
    }
}