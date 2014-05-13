using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    class When_fixture_uses_implicit_scenario_description : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new Fixture_uses_implicit_scenario_description();
        }

        [Then]
        public void It_should_use_the_class_name()
        {
            ScenarioReport.Should().Be(
                "Feature: TestSupport\r\n\r\nScenario: Fixture uses implicit scenario description\r\n  When fixture uses implicit scenario description\r\n  Then it will use the class name");
        }
    }
}