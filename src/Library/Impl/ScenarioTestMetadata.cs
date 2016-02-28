using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kekiri.Impl.Config;
using Kekiri.Impl.Reporting;

namespace Kekiri.Impl
{
    class ScenarioTestMetadata
    {
        readonly Type _scenarioTestType;

        class StepInfo
        {
            public IStepInvoker StepInvoker { get; set; }
            public string PrettyPrintedName { get; set; }
        }

        Settings Settings { get; }

        readonly IDictionary<StepType, IList<StepInfo>> _steps = new Dictionary<StepType, IList<StepInfo>>();
        
        public ScenarioTestMetadata(Type scenarioTestType)
        {
            Settings = Settings.GetInstance();
            _scenarioTestType = scenarioTestType;
            foreach (StepType stepType in Enum.GetValues(typeof(StepType)))
            {
                _steps.Add(stepType, new List<StepInfo>());
            }
        }

        public void AddStep(IStepInvoker step)
        {
            if (step.Type == StepType.When && _steps[StepType.When].Count == 1)
            {
                throw new NotSupportedException(
                    $"Only a single 'When' is supported, found: {_steps[StepType.When].First().StepInvoker.SourceDescription} and {step.SourceDescription}");
            }

            var stepInfo = new StepInfo
            {
                StepInvoker = step,
                PrettyPrintedName = step.Name.PrettyName
            };
            if (step.Type == StepType.When && string.IsNullOrEmpty(stepInfo.PrettyPrintedName))
            {
                stepInfo.PrettyPrintedName = new StepName(StepType.When, _scenarioTestType.Name).PrettyName;
            }
            
            _steps[step.Type].Add(stepInfo);   
        }

        public IEnumerable<IStepInvoker> GivenMethods => GetSteps(StepType.Given);

        public IStepInvoker WhenMethod => GetSteps(StepType.When).SingleOrDefault();

        public IEnumerable<IStepInvoker> ThenMethods => GetSteps(StepType.Then);

        public ScenarioReportingContext CreateReport()
        {
            var scenarioReport = new List<string>();
            var stepReport = new List<string>();

            stepReport.AddRange(GetStepReport(StepType.Given));
            stepReport.AddRange(GetStepReport(StepType.When));
            stepReport.AddRange(GetStepReport(StepType.Then));

            return new ScenarioReportingContext(
                scenarioReport,
                stepReport,
                Settings);
        }

        IEnumerable<IStepInvoker> GetSteps(StepType stepType)
        {
            return _steps[stepType].Select(s => s.StepInvoker);
        }

        string GetStepNameWithTokenizedStepType(StepInfo stepInfo)
        {
            return $"{Settings.GetStep(stepInfo.StepInvoker.Type)} {stepInfo.PrettyPrintedName}";
        }

        string GetStepNameWithTokenizedSeperators(StepInfo step)
        {
            return $"{step.StepInvoker.Name.SeparatorToken} {step.PrettyPrintedName}";
        }

        IEnumerable<string> GetStepReport(StepType stepType)
        {
            var lines = new List<string>();
            int insertedStepsCount = 0;
            foreach (var step in _steps[stepType]
                .ToList()
                .Where(s => !string.IsNullOrEmpty(s.PrettyPrintedName)))
            {
                lines.Add(insertedStepsCount == 0
                                      ? GetStepNameWithTokenizedStepType(step)
                                      : $"{Settings.GetSeperator(SeperatorType.Indent)}{GetStepNameWithTokenizedSeperators(step)}");
                insertedStepsCount++;
            }

            return lines;
        }
    }

    internal static class StepNameStringHelpers
    {
        public static string RemovePrefix(this string stepName, string prefix)
        {
            if (string.IsNullOrEmpty(stepName))
            {
                return null;
            }

            if (stepName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                if (stepName.Length == prefix.Length)
                {
                    // there is nothing meaningful to output
                    return null;
                }

                return stepName.Substring(prefix.Length);
            }

            return stepName;
        }

        public static string WithFirstLetterLowercase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            return $"{char.ToLower(str[0])}{(str.Length == 1 ? null : str.Substring(1))}";
        }

        public static bool StartsWithMultipleUppercaseLetters(this string str)
        {
            int uppercaseCount = 0;

            foreach (var c in str.SkipWhile(c => !char.IsLetterOrDigit(c)))
            {
                if (char.IsUpper(c))
                {
                    uppercaseCount++;
                }
                else
                {
                    break;
                }
            }
            return uppercaseCount > 1;
        }

        public static string ToLowerExceptFirstLetter(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? string.Empty : $"{str[0]}{(str.Length == 1 ? null : str.Substring(1).ToLower())}";
        }

        public static string AsSentence(this string str)
        {
            bool usingUnderscoreNamingConvention = str.Contains("_");

            if (usingUnderscoreNamingConvention)
            {
                return str.Replace("_", " ").TrimStart();
            }

            // pascal casing -- Adapted from: http://stackoverflow.com/questions/272633/add-spaces-before-capital-letters#272929
            var sentence = Regex.Replace(
                str, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

            return sentence.StartsWithMultipleUppercaseLetters()
                ? sentence
                : sentence.ToLowerExceptFirstLetter();
        }
    }
}
