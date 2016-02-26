using FluentAssertions;
using Kekiri.TestSupport.Scenarios;

namespace Kekiri.UnitTests
{
    [ScenarioBase(Feature.ExecuteTests)]
    class When_initializing_valid_fixture : Test
    {
        readonly When_initializing_valid_fixture_scenario _scenario = new When_initializing_valid_fixture_scenario();

        [When]
        public void When()
        {
            _scenario.SetupScenario();
        }

        [Then]
        public void It_should_call_given()
        {
            _scenario.GivenRunCount.Should().Be(1);
        }

        [Then]
        public void And_when()
        {
            _scenario.WhenRunCount.Should().Be(1);
        }

        [Then]
        public void But_not_then()
        {
            _scenario.ThenRunCount.Should().Be(0);
        }
    }
}