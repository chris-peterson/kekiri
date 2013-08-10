using FluentAssertions;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario]
    class Fixture_has_multiple_tags : ReportingScenarioTest
    {
        [Given]
        public void Given()
        {
            Scenario = new Fixture_has_multiple_tags_scenario();
        }

        [Then]
        public void It_should_have_tag_attributes()
        {
            ScenarioReport.Should().Contain("@Foo").And.Contain("@Bar");
        }
    }
}