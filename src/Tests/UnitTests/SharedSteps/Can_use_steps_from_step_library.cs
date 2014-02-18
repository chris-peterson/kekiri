using System.Dynamic;
using FluentAssertions;

namespace Kekiri.UnitTests.SharedSteps
{
    public class Can_use_steps_from_step_library : ScenarioTest
    {
        [Given]
        public void a_scenario()
        {
            Context.Scenario = new ExpandoObject();
        }

        When a_step_is_referenced_as_a_field_CAse_InSensitIVe;

        [Then]
        public void the_matching_step_is_executed_from_the_step_library()
        {
            bool isReferencedStepExecuted = Context.Scenario.IsReferencedStepExecuted;
            isReferencedStepExecuted.Should().BeTrue();
        }
    }

    internal class SomeSharedSteps : StepLibrary
    {
        [When]
        public void a_step_is_referenced_as_a_field_case_insensitive()
        {
            Context.Scenario.IsReferencedStepExecuted = true;
        }
    }
}