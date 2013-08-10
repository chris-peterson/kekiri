using FluentAssertions;
using Kekiri.TestSupport.Scenarios;

namespace Kekiri.UnitTests
{
    [Feature("Happy path test execution", "In order to be a viable test framework, it certainly must support tests that perform as expected, i.e. pass")]
    class When_initializing_valid_fixture : ScenarioTest
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