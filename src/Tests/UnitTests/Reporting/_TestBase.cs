using Kekiri.Config;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Scenario(Feature.Reporting)]
    public abstract class ReportingScenarioTest : Test
    {
        protected ReportingScenarioMetaTest Scenario { private get; set; }
        internal Settings Settings { get; private set; }

        protected ReportingScenarioTest()
        {
            Settings = Settings.GetInstance();
        }

        [When]
        public void When()
        {
            Scenario.SetupScenario();

            ScenarioReport = Scenario.Report.TrimEnd();
        }

        protected string ScenarioReport { get; set; }
    }
}