using FluentAssertions;

namespace Kekiri.UnitTests.Fluent
{
    [ScenarioOutline]
    [Example("example1")]
    [Example("example2")]
    public class Can_fluently_execute_a_scenario_outline : FluentScenario
    {
        private string _expectedAValue;
        
        public Can_fluently_execute_a_scenario_outline(string a)
        {
            _expectedAValue = a;
            
            Given(a_fluent_scenario_outline);
            And(a_step_method_uses_a_parameter_A, a);
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
            aValue.Should().Be(_expectedAValue);
        }
    }

    internal class a_step_class_uses_a_parameter_B
    {
    }
}