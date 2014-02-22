using FluentAssertions;

namespace Kekiri.UnitTests.Fluent
{
    [ScenarioOutline]
    [Example("example1", 3)]
    [Example("example2", 57)]
    public class Can_fluently_execute_a_scenario_outline : FluentScenario
    {
        private string _expectedAValue;
        private int _expectedBValue;
        
        public Can_fluently_execute_a_scenario_outline(string a, int b)
        {
            _expectedAValue = a;
            _expectedBValue = b;
            
            Given(a_fluent_scenario_outline);
            And(a_step_method_uses_a_parameter_A, a);
            And<a_step_class_uses_a_parameter_B>(b);
            When(the_scenario_is_executed);
            Then(the_parameters_are_used_by_the_scenario);
        }

        private void a_fluent_scenario_outline()
        {
            
        }

        private void a_step_method_uses_a_parameter_A(string a)
        {
            Context.AValue = a;
        }

        private void the_scenario_is_executed()
        {
            
        }

        private void the_parameters_are_used_by_the_scenario()
        {
            string aValue = Context.AValue;
            int bValue = Context.BValue;
            aValue.Should().Be(_expectedAValue);
            bValue.Should().Be(_expectedBValue);
        }
    }

    internal class a_step_class_uses_a_parameter_B : Step
    {
        private readonly int _b;

        public a_step_class_uses_a_parameter_B(int b)
        {
            _b = b;
        }

        public override void Execute()
        {
            Context.BValue = _b;
        }
    }
}