using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Kekiri.Reporting
{
    internal interface IReportTarget
    {
        void Report(ScenarioReportingContext scenario);
    }

    internal class CompositeReportTarget : IReportTarget
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

        public void Report(ScenarioReportingContext scenario)
        {
            foreach (var target in _targets)
            {
                target.Report(scenario);
            }
        }
    }

    internal class FeatureFileReportTarget : IReportTarget
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

        public void Report(ScenarioReportingContext scenario)
        {
            // if this test isn't categorized into a feature bucket, don't output it!
            if (scenario.FeatureReport == null)
            {
                return;
            }

            var featureName = scenario.FeatureReport.Name;
            if (_featureState.ContainsKey(featureName))
            {
                using (var fs = File.Open(_featureState[featureName].Path, FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(scenario.CreateReport(omitFeatureOutput: true));
                    }
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
                    using (var writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(scenario.CreateReport());
                    }
                }
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

    internal class TraceReportTarget : IReportTarget
    {
        private static readonly Lazy<TraceReportTarget> _target = new Lazy<TraceReportTarget>(() => new TraceReportTarget());

        private TraceReportTarget()
        {
        }

        public static IReportTarget GetInstance()
        {
            return _target.Value;
        }

        public void Report(ScenarioReportingContext scenario)
        {
            Trace.WriteLine(scenario.CreateReport());
        }
    }
}