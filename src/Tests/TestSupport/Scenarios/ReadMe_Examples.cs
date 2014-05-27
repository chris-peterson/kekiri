using System;
using Kekiri.TestSupport.Scenarios.Reporting;
using NUnit.Framework;

namespace Kekiri.TestSupport.Scenarios
{
    public class Calculator
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Add()
        {
            Result = Operand1 + Operand2;
        }

        public void Divide()
        {
            Result = Operand1/Operand2;
        }
    }

    [Scenario(Feature.TestSupport)]
    class Adding_two_numbers : Test
    {
        private Calculator _calculator;

        [Given]
        public void Given_a_calculator()
        {
            _calculator = new Calculator();
        }

        [Given]
        public void The_user_enters_50()
        {
            _calculator.Operand1 = 50;
        }

        [Given]
        public void Next_the_user_enters_70()
        {
            _calculator.Operand2 = 70;
        }

        [When]
        public void When_the_user_presses_add()
        {
            _calculator.Add();
        }

        [Then]
        public void The_screen_should_display_result_of_120()
        {
            Assert.AreEqual(120, _calculator.Result);
        }
    }

    class When_dividing_by_zero : Test
    {
        readonly Calculator _calculator = new Calculator();

        [When, Throws]
        public void When()
        {
            _calculator.Divide();
        }

        [Then]
        public void It_should_throw_an_exception()
        {
            Catch<DivideByZeroException>();
        }
    }
}