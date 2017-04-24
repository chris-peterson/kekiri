using System.Collections.Generic;
using System.Linq;
using Kekiri.TestRunner.xUnit;
using Xunit;

namespace Kekiri.Examples.xUnit
{
    public class AdditionScenarios : ExampleScenarios
    {
        [Scenario]
        public void Can_add_one_plus_two()
        {
            Given(a_number, 1)
            .And(another_number, 2);
            When(adding_them_up);
            Then(The_sum_is, 3);
        }

        [ScenarioOutline]
        [Example(1, 2, 3)]
        [Example(2, 3, 5)]
        public void Can_add_two_numbers(int a, int b, int sum)
        {
            Given(a_number, a)
                .And(another_number, b);
            When(adding_them_up);
            Then(The_sum_is, sum);
        }

        protected override void Before()
        {
            Context.Numbers = new List<int>();
        }

        private void a_number(int a)
        {
            Context.Numbers.Add(a);
        }

        private void another_number(int b)
        {
            Context.Numbers.Add(b);
        }

        private void adding_them_up()
        {
            List<int> numbers = Context.Numbers;
            Context.Sum = numbers.Sum();
        }

        private void The_sum_is(int sum)
        {
            Assert.Equal(sum, Context.Sum);
        }
    }
}