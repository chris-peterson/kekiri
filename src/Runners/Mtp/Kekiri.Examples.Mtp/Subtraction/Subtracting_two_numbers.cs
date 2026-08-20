using System;
using Kekiri.Mtp;
using Kekiri.Examples.Mtp.Addition;

namespace Kekiri.Examples.Mtp.Subtraction
{
    /// <summary>
    /// A second namespace, so the run has two features to group under.
    /// </summary>
    public class Subtracting_two_numbers : Scenarios
    {
        Calculator _calculator;

        [Scenario]
        public void Subtracting_5_from_12()
        {
            Given(a_calculator)
                .And(the_user_enters_12)
                .And(the_user_enters_5);
            When(subtracting);
            Then(the_result_is_7);
        }

        void a_calculator() => _calculator = new Calculator();
        void the_user_enters_12() => _calculator.Operand1 = 12;
        void the_user_enters_5() => _calculator.Operand2 = 5;
        void subtracting() => _calculator.Result = _calculator.Operand1 - _calculator.Operand2;

        void the_result_is_7()
        {
            if (_calculator.Result != 7m)
            {
                throw new Exception($"Expected 7 but got {_calculator.Result}");
            }
        }
    }
}
