using Kekiri.Config;
using Kekiri.TestSupport.Scenarios.Reporting;

namespace Kekiri.UnitTests.Reporting
{
    [Feature("Report generation", "In order to view the business rules of the system under test,",
        "The framework supports generating a human-readable report using Gherkin language semantics")]
    public abstract class ReportingScenarioTest : ScenarioTest
    {
        protected ReportingScenarioMetaTest Scenario { private get; set; }
        protected GherkinTestFrameworkSettingsFacade Settings { get; private set; }

        protected ReportingScenarioTest()
        {
            Settings = GherkinTestFrameworkSettingsFacade.GetInstance();
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