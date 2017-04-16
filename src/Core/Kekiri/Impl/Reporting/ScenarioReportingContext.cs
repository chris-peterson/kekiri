using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kekiri.Impl.Config;

namespace Kekiri.Impl.Reporting
{
    class ScenarioReportingContext
    {
        public string FeatureName { get; }
        public IList<string> StepReport { get; }

        public Settings Settings { get; }

        public ScenarioReportingContext(
            string featureName,
            IList<string> stepReport,
            Settings settings)
        {
            FeatureName = featureName;
            StepReport = stepReport;
            Settings = settings;
        }

        public string CreateReport()
        {
            const int indentationLevel = 0;
            var report = new List<string>();
            report.AddRange(TabifyLines(StepReport, indentationLevel));
            
            return InsertNewLines(report);
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