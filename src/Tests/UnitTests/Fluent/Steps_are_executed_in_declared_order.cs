using FluentAssertions;

namespace Kekiri.UnitTests.Fluent
{
    public class Steps_are_executed_in_declared_order : FluentTest
    {
        protected override void Before()
        {
            Context.Message = "";
        }
        
        public Steps_are_executed_in_declared_order()
        {
           Given(step_1)
              .And<step_2>()
              .And(step_3);
            When(the_steps_are_executed);
            Then(they_are_executed_in_the_order_they_were_declared);
        }

        public void step_1()
        {
            Context.Message += "1";
        }

        public void step_3()
        {
            Context.Message += "3";
        }

        public void the_steps_are_executed()
        { }

        public void they_are_executed_in_the_order_they_were_declared()
        {
            string message = Context.Message;
            message.Should().Be("123");
        }
    }

    internal class step_2 : Step
    {
        public override void Execute()
        {
            Context.Message += "2";
        }
    }
}