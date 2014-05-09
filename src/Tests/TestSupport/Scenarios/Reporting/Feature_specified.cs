namespace Kekiri.TestSupport.Scenarios.Reporting
{
    [Scenario(Feature.Reporting)]
    public class Feature_specified : ReportingScenarioMetaTest
    {
        public Feature_specified()
        {
            IncludeFeatureReport = true;
        }

        [Given]
        public void Given_precondition()
        {
        }

        [When]
        public void When_doing_the_deed()
        {
        }

        [Then]
        public void It_should_do_the_right_thing()
        {
        }
    }

    public enum Feature
    {
        Reporting
    }
}