using Kekiri.TestRunner.NUnit;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit.Calculator.Subtraction
{
    [Example(12, 5, 7)]
    [Example(20, 5, 15)]
    public class Subtracting_two_numbers : Scenario
    {
        readonly Calculator _calculator = new Calculator();

        public Subtracting_two_numbers(double operand1, double operand2, double expectedResult)
        {
            Given(the_user_enters_OPERAND1, operand1)
                .And(the_user_enters_OPERAND2, operand2);
            When(subtracting);
            Then(the_result_is_EXPECTED, expectedResult);
        }

        void the_user_enters_OPERAND1(double operand1)
        {
            _calculator.Operand1 = operand1;
        }

        void the_user_enters_OPERAND2(double operand2)
        {
            _calculator.Operand2 = operand2;
        }

        void subtracting()
        {
            _calculator.Subtract();
        }

        void the_result_is_EXPECTED(double expected)
        {
            Assert.AreEqual(expected, _calculator.Result);
        }
    }

    class Calculator
    {
        public double Operand1 { get; set; }
        public double Operand2 { get; set; }

        public double Result { get; set; }

        public void Subtract()
        {
            Result = Operand1 - Operand2;
        }
    }
}