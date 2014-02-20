using FluentAssertions;

namespace Kekiri.UnitTests.SharedSteps
{
    public class Can_share_strongly_typed_context_between_steps : ScenarioTest<StronglyTypedContext>
    {
        [Given]
        public void a_scenario()
        {
            //no-op
        }

        When_a_step_is_referenced_that_uses_strongly_typed_context _;

        [Then]
        public void the_matching_step_is_executed_from_the_step_library()
        {
            Context.IsReferencedStepExecuted.Should().BeTrue();
        }
    }

    internal class When_a_step_is_referenced_that_uses_strongly_typed_context : Step<StronglyTypedContext>
    {
        public override void Execute()
        {
            Context.IsReferencedStepExecuted = true;
        }
    }

    public class StronglyTypedContext
    {
        public bool IsReferencedStepExecuted { get; set; }
    }
}