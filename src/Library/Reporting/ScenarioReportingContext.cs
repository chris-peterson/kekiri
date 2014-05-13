using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kekiri.Config;

namespace Kekiri.Reporting
{
    internal class ScenarioReportingContext
    {
        public FeatureReport FeatureReport { get; private set; }
        public IList<string> ScenarioReport { get; private set; }
        public IList<string> StepReport { get; private set; }

        public Settings Settings { get; set; }

        public ScenarioReportingContext(
            FeatureReport featureReport,
            IList<string> scenarioReport,
            IList<string> stepReport,
            Settings settings)
        {
            FeatureReport = featureReport;
            ScenarioReport = scenarioReport;
            StepReport = stepReport;
            Settings = settings;
        }

        public string CreateReport()
        {
            if (FeatureReport == null)
            {
                return string.Empty;
            }

            int indentationLevel = 0;
            var report = new List<string>();

            report.Insert(0, string.Format("{0}{1}",
                Settings.GetToken(TokenType.Feature), FeatureReport.Summary));
            if (HasItems(FeatureReport.Details))
            {
                report.AddRange(FeatureReport.Details.Select(line =>
                    string.Format(
                        "{0}{1}",
                        Settings.GetSeperator(SeperatorType.Indent), line)));
            }
            report.Add(string.Empty);

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

    internal class FeatureReport
    {
        public FeatureReport(string name)
        {
            Summary = Name = name;
        }

        public string Name { get; private set; }

        public string Summary { get; private set; }

        public IEnumerable<string> Details { get; private set; }

        public void Set(string summary, IEnumerable<string> details)
        {
            Summary = summary;
            Details = details;
        }
    }
}