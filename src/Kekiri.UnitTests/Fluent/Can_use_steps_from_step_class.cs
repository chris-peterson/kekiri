using System.Dynamic;
using FluentAssertions;

namespace Kekiri.UnitTests.Fluent
{
    public class Can_use_steps_from_step_class : ScenarioBase
    {
        public Can_use_steps_from_step_class()
        {
            Given(a_scenario);
            When<a_step_class_is_referenced>();
            Then(the_matching_step_class_is_executed);
        }

        private void a_scenario()
        {
            Context.Scenario = new ExpandoObject();
        }

        private void the_matching_step_class_is_executed()
        {
            bool isReferencedStepExecuted = Context.Scenario.IsReferencedStepExecuted;
            isReferencedStepExecuted.Should().BeTrue();
        }
    }

    internal class a_step_class_is_referenced : Step
    {
        public override void Execute()
        {
            Context.Scenario.IsReferencedStepExecuted = true;
        }
    }
}