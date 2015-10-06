//-----------------------------------------------------------------------
// <copyright file="WorkflowTransition.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Describes the transition between two activities. </summary>
    public class WorkflowTransition
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowTransition"/> class. </summary>
        public WorkflowTransition()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowTransition"/> class. </summary>
        public WorkflowTransition(IWorkflowActivityBase from, IWorkflowActivityBase to)
        {
            From = from.Id;
            To = to.Id;
        }

        /// <summary>Gets or sets the start of the transition. </summary>
        [XmlAttribute]
        public string From { get; set; }

        /// <summary>Gets or sets the end of the transition. </summary>
        [XmlAttribute]
        public string To { get; set; }

        /// <summary>Gets or sets the condition for the transition. </summary>
        [XmlAttribute]
        public string Condition { get; set; }

        /// <summary>Gets a value indicating whether the transition is conditional. </summary>
        [XmlIgnore]
        public bool IsConditional
        {
            get { return !string.IsNullOrEmpty(Condition); }
        }
    }
}