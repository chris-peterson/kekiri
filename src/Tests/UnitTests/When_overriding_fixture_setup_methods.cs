using FluentAssertions;
using Kekiri.TestSupport.Scenarios;

namespace Kekiri.UnitTests
{
    [Feature("Customized fixture setup and teardown", "In order to allow developers to do custom steps during a fixture setup or teardown,", "The framework has a way of overriding the default behavior")]
    class When_overriding_fixture_setup_methods : ScenarioTest
    {
        readonly When_overriding_fixture_setup_methods_scenario _scenario = new When_overriding_fixture_setup_methods_scenario();

        [When]
        public void When()
        {
            _scenario.SetupScenario();
            _scenario.CleanupScenario();
        }

        [Then]
        public void It_should_call_setup_once()
        {
            _scenario.SetupScenairoCalledCount.Should().Be(1);
        }

        [Then]
        public void It_should_call_cleanup_once()
        {
            _scenario.CleanupScenarioCalledCount.Should().Be(1);
        }
    }
}