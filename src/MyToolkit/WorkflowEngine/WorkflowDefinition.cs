//-----------------------------------------------------------------------
// <copyright file="Workflow.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

// Workflow shapes: http://msdn.microsoft.com/en-us/library/ee264487(v=bts.10).aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using MyToolkit.Serialization;
using MyToolkit.WorkflowEngine.Exceptions;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Describes a workflow using activities and transitions. </summary>
    public class WorkflowDefinition
    {
        /// <summary>Gets or sets the ID of the first inputActivity. </summary>
        [XmlElement("StartActivity")]
        public string StartActivityId { get; set; }

        /// <summary>Gets or sets the workflow activities. </summary>
        [XmlArray("Activities")]
        [XmlArrayItem("Activity")]
        public List<object> RawActivities { get; set; }

        /// <summary>Gets or sets the workflow activities. </summary>
        [XmlIgnore]
        public List<IWorkflowActivityBase> Activities
        {
            get { return RawActivities.OfType<IWorkflowActivityBase>().ToList(); }
            set { RawActivities = value.OfType<object>().ToList(); }
        }

        /// <summary>Gets or sets the workflow transitions. </summary>
        [XmlArray]
        [XmlArrayItem("Transition")]
        public List<WorkflowTransition> Transitions { get; set; }

        /// <summary>Gets or sets the first inputActivity. </summary>
        /// <exception cref="WorkflowException" accessor="get">The activity could not be found. </exception>
        [XmlIgnore]
        public IWorkflowActivityBase StartActivity
        {
            get { return GetActivityById(StartActivityId); }
            set { StartActivityId = value.Id; }
        }

        /// <summary>Creates a workflow instance from a XML string. </summary>
        /// <param name="xml">The XML. </param>
        /// <param name="activityTypes">The possible inputActivity types. </param>
        /// <returns>The workflow. </returns>
        /// <exception cref="WorkflowException">The activity could not be found. </exception>
        public static WorkflowDefinition FromXml(string xml, Type[] activityTypes)
        {
            var workflow = XmlSerialization.Deserialize<WorkflowDefinition>(xml, activityTypes.ToArray());
            workflow.StartActivity = workflow.GetActivityById(workflow.StartActivityId);
            return workflow;
        }

        /// <summary>Serializes the workflow to XML. </summary>
        /// <returns>The XML. </returns>
        public string ToXml()
        {
            var extraTypes = Activities.Select(a => a.GetType());
            return XmlSerialization.Serialize(this, extraTypes.Distinct().ToArray());
        }

        /// <summary>Creates an workflow instance based on this workflow definition. </summary>
        /// <returns>The <see cref="WorkflowInstance"/>. </returns>
        /// <exception cref="WorkflowException">A workflow validation exception occurred. </exception>
        public WorkflowInstance CreateInstance()
        {
            ValidateRoutes();

            var instance = new WorkflowInstance(this, new List<ActivityData>());
            instance.CurrentActivityIds.Add(StartActivityId);
            return instance;
        }

        /// <summary>Gets an inputActivity by ID. </summary>
        /// <param name="activityId">The inputActivity ID. </param>
        /// <returns>The <see cref="IWorkflowActivityBase"/>. </returns>
        /// <exception cref="WorkflowException">The activity could not be found. </exception>
        public IWorkflowActivityBase GetActivityById(string activityId)
        {
            try
            {
                return Activities.Single(a => a.Id == activityId);
            }
            catch (InvalidOperationException ex)
            {
                throw new WorkflowException("The activity could not be found. ", ex);
            }
        }
        
        /// <summary>Gets the outbound transitions of a given inputActivity. </summary>
        /// <param name="activity">The inputActivity. </param>
        /// <returns>The transitions. </returns>
        public WorkflowTransition[] GetOutboundTransitions(IWorkflowActivityBase activity)
        {
            return Transitions.Where(t => t.From == activity.Id).ToArray();
        }

        /// <summary>Gets the inbound transitions of a given inputActivity. </summary>
        /// <param name="activity">The inputActivity. </param>
        /// <returns>The transitions. </returns>
        public WorkflowTransition[] GetInboundTransitions(IWorkflowActivityBase activity)
        {
            return Transitions.Where(t => t.To == activity.Id).ToArray();
        }

        /// <exception cref="WorkflowException">A workflow validation exception occurred. </exception>
        private void ValidateRoutes()
        {
            foreach (var activity in Activities)
            {
                foreach (var route in activity.Routes)
                    ValidateRoute(activity, route);
            }
        }

        /// <exception cref="WorkflowException">A workflow validation exception occurred. </exception>
        private void ValidateRoute(IWorkflowActivityBase inputActivity, WorkflowRoute route)
        {
            var outputActivity = GetActivityById(route.OutputActivityId);
            var outputProperty = outputActivity.OutputType.GetRuntimeProperty(route.OutputProperty);
            if (outputProperty == null)
                throw new WorkflowException("The output property of the route could not be found on the output data type. ");

            var inputProperty = inputActivity.InputType.GetRuntimeProperty(route.InputProperty);
            if (inputProperty == null)
                throw new WorkflowException("The input property of the route could not be found on the input data type. ");

            if (outputProperty.PropertyType != inputProperty.PropertyType)
                throw new WorkflowException("The input property and output property types of a route do not match. ");
        }
    }
}