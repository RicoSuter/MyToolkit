//-----------------------------------------------------------------------
// <copyright file="WorkflowInstance.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
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
        private string[] _lastCurrentActivityIds = null;

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
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync(IWorkflowActivityBase activity)
        {
            return CompleteAsync<WorkflowActivityInput>(activity.Id, CancellationToken.None, null);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync(string activityId)
        {
            return CompleteAsync<WorkflowActivityInput>(activityId, CancellationToken.None, null);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync(IWorkflowActivityBase activity, CancellationToken cancellationToken)
        {
            return CompleteAsync<WorkflowActivityInput>(activity.Id, cancellationToken, null);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync(string activityId, CancellationToken cancellationToken)
        {
            return CompleteAsync<WorkflowActivityInput>(activityId, cancellationToken, null);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync<T>(IWorkflowActivityBase activity, Action<T> initializeInput)
            where T : WorkflowActivityInput
        {
            return CompleteAsync(activity.Id, CancellationToken.None, initializeInput);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync<T>(IWorkflowActivityBase activity, CancellationToken cancellationToken, 
            Action<T> initializeInput)
            where T : WorkflowActivityInput
        {
            return CompleteAsync(activity.Id, cancellationToken, initializeInput);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityOutput> CompleteAsync<T>(string activityId, Action<T> initializeInput)
            where T : WorkflowActivityInput
        {
            return CompleteAsync(activityId, CancellationToken.None, initializeInput);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public async Task<WorkflowActivityOutput> CompleteAsync<T>(string activityId, CancellationToken cancellationToken, Action<T> initializeInput)
            where T : WorkflowActivityInput
        {
            try
            {
                IsRunning = true;
                return await CompleteInternalAsync(activityId, cancellationToken, initializeInput);
            }
            finally
            {
                IsRunning = false;
                RaiseCurrentActivitiesChanged();
            }
        }

        /// <exception cref="WorkflowException">Activity has multiple next activities but only ForkActivity activities can return multiple activities. </exception>
        private async Task<WorkflowActivityOutput> CompleteInternalAsync<T>(string activityId, CancellationToken cancellationToken, Action<T> initializeInput)
            where T : WorkflowActivityInput
        {
            if (CurrentActivityIds.Contains(activityId))
            {
                var activity = WorkflowDefinition.GetActivityById(activityId);
                var allowedTransitions = WorkflowDefinition.GetOutboundTransitions(activity);

                var input = CreateActivityInput(activity, initializeInput);
                var result = await activity.CompleteAsync(input, cancellationToken);
                if (result.Successful)
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

                    CurrentActivityIds.Remove(activityId);

                    await AddNextActivitiesAsync(activityId, nextActivities, allowedTransitions);
                    return result;
                }
                else
                {
                    // TODO: Workflow => What if not successful
                }
            }
            throw new WorkflowException("The activity to complete could not be found in the current activity list. ");
        }

        private void AddActivityResultToData(IWorkflowActivityBase activity, WorkflowActivityOutput result)
        {
            var data = Data.SingleOrDefault(d => d.ActivityId == activity.Id);
            if (data != null)
                Data.Remove(data);

            Data.Add(new ActivityData { ActivityId = activity.Id, Output = result });
        }

        /// <exception cref="WorkflowException">The requested input data for the activity could not be found. </exception>
        private WorkflowActivityInput CreateActivityInput<T>(IWorkflowActivityBase activity, Action<T> initializeInput)
            where T : WorkflowActivityInput
        {
            var input = (WorkflowActivityInput)Activator.CreateInstance(activity.InputType);
            input.Instance = this;
            foreach (var route in activity.Routes)
            {
                var outputData = Data.FirstOrDefault(d => d.ActivityId == route.OutputActivityId);
                if (outputData != null)
                {
                    var inputProperty = input.GetType().GetRuntimeProperty(route.InputProperty);
                    var outputProperty = outputData.Output.GetType().GetRuntimeProperty(route.OutputProperty);

                    inputProperty.SetValue(input, outputProperty.GetValue(outputData.Output));
                }
                else
                    throw new WorkflowException("The requested input data for the activity could not be found. ");
            }

            if (initializeInput != null)
                initializeInput((T)input);

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

        /// <exception cref="WorkflowException">Transitions for activities ({0}) produced by activity could not be found. </exception>
        private async Task AddNextActivitiesAsync(string activityId, IList<IWorkflowActivityBase> nextActivities, WorkflowTransition[] allowedTransitions)
        {
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
                        string.Join(", ", nextActivities.Select(a => a.Id)), activityId));
            }
        }

        /// <exception cref="WorkflowException">Activity has multiple next activities but only ForkActivity activities can return multiple activities. </exception>
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
                    await CompleteInternalAsync<WorkflowActivityInput>(nextActivityId, CancellationToken.None, null);
            }
        }
    }
}