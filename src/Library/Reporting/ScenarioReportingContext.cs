using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kekiri.Config;

namespace Kekiri.Reporting
{
    public class ScenarioReportingContext
    {
        public FeatureReport FeatureReport { get; private set; }
        public IList<string> ScenarioReport { get; private set; }
        public IList<string> StepReport { get; private set; }

        public GherkinTestFrameworkSettingsFacade Settings { get; set; }

        public ScenarioReportingContext(
            FeatureReport featureReport,
            IList<string> scenarioReport,
            IList<string> stepReport,
            GherkinTestFrameworkSettingsFacade settings)
        {
            FeatureReport = featureReport;
            ScenarioReport = scenarioReport;
            StepReport = stepReport;
            Settings = settings;
        }

        public string CreateReportWithStandardSpacing(int indentationLevel, bool includeFeatureReport = true)
        {
            var report = new List<string>();

            if (includeFeatureReport)
            {
                report.Insert(0, string.Format("{0}{1}",
                    Settings.GetToken(TokenType.Feature), FeatureReport.Summary));
                if (FeatureReport.Details != null)
                {
                    report.AddRange(FeatureReport.Details.Select(line =>
                        string.Format(
                            "{0}{1}",
                            Settings.GetSeperator(SeperatorType.Indent), line)));
                }
                report.Add(string.Empty);
                indentationLevel++;
            }

            if (HasItems(ScenarioReport))
            {
                report.AddRange(TabifyLines(ScenarioReport, indentationLevel));
                indentationLevel++;
            }

            report.AddRange(TabifyLines(StepReport, indentationLevel));
            
            return InsertNewLines(report);
        }

        private bool HasItems(IEnumerable<string> lines)
        {
            return lines != null && lines.Any();
        }

        private IEnumerable<string> TabifyLines(IEnumerable<string> lines, int indentationLevel)
        {
            return lines.Select(line => string.Format("{0}{1}", GetTabs(indentationLevel), line));
        }

        private string GetTabs(int indentationLevel)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < indentationLevel; i++)
            {
                sb.Append(Settings.GetSeperator(SeperatorType.Indent));
            }

            return sb.ToString();
        }

        private string InsertNewLines(IEnumerable<string> lines)
        {
            return string.Format("{0}{1}",
                                 string.Join(Settings.GetSeperator(SeperatorType.Line), lines),
                                 Settings.GetSeperator(SeperatorType.Line));
        }
    }

    public class FeatureReport
    {
        public string Name { get; internal set; }

        public string Summary { get; internal set; }

        public IList<string> Details { get; internal set; }
    }
}