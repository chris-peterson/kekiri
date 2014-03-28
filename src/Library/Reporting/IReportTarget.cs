using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Kekiri.Reporting
{
    public interface IReportTarget
    {
        void Report(ReportType reportType, ScenarioReportingContext scenario);
    }

    public class CompositeReportTarget : IReportTarget
    {
        private readonly List<IReportTarget> _targets;

        private CompositeReportTarget(IEnumerable<IReportTarget> targets)
        {
            _targets = new List<IReportTarget>(targets);
        }

        public static IReportTarget GetInstance()
        {
            return new CompositeReportTarget(
                new[]
                {
                    TraceReportTarget.GetInstance(),
                    FeatureFileReportTarget.GetInstance()
                });
        }

        public void Report(ReportType reportType, ScenarioReportingContext scenario)
        {
            foreach (var target in _targets)
            {
                target.Report(reportType, scenario);
            }
        }
    }

    public class FeatureFileReportTarget : IReportTarget
    {
        private static readonly Lazy<FeatureFileReportTarget> _target = new Lazy<FeatureFileReportTarget>(() => new FeatureFileReportTarget());

        private readonly Dictionary<string, dynamic> _featureState = new Dictionary<string, dynamic>();

        private FeatureFileReportTarget()
        {
        }
        
        public static IReportTarget GetInstance()
        {
            return _target.Value;
        }

        public void Report(ReportType reportType, ScenarioReportingContext scenario)
        {
            if (reportType != ReportType.EntireScenario)
            {
                return;
            }

            var featureName = scenario.FeatureReport.Name;
            if (_featureState.ContainsKey(featureName))
            {
                using (var fs = File.Open(_featureState[featureName].Path, FileMode.Append, FileAccess.Write))
                {
                    WriteScenario(scenario, fs, includeFeatureReport: false);
                }
            }
            else
            {
                _featureState.Add(featureName, new
                {
                    Path = string.Format("{0}.feature", CoerceValidFileName(featureName))
                });

                using (var fs = File.Create(_featureState[featureName].Path))
                {
                    WriteScenario(scenario, fs, includeFeatureReport: true);
                }
            }
        }

        private static void WriteScenario(ScenarioReportingContext reportingContext, Stream fs, bool includeFeatureReport)
        {
            using (var writer = new StreamWriter(fs))
            {
                writer.WriteLine(reportingContext.CreateReportWithStandardSpacing(
                    0, includeFeatureReport));
            }
        }

        // http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        private static string CoerceValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(@"[{0}]+", invalidChars);

            var reservedWords = new[]
                                    {
                                        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                                        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                                        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                                    };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format("^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }
    }

    public class TraceReportTarget : IReportTarget
    {
        private static readonly Lazy<TraceReportTarget> _target = new Lazy<TraceReportTarget>(() => new TraceReportTarget());
        private string _previousFeatureKey;

        private TraceReportTarget()
        {
        }

        public static IReportTarget GetInstance()
        {
            return _target.Value;
        }

        public void Report(ReportType reportType, ScenarioReportingContext scenario)
        {
            // only output current test info if running in R# -- it's not useful in command line builds
            if (reportType == ReportType.CurrentTest)
            {
                var executingProcessName = Process.GetCurrentProcess().ProcessName;

                if (executingProcessName.IndexOf("JetBrains.ReSharper", StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    return;
                }
            }

            int indentationLevel = 0;
            // avoid repeating the same feature over and over
            var featureKey = GetKey(scenario.FeatureReport);
            bool includeFeatureReport = true;
            if (featureKey != null && featureKey == _previousFeatureKey)
            {
                indentationLevel = 1;
                includeFeatureReport = false;
            }

            Trace.WriteLine(scenario.CreateReportWithStandardSpacing(indentationLevel, includeFeatureReport));

            _previousFeatureKey = featureKey;
        }

        private static string GetKey(FeatureReport report)
        {
            if (report == null)
            {
                return null;
            }

            // TODO: this is garbage.
            var list = new List<string> {report.Summary};
            if (report.Details != null)
            {
                list.AddRange(report.Details);
            }

            return string.Join("-", list);
        }
    }
}