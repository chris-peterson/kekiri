using System.Dynamic;
using FluentAssertions;

namespace Kekiri.UnitTests.SharedSteps
{
    public class Can_use_steps_from_step_class : ScenarioTest
    {
        [Given]
        public void a_scenario()
        {
            Context.Scenario = new ExpandoObject();
        }

        When_a_step_is_referenced_as_a_field _;

        [Then]
        public void the_matching_step_class_is_executed()
        {
            bool isReferencedStepExecuted = Context.Scenario.IsReferencedStepExecuted;
            isReferencedStepExecuted.Should().BeTrue();
        }
    }

    internal class When_a_step_is_referenced_as_a_field : Step
    {
        public override void Execute()
        {
            Context.Scenario.IsReferencedStepExecuted = true;
        }
    }
}