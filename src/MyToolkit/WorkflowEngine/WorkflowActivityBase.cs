//-----------------------------------------------------------------------
// <copyright file="WorkflowActivityBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MyToolkit.WorkflowEngine.Activities;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>The base workflow activity class with no input and output types. </summary>
    public class WorkflowActivityBase : WorkflowActivityBase<WorkflowActivityInput, WorkflowActivityOutput>
    {
        
    }

    /// <summary>The base workflow activity class. </summary>
    [XmlInclude(typeof(EmptyActivity))]
    [XmlInclude(typeof(ForkActivity))]
    [XmlInclude(typeof(JoinActivity))]
    [XmlInclude(typeof(AutomaticWorkflowActivityBase))]
    public abstract class WorkflowActivityBase<TInputType, TOutputType> : IWorkflowActivityBase
        where TInputType : WorkflowActivityInput, new()
        where TOutputType : WorkflowActivityOutput, new()
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowActivityBase{TInputType,TOutputType}"/> class. </summary>
        protected WorkflowActivityBase()
        {
            Id = Guid.NewGuid().ToString();
            Routes = new List<WorkflowRoute>();
        }

        /// <summary>Gets or sets the id of the activity. </summary>
        [XmlAttribute]
        public string Id { get; set; }

        /// <summary>Gets the type of the input.</summary>
        [XmlIgnore]
        public Type InputType
        {
            get { return typeof(TInputType); }
        }

        /// <summary>Gets the type of the output.</summary>
        [XmlIgnore]
        public Type OutputType
        {
            get { return typeof(TOutputType); }
        }

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

        /// <summary>Gets or sets the routes. </summary>
        [XmlArray]
        [XmlArrayItem("Route")]
        public List<WorkflowRoute> Routes { get; set; }

        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="input">The input. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        public virtual Task<bool> PrepareAsync(TInputType input)
        {
            return Task.FromResult(false);
        }

        /// <summary>Called when the previous activity has been executed. 
        /// The method may be called multiple times when there are multiple incoming transitions. </summary>
        /// <param name="input">The input. </param>
        /// <param name="definition">The workflow definition. </param>
        /// <returns>True when the activity should be automatically and immediately executed (with no args). </returns>
        internal virtual Task<bool> PrepareAsync(TInputType input, WorkflowDefinition definition)
        {
            return PrepareAsync(input);
        }

        /// <summary>Completes the activity. </summary>
        /// <param name="input">The input. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>True when the activity has been completed. </returns>
        public virtual Task<TOutputType> CompleteAsync(TInputType input, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TOutputType { Successful = true });
        }

        async Task<WorkflowActivityOutput> IWorkflowActivityBase.CompleteAsync(WorkflowActivityInput input, CancellationToken cancellationToken)
        {
            return await CompleteAsync((TInputType)input, cancellationToken);
        }

        Task<bool> IWorkflowActivityBase.PrepareAsync(WorkflowActivityInput input, WorkflowDefinition definition)
        {
            return PrepareAsync((TInputType)input, definition);
        }
    }
}