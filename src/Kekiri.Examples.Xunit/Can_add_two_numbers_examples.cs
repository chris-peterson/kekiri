using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kekiri.TestRunner.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    [Example(1, 2, 3)]
    [Example(2, 3, 5)]
    public class Can_add_two_numbers_examples : Scenario
    {
        public Can_add_two_numbers_examples(int a, int b, int sum)
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