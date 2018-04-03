using System;
using Kekiri.NUnit;
using NUnit.Framework;
using ScenarioAttribute = Kekiri.NUnit.ScenarioAttribute;

namespace Kekiri.Examples.NUnit.Calculator
{
    [Feature]
    public class Calculator : FeatureBase
    {
        Calculator2 _calculator;

        [Scenario]
        public void Adding_two_numbers()
        {
            Given(a_calculator)
               .And(the_user_enters_OPERAND1, 50m)
                .And(the_user_enters_OPERAND2, 70m);
            When(adding);
            Then(the_result_is_EXPECTED, 120m);
        }

        [Scenario]
        public void Divide_by_zero()
        {
            Given(a_calculator)
                .And(the_user_enters_OPERAND1, 5m)
                .And(the_user_enters_OPERAND2, 0m);
            When(dividing).Throws();
            Then(an_exception_is_raised);
        }

        [Scenario]
        public void Subtracting_two_numbers()
        {
            Given(a_calculator)
                .And(the_user_enters_OPERAND1, 70m)
                .And(the_user_enters_OPERAND2, 50m);
            When(subtracting);
            Then(the_result_is_EXPECTED, 20m);
        }

        void a_calculator()
        {
            _calculator = new Calculator2();
        }

        void the_user_enters_OPERAND1(decimal operand1)
        {
            _calculator.Operand1 = operand1;
        }

        void the_user_enters_OPERAND2(decimal operand2)
        {
            _calculator.Operand2 = operand2;
        }

        void the_result_is_EXPECTED(decimal expected)
        {
            Assert.AreEqual(expected, _calculator.Result);
        }

        void subtracting()
        {
            _calculator.Subtract();
        }

        void adding()
        {
            _calculator.Add();
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

    class Calculator2
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }

        public decimal Result { get; set; }

        public void Add()
        {
            Result = Operand1 + Operand2;
        }

        public void Subtract()
        {
            Result = Operand1 - Operand2;
        }

        public void Divide()
        {
            Result = Operand1 / Operand2;
        }
    }
}
