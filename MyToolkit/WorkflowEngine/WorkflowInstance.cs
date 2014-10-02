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
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
        /// <param name="dataTypes">All possible data container types. </param>
        /// <returns>The workflow instance. </returns>
        public static WorkflowInstance FromXml(string xml, WorkflowDefinition workflowDefinition, Type[] dataTypes)
        {
            dataTypes = dataTypes.Union(new[] {typeof (WorkflowInstanceData)}).ToArray();

            var instance = new WorkflowInstance();
            instance.Data = XmlSerialization.Deserialize<WorkflowDataProvider>(xml, dataTypes);
            instance.WorkflowDefinition = workflowDefinition;
            return instance;
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowInstance"/> class. </summary>
        /// <param name="workflowDefinition">The workflow definition. </param>
        /// <param name="dataProvider">The data provider. </param>
        public WorkflowInstance(WorkflowDefinition workflowDefinition, WorkflowDataProvider dataProvider)
        {
            Data = dataProvider;
            WorkflowDefinition = workflowDefinition;
        }

        /// <summary>Used only for serialization. </summary>
        internal WorkflowInstance()
        {
        }

        /// <summary>Gets the instance's workflow description. </summary>
        public WorkflowDefinition WorkflowDefinition { get; private set; }

        /// <summary>Gets or sets the data provider of the workflow instance. </summary>
        public WorkflowDataProvider Data { get; set; }

        /// <summary>Occurs when the current activities of the workflow instance changed. </summary>
        public event EventHandler<CurrentActivitiesChangedEventArgs> CurrentActivitiesChanged;

        /// <summary>Gets or sets a value indicating whether an activity of the instance is currently running. </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { Set(ref _isRunning, value); }
        }

        /// <summary>Gets the list of current activities. </summary>
        public WorkflowActivityBase[] CurrentActivities
        {
            get
            {
                return WorkflowDefinition.Activities
                    .Where(a => CurrentActivityIds.Contains(a.Id))
                    .ToArray();
            }
        }

        /// <summary>Gets the first activity of the current activities or null of there are no more activities. </summary>
        public WorkflowActivityBase NextActivity
        {
            get { return CurrentActivities.FirstOrDefault(); }
        }

        /// <summary>Gets the list of current activity ids. </summary>
        internal List<string> CurrentActivityIds
        {
            get { return Data.ResolveInstanceData().CurrentActivityIds; }
        }

        /// <summary>Serializes the workflow instance to XML. </summary>
        /// <returns>The XML. </returns>
        public string ToXml()
        {
            return XmlSerialization.Serialize(Data, Data.Data.Select(d => d.GetType()).Distinct().ToArray());
        }

        /// <summary>Resolves the requested data object. </summary>
        /// <typeparam name="T">The type of the data object. </typeparam>
        /// <param name="activity">The current activity to load the used data group from. </param>
        /// <returns>The data object. </returns>
        public T ResolveData<T>(WorkflowActivityBase activity) where T : ActivityDataBase, new()
        {
            return Data.Resolve<T>(activity);
        }

        /// <summary>Resolves the requested data object. </summary>
        /// <typeparam name="T">The type of the data object. </typeparam>
        /// <param name="group">The data group. Only use activity properties as parameter (no hard-coded strings). </param>
        /// <returns>The data object. </returns>
        public T ResolveData<T>(string group) where T : ActivityDataBase, new()
        {
            return Data.Resolve<T>(group);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityResult> CompleteAsync(WorkflowActivityBase activity, params object[] args)
        {
            return CompleteAsync(activity.Id, CancellationToken.None, args);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityResult> CompleteAsync(WorkflowActivityBase activity, CancellationToken cancellationToken, params object[] args)
        {
            return CompleteAsync(activity.Id, cancellationToken, args);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public Task<WorkflowActivityResult> CompleteAsync(string activityId, params object[] args)
        {
            return CompleteAsync(activityId, CancellationToken.None, args);
        }

        /// <summary>Executes the given activity with the given arguments. </summary>
        /// <exception cref="WorkflowException">Thrown when execution failed. </exception>
        public async Task<WorkflowActivityResult> CompleteAsync(string activityId, CancellationToken cancellationToken, params object[] args)
        {
            try
            {
                IsRunning = true;
                return await CompleteInternalAsync(activityId, cancellationToken, args);
            }
            finally
            {
                IsRunning = false;
                RaiseCurrentActivitiesChanged();
            }
        }

        private async Task<WorkflowActivityResult> CompleteInternalAsync(string activityId, CancellationToken cancellationToken, object[] args)
        {
            if (CurrentActivityIds.Contains(activityId))
            {
                var activity = WorkflowDefinition.GetActivityById(activityId);
                var allowedTransitions = WorkflowDefinition.GetOutboundTransitions(activity);

                var argsContainer = new WorkflowActivityArguments(args);
                var result = await activity.CompleteAsync(Data, WorkflowDefinition, argsContainer, cancellationToken);
                if (result.Successful)
                {
                    var nextActivities = result.NextActivities;
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
            }

            return new WorkflowActivityResult(false);
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

        private WorkflowActivityBase[] GetDefaultNextActivities(WorkflowActivityBase activity)
        {
            return WorkflowDefinition.GetOutboundTransitions(activity)
                .Select(t => WorkflowDefinition.GetActivityById(t.To))
                .ToArray();
        }

        private async Task AddNextActivitiesAsync(string activityId, IList<WorkflowActivityBase> nextActivities, WorkflowTransition[] allowedTransitions)
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

        private async Task PrepareNextActivities(IEnumerable<WorkflowActivityBase> nextActivities)
        {
            foreach (var nextActivity in nextActivities)
            {
                var nextActivityId = nextActivity.Id;
                if (!CurrentActivityIds.Contains(nextActivityId))
                    CurrentActivityIds.Add(nextActivityId);

                RaiseCurrentActivitiesChanged();

                var immediatelyExecuteActivity = await nextActivity.PrepareAsync(Data, WorkflowDefinition);
                if (immediatelyExecuteActivity)
                    await CompleteInternalAsync(nextActivityId, CancellationToken.None, new object[] { });
            }
        }
    }
}