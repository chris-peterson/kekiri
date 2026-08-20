using System;
using Kekiri.Mtp;

namespace Kekiri.Examples.Mtp.Addition
{
    public class Adding_two_numbers : Scenarios
    {
        Calculator _calculator;

        [Scenario]
        public void Adding_50_and_70()
        {
            Given(a_calculator)
                .And(the_user_enters_50)
                .And(the_user_enters_70);
            When(adding);
            Then(the_result_is_120);
        }

        [Scenario]
        public void A_failing_scenario_reports_which_step_failed()
        {
            Given(a_calculator)
                .And(the_user_enters_50);
            When(adding);
            Then(the_result_is_120);
        }

        void a_calculator() => _calculator = new Calculator();
        void the_user_enters_50() => _calculator.Operand1 = 50;
        void the_user_enters_70() => _calculator.Operand2 = 70;
        void adding() => _calculator.Add();

        void the_result_is_120()
        {
            if (_calculator.Result != 120m)
            {
                throw new Exception($"Expected 120 but got {_calculator.Result}");
            }
        }
    }

    public class Calculator
    {
        public decimal Operand1 { get; set; }
        public decimal Operand2 { get; set; }
        public decimal Result { get; set; }

        public void Add() => Result = Operand1 + Operand2;
    }
}
