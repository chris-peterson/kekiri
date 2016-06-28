using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kekiri.Impl.Reporting
{
    class FeatureFileReportTarget : IReportTarget
    {
        static readonly Lazy<FeatureFileReportTarget> _target = new Lazy<FeatureFileReportTarget>(() => new FeatureFileReportTarget());

        readonly Dictionary<string, dynamic> _featureState = new Dictionary<string, dynamic>();

        FeatureFileReportTarget()
        {
        }
        
        public static IReportTarget GetInstance()
        {
            return _target.Value;
        }

        public void Report(ScenarioReportingContext scenario)
        {
            var featureName = scenario.FeatureName;

            if (_featureState.ContainsKey(featureName))
            {
                using (var fs = File.Open(_featureState[featureName].Path, FileMode.Append, FileAccess.Write))
                {
                    Write(scenario, fs);
}
            }
            else
            {
                _featureState.Add(featureName, new
                {
                    Path = $"{CoerceValidFileName(featureName)}.feature"
                });
                using (var fs = File.Create(_featureState[featureName].Path))
                {
                    Write(scenario, fs);  
                }
            }
        }

        static void Write(ScenarioReportingContext context, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(context.CreateReport());
            }
        }

        // http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        static string CoerceValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = $@"[{invalidChars}]+";

            var reservedWords = new[]
            {
                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");

            return reservedWords.Select(reservedWord => $"^{reservedWord}\\.").Aggregate(sanitisedNamePart, (current, reservedWordPattern) => Regex.Replace(current, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase));
        }
    }
}