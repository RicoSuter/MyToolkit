//-----------------------------------------------------------------------
// <copyright file="FileName.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using MyToolkit.Utilities;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>A route connects the output of an activity with the input of an activity. </summary>
    public class WorkflowRoute
    {
        /// <summary>Initializes a new instance of the <see cref="WorkflowRoute"/> class. </summary>
        internal WorkflowRoute()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowRoute"/> class. </summary>
        /// <param name="outputActivity">The output activity. </param>
        /// <param name="outputProperty">The output property defined on the output activity's output type. </param>
        /// <param name="inputProperty">The input property defined on the input activity's input type. </param>
        public WorkflowRoute(IWorkflowActivityBase outputActivity, string outputProperty, string inputProperty)
        {
            OutputActivityId = outputActivity.Id;
            OutputProperty = outputProperty;
            InputProperty = inputProperty;
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowRoute"/> class. </summary>
        /// <param name="outputActivityId">The output activity ID. </param>
        /// <param name="outputProperty">The output property defined on the output activity's output type. </param>
        /// <param name="inputProperty">The input property defined on the input activity's input type. </param>
        public WorkflowRoute(string outputActivityId, string outputProperty, string inputProperty)
        {
            OutputActivityId = outputActivityId;
            OutputProperty = outputProperty;
            InputProperty = inputProperty;
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowRoute"/> class. </summary>
        /// <typeparam name="TOutputActivity">The type of the output activity's output. </typeparam>
        /// <typeparam name="TInputActivity">The type of the input activity's input. </typeparam>
        /// <param name="outputActivity">The output activity. </param>
        /// <param name="outputPropertyExpression">The output property as expression. </param>
        /// <param name="inputPropertyExpression">The output property as expression. </param>
        /// <returns>The <see cref="WorkflowRoute"/>. </returns>
        public static WorkflowRoute Create<TOutputActivity, TInputActivity>(IWorkflowActivityBase outputActivity, 
            Expression<Func<TOutputActivity, object>> outputPropertyExpression,
            Expression<Func<TInputActivity, object>> inputPropertyExpression)
            where TInputActivity : WorkflowActivityInput
            where TOutputActivity : WorkflowActivityOutput
        {
            return Create(outputActivity.Id, outputPropertyExpression, inputPropertyExpression);
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowRoute"/> class. </summary>
        /// <typeparam name="TOutputActivity">The type of the output activity's output. </typeparam>
        /// <typeparam name="TInputActivity">The type of the input activity's input. </typeparam>
        /// <param name="outputActivityId">The output activity ID. </param>
        /// <param name="outputPropertyExpression">The output property as expression. </param>
        /// <param name="inputPropertyExpression">The output property as expression. </param>
        /// <returns>The <see cref="WorkflowRoute"/>. </returns>
        public static WorkflowRoute Create<TOutputActivity, TInputActivity>(string outputActivityId,
            Expression<Func<TOutputActivity, object>> outputPropertyExpression,
            Expression<Func<TInputActivity, object>> inputPropertyExpression)
            where TInputActivity : WorkflowActivityInput
            where TOutputActivity : WorkflowActivityOutput
        {
            return new WorkflowRoute(outputActivityId,
                ExpressionUtilities.GetPropertyName(outputPropertyExpression), 
                ExpressionUtilities.GetPropertyName(inputPropertyExpression));
        }

        /// <summary>Gets the output activity ID. </summary>
        public string OutputActivityId { get; set; }

        /// <summary>Gets output property. </summary>
        public string OutputProperty { get; set; }

        /// <summary>Gets input property. </summary>
        public string InputProperty { get; set; }
    }
}