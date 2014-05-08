using FluentAssertions;

namespace Kekiri.UnitTests.Fluent
{
    public class Can_share_strongly_typed_context_between_steps : FluentScenario<StronglyTypedContext>
    {
        public Can_share_strongly_typed_context_between_steps()
        {
            Given(a_scenario);
            When<a_step_is_referenced_that_uses_strongly_typed_context>();
            Then(the_matching_step_is_executed_from_the_step_library);
        }

        private void a_scenario()
        {
            //no-op
        }

        private void the_matching_step_is_executed_from_the_step_library()
        {
            Context.IsReferencedStepExecuted.Should().BeTrue();
        }
    }

    internal class a_step_is_referenced_that_uses_strongly_typed_context : Step<StronglyTypedContext>
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