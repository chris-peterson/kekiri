using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kekiri.TestRunner.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    public class AdditionScenarios : Scenarios
    {
        [Scenario]
        public void Can_add_two_numbers()
        {
            Given(a_one)
            .And(a_two);
            When(adding_them_up);
            Then(The_sum_is_three);
        }

        protected override void Before()
        {
            Context.Numbers = new List<int>();
        }

        private void a_one()
        {
            Console.WriteLine("a one");
            Context.Numbers.Add(1);
        }

        private void a_two()
        {
            Context.Numbers.Add(2);
        }

        private void adding_them_up()
        {
            List<int> numbers = Context.Numbers;
            Context.Sum = numbers.Sum();
        }

        private void The_sum_is_three()
        {
            Assert.Equal(3, Context.Sum);
        }
    }
}