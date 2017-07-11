using System.Collections.Generic;
using System.Linq;
using Kekiri.TestRunner.xUnit;
using Xunit;

namespace Kekiri.Examples.xUnit
{
    public class Scenario_with_typed_context : Scenarios<MyContext>
    {
        [Scenario]
        public void Can_add_one_plus_two()
        {
            Given(a_number, 1)
                .And(another_number, 2);
            When(adding_them_up);
            Then(The_sum_is, 3);
        }

        private void a_number(int a)
        {
            Context.Value1 = a;
        }

        private void another_number(int b)
        {
            Context.Value2 = b;
        }

        private void adding_them_up()
        {
            Context.Sum = Context.Value1 + Context.Value2;
        }

        private void The_sum_is(int sum)
        {
            Assert.Equal(sum, Context.Sum);
        }
    }

    public class MyContext
    {
        public int Value1 { get; set; }

        public int Value2 { get; set; }

        public int Sum { get; set; }
    }
}