﻿namespace Kekiri.TestSupport.Scenarios.Reporting
{
    [Tag("Foo"), Tag("Bar")]
    [Scenario(Feature.TestSupport)]
    public class Fixture_has_multiple_tags_scenario : ReportingScenarioMetaTest
    {
        [When]
        public void When()
        {
        }

        [Then]
        public void Then()
        {
        }
    }
}