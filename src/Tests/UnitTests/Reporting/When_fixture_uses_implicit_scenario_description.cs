using FluentAssertions;
using Kekiri.Config;
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
            ScenarioReport.Should().StartWith(string.Format("{0}Fixture uses implicit scenario description",
                Settings.GetInstance().GetToken(TokenType.Scenario)));
        }
    }
}