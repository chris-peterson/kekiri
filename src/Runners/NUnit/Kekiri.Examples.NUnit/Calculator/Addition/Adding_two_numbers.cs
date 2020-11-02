using Kekiri.NUnit;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit.Calculator.Addition
{
    class Adding_two_numbers : Scenarios
    {
        Calculator _calculator;

        [Scenario]
        public void Can_add_two_numbers()
        {
            Given(a_calculator)
               .And(the_user_enters_50)
               .And(the_user_enters_70);
            When(adding);
            Then(the_result_is_120);
        }

        void a_calculator()
        {
            _calculator = new Calculator();
        }

        void the_user_enters_50()
        {
            _calculator.Operand1 = 50;
        }

        void the_user_enters_70()
        {
            _calculator.Operand2 = 70;
        }

        void adding()
        {
            _calculator.Add();
        }

        void the_result_is_120()
        {
            Assert.AreEqual(120m, _calculator.Result);
        }
    }

    class Calculator
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Add()
        {
            Result = Operand1 + Operand2;
        }
    }
}