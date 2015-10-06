//-----------------------------------------------------------------------
// <copyright file="WorkflowInstance.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MyToolkit.Model;
using MyToolkit.Serialization;
using MyToolkit.Utilities;
using MyToolkit.WorkflowEngine.Activities;
using MyToolkit.WorkflowEngine.Exceptions;

namespace MyToolkit.WorkflowEngine
{
    /// <summary>Represents an instance of a <see cref="WorkflowDefinition"/>. </summary>
    public class WorkflowInstance : ObservableObject
    {
        private bool _isRunning;
        private string[] _lastCurrentActivityIds;

        /// <summary>Deserializes a workflow instance from XML. </summary>
        /// <param name="xml">The XML as string. </param>
        /// <param name="workflowDefinition">The instance's workflow description. </param>
        /// <returns>The workflow instance. </returns>
        public static WorkflowInstance FromXml(string xml, WorkflowDefinition workflowDefinition)
        {
            var types = workflowDefinition.Activities.Select(a => a.GetType()).ToArray();
            var instance = new WorkflowInstance();
            instance.Data = XmlSerialization.Deserialize<List<ActivityData>>(xml, types);
            instance.WorkflowDefinition = workflowDefinition;
            return instance;
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowInstance"/> class. </summary>
        /// <param name="workflowDefinition">The workflow definition. </param>
        /// <param name="data">The data provider. </param>
        public WorkflowInstance(WorkflowDefinition workflowDefinition, List<ActivityData> data)
        {
            Data = data;
            WorkflowDefinition = workflowDefinition;
            CurrentActivityIds = new List<string>();
        }

        /// <summary>Used only for serialization. </summary>
        internal WorkflowInstance()
        {
        }

        /// <summary>Gets the data. </summary>
        public List<ActivityData> Data { get; private set; }

        /// <summary>Gets the instance's workflow description. </summary>
        public WorkflowDefinition WorkflowDefinition { get; private set; }

        /// <summary>Occurs when the current activities of the workflow instance changed. </summary>
        public event EventHandler<CurrentActivitiesChangedEventArgs> CurrentActivitiesChanged;

        /// <summary>Gets or sets a value indicating whether an activity of the instance is currently running. </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { Set(ref _isRunning, value); }
        }

        /// <summary>Gets the list of current activities. </summary>
        public IWorkflowActivityBase[] CurrentActivities
        {
            get
            {
                return WorkflowDefinition.Activities
                    .Where(a => CurrentActivityIds.Contains(a.Id))
                    .ToArray();
            }
        }

        /// <summary>Gets the first activity of the current activities or null of there are no more activities. </summary>
        public IWorkflowActivityBase NextActivity
        {
            get { return CurrentActivities.FirstOrDefault(); }
        }

        /// <summary>Gets the list of current activity ids. </summary>
        internal List<string> CurrentActivityIds { get; set; }

        /// <summary>Serializes the workflow instance to XML. </summary>
        /// <returns>The XML. </returns>
        public string ToXml()
        {
            return XmlSerialization.Serialize(Data, Data.Select(d => d.Output.GetType()).Distinct().ToArray());
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync(IWorkflowActivityBase activity)
        {
            return CompleteAsync<WorkflowActivityInput>(activity, CancellationToken.None, null);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync(IWorkflowActivityBase activity, CancellationToken cancellationToken)
        {
            return CompleteAsync<WorkflowActivityInput>(activity, cancellationToken, null);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync<TInput>(IWorkflowActivityBase activity, TInput input)
            where TInput : WorkflowActivityInput
        {
            return CompleteAsync(activity, CancellationToken.None, input);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        public async Task<WorkflowActivityOutput> CompleteAsync<TInput>(
            IWorkflowActivityBase activity, CancellationToken cancellationToken, TInput input)
            where TInput : WorkflowActivityInput
        {
            try
            {
                IsRunning = true;
                return await CompleteInternalAsync(activity, cancellationToken, input);
            }
            finally
            {
                IsRunning = false;
                RaiseCurrentActivitiesChanged();
            }
        }

        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        private async Task<WorkflowActivityOutput> CompleteInternalAsync<TInput>(
            IWorkflowActivityBase activity, CancellationToken cancellationToken, TInput input)
            where TInput : WorkflowActivityInput
        {
            if (CurrentActivityIds.Contains(activity.Id))
            {
                input = CreateActivityInput(activity, input);

                WorkflowActivityOutput result;
                try
                {
                    result = await activity.CompleteAsync(input, cancellationToken);
                }
                catch (Exception exception)
                {
                    return HandleCompletionException(exception);
                }

                if (result.Successful)
                    return await HandleCompletionSuccess(activity, result);
                else
                    return HandleCompletionFailed(result);
            }
            throw new WorkflowException("The activity to complete could not be found in the current activity list. ");
        }

        private static WorkflowActivityOutput HandleCompletionFailed(WorkflowActivityOutput result)
        {
            return result;
        }

        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        private static WorkflowActivityOutput HandleCompletionException(Exception exception)
        {
            if (exception != null)
                throw new WorkflowException("Error while completing an activity.", exception);

            return new WorkflowActivityOutput { Successful = false };
        }

        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        private async Task<WorkflowActivityOutput> HandleCompletionSuccess(IWorkflowActivityBase activity, WorkflowActivityOutput result)
        {
            AddActivityResultToData(activity, result);

            var nextActivities = result.GetNextActivities(activity, WorkflowDefinition);
            if (nextActivities == null)
                nextActivities = GetDefaultNextActivities(activity);

            if (!(activity is ForkActivity) && nextActivities.Length > 1)
            {
                throw new WorkflowException(string.Format("Activity ({0}) has multiple next activities ({1}) " +
                                                          "but only ForkActivity activities can return multiple activities. ",
                    activity.Id, string.Join(", ", nextActivities.Select(a => a.Id))));
            }

            CurrentActivityIds.Remove(activity.Id);

            await AddNextActivitiesAsync(activity, nextActivities);
            return result;
        }

        private void AddActivityResultToData(IWorkflowActivityBase activity, WorkflowActivityOutput result)
        {
            var data = Data.SingleOrDefault(d => d.ActivityId == activity.Id);
            if (data != null)
                Data.Remove(data);

            Data.Add(new ActivityData { ActivityId = activity.Id, Output = result });
        }

        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        private TInput CreateActivityInput<TInput>(IWorkflowActivityBase activity, TInput input)
            where TInput : WorkflowActivityInput
        {
            input = input ?? (TInput)Activator.CreateInstance(activity.InputType);
            input.Instance = this;
            foreach (var route in activity.Routes)
            {
                var outputData = Data.FirstOrDefault(d => d.ActivityId == route.OutputActivityId);
                if (outputData != null)
                {
                    var inputProperty = input.GetType().GetRuntimeProperty(route.InputProperty);
                    if (inputProperty != null)
                    {
                        var outputProperty = outputData.Output.GetType().GetRuntimeProperty(route.OutputProperty);
                        if (outputProperty != null)
                            inputProperty.SetValue(input, outputProperty.GetValue(outputData.Output));
                        else
                            throw new WorkflowException("The output property of the route could not be found on the output data. ");
                    }
                    else
                        throw new WorkflowException("The input property of the route could not be found on the input data. ");
                }
                else
                    throw new WorkflowException("The requested input data for the activity could not be found. ");
            }
            return input;
        }

        private void RaiseCurrentActivitiesChanged()
        {
            var currentActivitiesHasChanged = !CurrentActivityIds.IsCopyOf(_lastCurrentActivityIds);
            if (currentActivitiesHasChanged)
            {
                _lastCurrentActivityIds = CurrentActivityIds.ToArray();

                var copy = CurrentActivitiesChanged;
                if (copy != null)
                    copy(this, new CurrentActivitiesChangedEventArgs());

                RaisePropertyChanged(() => CurrentActivities);
                RaisePropertyChanged(() => NextActivity);
            }
        }

        /// <exception cref="WorkflowException">Default outgoing transitions of cannot be conditional. </exception>
        private IWorkflowActivityBase[] GetDefaultNextActivities(IWorkflowActivityBase activity)
        {
            var transitions = WorkflowDefinition.GetOutboundTransitions(activity).ToArray();
            if (transitions.Any(t => t.IsConditional))
                throw new WorkflowException(string.Format("Default outgoing transitions of ({0}) cannot be conditional. ", activity.Id));

            return transitions
                .Select(t => WorkflowDefinition.GetActivityById(t.To))
                .ToArray();
        }

        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        private async Task AddNextActivitiesAsync(IWorkflowActivityBase activity, IList<IWorkflowActivityBase> nextActivities)
        {
            var allowedTransitions = WorkflowDefinition.GetOutboundTransitions(activity);

            var hasFollowingActivities = allowedTransitions.Length > 0;
            if (!hasFollowingActivities)
                return;

            var areNextActivitiesAllowed = nextActivities.All(a => allowedTransitions.Any(t => t.To == a.Id));
            if (areNextActivitiesAllowed)
                await PrepareNextActivities(nextActivities);
            else
            {
                throw new WorkflowException(
                    string.Format("Transitions for activities ({0}) produced by activity ({1}) could not be found. ",
                        string.Join(", ", nextActivities.Select(a => a.Id)), activity.Id));
            }
        }

        /// <exception cref="WorkflowException">A workflow exception occurred. </exception>
        private async Task PrepareNextActivities(IEnumerable<IWorkflowActivityBase> nextActivities)
        {
            foreach (var nextActivity in nextActivities)
            {
                var nextActivityId = nextActivity.Id;
                if (!CurrentActivityIds.Contains(nextActivityId))
                    CurrentActivityIds.Add(nextActivityId);

                RaiseCurrentActivitiesChanged();

                var input = CreateActivityInput<WorkflowActivityInput>(nextActivity, null);
                var immediatelyExecuteActivity = await nextActivity.PrepareAsync(input, WorkflowDefinition);
                if (immediatelyExecuteActivity)
                    await CompleteInternalAsync<WorkflowActivityInput>(nextActivity, CancellationToken.None, null);
            }
        }
    }
}