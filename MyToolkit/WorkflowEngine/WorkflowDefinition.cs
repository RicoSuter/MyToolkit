//-----------------------------------------------------------------------
// <copyright file="Workflow.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

// Workflow shapes: http://msdn.microsoft.com/en-us/library/ee264487(v=bts.10).aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MyToolkit.Serialization;
using MyToolkit.WorkflowEngine.Activities;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>
    /// Describes a workflow using activities and transitions. 
    /// </summary>
    public class WorkflowDefinition
    {
        /// <summary>Gets or sets the workflow parameters. </summary>
        public WorkflowParametersBase Parameters { get; set; }
            
        /// <summary>Gets or sets the ID of the first activity. </summary>
        [XmlElement("StartActivity")]
        public string StartActivityId { get; set; }

        /// <summary>Gets or sets the workflow activities. </summary>
        [XmlArray]
        [XmlArrayItem("Activity")]
        public List<WorkflowActivityBase> Activities { get; set; }

        /// <summary>Gets or sets the workflow transitions. </summary>
        [XmlArray]
        [XmlArrayItem("Transition")]
        public List<WorkflowTransition> Transitions { get; set; }

        /// <summary>Gets or sets the first activity. </summary>
        [XmlIgnore]
        public WorkflowActivityBase StartActivity
        {
            get { return GetActivityById(StartActivityId); }
            set { StartActivityId = value.Id; }
        }

        /// <summary>Creates a workflow instance from a XML string. </summary>
        /// <param name="xml">The XML. </param>
        /// <param name="activityTypes">The possible activity types. </param>
        /// <param name="parameterTypes">The possible parameter types. </param>
        /// <returns>The workflow. </returns>
        public static WorkflowDefinition FromXml(string xml, Type[] activityTypes, Type[] parameterTypes)
        {
            var workflow = XmlSerialization.Deserialize<WorkflowDefinition>(xml, activityTypes.Union(parameterTypes).ToArray());
            workflow.StartActivity = workflow.GetActivityById(workflow.StartActivityId);
            return workflow;
        }

        /// <summary>Serializes the workflow to XML. </summary>
        /// <returns>The XML. </returns>
        public string ToXml()
        {
            var extraTypes = Parameters != null
                ? Activities.Select(a => a.GetType())
                    .Union(new[] {Parameters.GetType()})
                : Activities.Select(a => a.GetType());

            return XmlSerialization.Serialize(this, extraTypes.Distinct().ToArray());
        }

        /// <summary>Gets the parameters in a typed way. </summary>
        /// <typeparam name="T">The type of the parameters object. </typeparam>
        /// <returns>The parameters. </returns>
        public T GetParameters<T>() where T : WorkflowParametersBase
        {
            return (T) Parameters;
        }

        /// <summary>Creates an workflow instance based on this workflow definition. </summary>
        /// <returns>The <see cref="WorkflowInstance"/>. </returns>
        public WorkflowInstance CreateInstance()
        {
            return new WorkflowInstance(this, new WorkflowDataProvider());
        }

        /// <summary>Gets an activity by ID. </summary>
        /// <param name="activityId">The activity ID. </param>
        /// <returns>The <see cref="WorkflowActivityBase"/>. </returns>
        public WorkflowActivityBase GetActivityById(string activityId)
        {
            return Activities.Single(a => a.Id == activityId);
        }

        /// <summary>Gets the outbound transitions of a given activity. </summary>
        /// <param name="activity">The activity. </param>
        /// <returns>The transitions. </returns>
        public WorkflowTransition[] GetOutboundTransitions(WorkflowActivityBase activity)
        {
            return Transitions.Where(t => t.From == activity.Id).ToArray();
        }

        /// <summary>Gets the inbound transitions of a given activity. </summary>
        /// <param name="activity">The activity. </param>
        /// <returns>The transitions. </returns>
        public WorkflowTransition[] GetInboundTransitions(WorkflowActivityBase activity)
        {
            return Transitions.Where(t => t.To == activity.Id).ToArray();
        }
    }
}