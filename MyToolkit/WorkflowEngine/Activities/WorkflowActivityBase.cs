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
    /// <summary>The base workflow activity class. </summary>
    [XmlInclude(typeof(EmptyActivity))]
    [XmlInclude(typeof(ForkActivity))]
    [XmlInclude(typeof(JoinActivity))]
    [XmlInclude(typeof(AutomaticWorkflowActivityBase))]
    public abstract class WorkflowActivityBase
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowActivityBase"/> class. </summary>
        protected WorkflowActivityBase()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>Gets or sets the id of the activity. </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>Gets or sets the description. </summary>
        [XmlElement]
        public string Description { get; set; }

        /// <summary>Gets or sets the used data group. </summary>
        [XmlAttribute]
        public string Group { get; set; }

        /// <summary>Gets or sets the x position in the designer. </summary>
        [XmlAttribute]
        public double PositionX { get; set; }

        /// <summary>Gets or sets the y position in the designer. </summary>
        [XmlAttribute]
        public double PositionY { get; set; }

        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="instance">The workflow instance. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        public virtual async Task<bool> PrepareAsync(WorkflowInstance instance)
        {
            return false;
        }

        /// <summary>Completes the activity. </summary>
        /// <param name="instance">The workflow instance. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <param name="args">The execution args. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>True when the activity has been completed. </returns>
        public virtual async Task<WorkflowActivityResult> CompleteAsync(WorkflowInstance instance, WorkflowDefinition definition, WorkflowActivityArguments args, CancellationToken cancellationToken)
        {
            return new WorkflowActivityResult(true);
        }
    }
}