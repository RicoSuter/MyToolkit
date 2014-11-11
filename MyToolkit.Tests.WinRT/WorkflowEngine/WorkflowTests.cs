using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.WorkflowEngine;
using MyToolkit.WorkflowEngine.Activities;

namespace MyToolkit.Tests.WinRT.WorkflowEngine
{
    [TestClass]
    public class WorkflowTests
    {
        private static readonly Type[] _activityTypes = { typeof(MockActivity) };

        [TestMethod]
        public void When_serializing_workfow_then_deserialization_should_work()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();

            //// Act
            var workflowXml = workflow.ToXml();
            var workflow2 = WorkflowDefinition.FromXml(workflowXml, _activityTypes);
            var workflowXml2 = workflow2.ToXml();

            //// Assert
            Assert.AreEqual(workflowXml, workflowXml2);
        }

        [TestMethod]
        public async Task When_serializing_workflow_instance_then_deserialization_should_work()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();
            var instance = workflow.CreateInstance();

            //// Act
            var activity = instance.CurrentActivities.First();
            var result = await instance.CompleteAsync(activity);

            var instanceXml = instance.ToXml();
            var instance2 = WorkflowInstance.FromXml(instanceXml, workflow);
            var instanceXml2 = instance2.ToXml();

            //// Assert
            Assert.AreEqual(instanceXml, instanceXml2);
        }

        [TestMethod]
        public void When_creating_instance_then_first_activity_is_start_activity()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();

            //// Act
            var instance = workflow.CreateInstance();

            //// Assert
            Assert.AreEqual(1, instance.CurrentActivities.Length);
            Assert.AreEqual(workflow.StartActivity, instance.CurrentActivities.First());
        }

        [TestMethod]
        public void When_getting_outbound_transitions_then_the_properties_should_be_correct()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();

            //// Act
            var transitions = workflow.GetOutboundTransitions(workflow.Activities.First());

            //// Assert
            Assert.AreEqual(1, transitions.Length);
            Assert.AreEqual(workflow.Activities[0].Id, transitions[0].From);
            Assert.AreEqual(workflow.Activities[1].Id, transitions[0].To);
        }

        [TestMethod]
        public void When_getting_inbound_transitions_then_the_properties_should_be_correct()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();

            //// Act
            var transitions = workflow.GetInboundTransitions(workflow.Activities[1]);

            //// Assert
            Assert.AreEqual(1, transitions.Length);
            Assert.AreEqual(workflow.Activities[0].Id, transitions[0].From);
            Assert.AreEqual(workflow.Activities[1].Id, transitions[0].To);
        }

        [TestMethod]
        public async Task When_executing_all_activities_then_no_more_activities()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();
            var instance = workflow.CreateInstance();

            //// Act
            await instance.CompleteAsync(instance.CurrentActivities.First());
            await instance.CompleteAsync(instance.CurrentActivities.First());
            await instance.CompleteAsync(instance.CurrentActivities.First());

            //// Assert
            Assert.AreEqual(0, instance.CurrentActivities.Length);
        }

        [TestMethod]
        public async Task When_executing_task_then_changed_event_must_be_called()
        {
            //// Arrange
            var changes = 0;
            var workflow = Given_a_workflow_with_three_serial_empty_activities();
            var instance = workflow.CreateInstance();
            instance.CurrentActivitiesChanged += (sender, args) => { changes++; };

            //// Act
            await instance.CompleteAsync(instance.CurrentActivities.First());
            await instance.CompleteAsync(instance.CurrentActivities.First());
            await instance.CompleteAsync(instance.CurrentActivities.First());

            //// Assert
            Assert.AreEqual(3, changes);
        }

        [TestMethod]
        public async Task When_setting_data_from_activity_argument_then_they_should_be_written()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_three_serial_empty_activities();
            var instance = workflow.CreateInstance();

            //// Act
            var activity = instance.CurrentActivities.First();
            await instance.CompleteAsync(activity);

            //// Assert
            var data = instance.Data.Single(d => d.ActivityId == activity.Id);
            Assert.AreEqual("Hello", ((MyOutput)data.Output).Output);
        }

        [TestMethod]
        public async Task When_joining_activities_then_join_is_automatically_completed_when_everything_inbound_is_complete()
        {
            //// Arrange
            var workflow = Given_a_workflow_with_fork_and_join();
            var instance = workflow.CreateInstance();

            //// Act
            await instance.CompleteAsync(instance.CurrentActivities.First());

            var activities = instance.CurrentActivities;
            Assert.AreEqual(2, activities.Length);

            await instance.CompleteAsync(activities[0]);
            await instance.CompleteAsync(activities[1]);

            // JoinActivity should now be completed

            //// Assert
            Assert.AreEqual(0, instance.CurrentActivities.Length);
        }

        private static WorkflowDefinition Given_a_workflow_with_three_serial_empty_activities()
        {
            var workflow = new WorkflowDefinition();
            workflow.Activities = new List<IWorkflowActivityBase>
            {
                new MockActivity { Id = "1", DefaultInput = "Hello" },
                new MockActivity 
                {
                    Id = "2", 
                    Routes = { WorkflowRoute.Create<MyOutput, MyInput>("1", a => a.Output, a => a.Input) }
                },
                new MockActivity { Id = "3" },
            };
            workflow.StartActivity = workflow.Activities.First();
            workflow.Transitions = new List<WorkflowTransition>
            {
                new WorkflowTransition { From = "1", To = "2" },
                new WorkflowTransition { From = "2", To = "3" },
            };
            return workflow;
        }

        private static WorkflowDefinition Given_a_workflow_with_fork_and_join()
        {
            var workflow = new WorkflowDefinition();
            workflow.Activities = new List<IWorkflowActivityBase>
            {
                new ForkActivity { Id = "1" },
                new MockActivity { Id = "2" },
                new MockActivity { Id = "3" },
                new JoinActivity { Id = "4" },
            };
            workflow.StartActivity = workflow.Activities.First();
            workflow.Transitions = new List<WorkflowTransition>
            {
                new WorkflowTransition { From = "1", To = "2" },
                new WorkflowTransition { From = "1", To = "3" },
                new WorkflowTransition { From = "2", To = "4" },
                new WorkflowTransition { From = "3", To = "4" },
            };
            return workflow;
        }

        // TODO: Use this
        private static WorkflowDefinition Given_a_workflow_with_invalid_fork_and_join()
        {
            var workflow = new WorkflowDefinition();
            workflow.Activities = new List<IWorkflowActivityBase>
            {
                new ForkActivity { Id = "1" },
                new MockActivity { Id = "2" },
                new MockActivity { Id = "3" },
                new JoinActivity { Id = "4" },
            };
            workflow.StartActivity = workflow.Activities.First();
            workflow.Transitions = new List<WorkflowTransition>
            {
                new WorkflowTransition { From = "1", To = "2" },
                new WorkflowTransition { From = "1", To = "3" },
                new WorkflowTransition { From = "2", To = "3" },
                new WorkflowTransition { From = "2", To = "4" },
                new WorkflowTransition { From = "3", To = "4" },
            };
            return workflow;
        }

        [XmlType("MockActivity")]
        [XmlInclude(typeof(MyInput))]
        [XmlInclude(typeof(MyOutput))]
        public class MockActivity : WorkflowActivityBase<MyInput, MyOutput>
        {
            public string DefaultInput { get; set; }
            
            /// <summary>Completes the activity. </summary>
            /// <param name="input">The input. </param>
            /// <param name="cancellationToken">The cancellation token. </param>
            /// <returns>True when the activity has been completed. </returns>
            public override Task<MyOutput> CompleteAsync(MyInput input, CancellationToken cancellationToken)
            {
                return Task.FromResult(new MyOutput
                {
                    Successful = true,
                    Output = input.Input ?? DefaultInput
                });
            }
        }

        public class MyInput : WorkflowActivityInput
        {
            [XmlElement]
            public string Input { get; set; }
        }

        public class MyOutput : WorkflowActivityOutput
        {
            [XmlElement]
            public string Output { get; set; }
        }
    }
}
