using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kekiri.Config;

namespace Kekiri.Reporting
{
    class ScenarioReportingContext
    {
        public IList<string> ScenarioReport { get; }
        public IList<string> StepReport { get; }

        public Settings Settings { get; set; }

        public ScenarioReportingContext(
            IList<string> scenarioReport,
            IList<string> stepReport,
            Settings settings)
        {
            ScenarioReport = scenarioReport;
            StepReport = stepReport;
            Settings = settings;
        }

        public string CreateReport(bool omitFeatureOutput = false)
        {
            int indentationLevel = 0;
            var report = new List<string>();

            if (HasItems(ScenarioReport))
            {
                report.AddRange(TabifyLines(ScenarioReport, indentationLevel));
                indentationLevel++;
            }

            report.AddRange(TabifyLines(StepReport, indentationLevel));
            
            return InsertNewLines(report);
        }

        static bool HasItems(IEnumerable<string> lines)
        {
            return lines != null && lines.Any();
        }

        IEnumerable<string> TabifyLines(IEnumerable<string> lines, int indentationLevel)
        {
            return lines.Select(line => $"{GetTabs(indentationLevel)}{line}");
        }

        string GetTabs(int indentationLevel)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < indentationLevel; i++)
            {
                sb.Append(Settings.GetSeperator(SeperatorType.Indent));
            }

            return sb.ToString();
        }

        string InsertNewLines(IEnumerable<string> lines)
        {
            return $"{string.Join(Settings.GetSeperator(SeperatorType.Line), lines)}{Settings.GetSeperator(SeperatorType.Line)}";
        }
    }
}