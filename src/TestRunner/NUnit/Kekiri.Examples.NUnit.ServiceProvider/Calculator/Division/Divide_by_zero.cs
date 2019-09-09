using System;
using Kekiri.NUnit;

namespace Kekiri.Examples.NUnit.Calculator.Division
{
    class Divide_by_zero : Scenarios
    {
        readonly Calculator _calculator = new Calculator();

        [Scenario]
        public void If_divide_by_zero()
        {
            Given(a_denominator_of_0);
            When(dividing).Throws();
            Then(an_exception_is_raised);
        }

        void a_denominator_of_0()
        {
            _calculator.Operand2 = 0;
        }

        void dividing()
        {
            _calculator.Divide();
        }

        void an_exception_is_raised()
        {
            Catch<DivideByZeroException>();
        }
    }

    class Calculator
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Divide()
        {
            Result = Operand1/Operand2;
        }
    }
}