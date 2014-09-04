//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyToolkit.WorkflowEngine.Activities
{
    [XmlInclude(typeof(EmptyActivity))]
    [XmlInclude(typeof(ForkActivity))]
    [XmlInclude(typeof(JoinActivity))]
    [XmlInclude(typeof(AutomaticWorkflowActivityBase))]
    public abstract class WorkflowActivityBase
    {
        protected WorkflowActivityBase()
        {
            Id = Guid.NewGuid().ToString();
        }

        [XmlAttribute]
        public string Id { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlAttribute]
        public string Group { get; set; }

        [XmlAttribute]
        public double PositionX { get; set; }

        [XmlAttribute]
        public double PositionY { get; set; }

        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="instance">The workflow instance. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        public virtual Task<bool> PrepareAsync(WorkflowInstance instance)
        {
            return Task.FromResult(false);
        }

        /// <summary>Completes the activity. </summary>
        /// <param name="instance">The workflow instance. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <param name="args">The execution args. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>True when the activity has been completed. </returns>
        public virtual Task<WorkflowActivityResult> CompleteAsync(WorkflowInstance instance, WorkflowDefinition definition, WorkflowActivityArguments args, CancellationToken cancellationToken)
        {
            return Task.FromResult(new WorkflowActivityResult(true));
        }
    }
}