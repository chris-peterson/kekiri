using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kekiri.Config;
using Kekiri.Reporting;

namespace Kekiri.Impl
{
    internal class ScenarioTestMetadata
    {
        private readonly Type _scenarioTestType;

        private class StepInfo
        {
            public IStepInvoker StepInvoker { get; set; }
            public string PrettyPrintedName { get; set; }
        }

        private Settings Settings { get; set; }

        private readonly IDictionary<StepType, IList<StepInfo>> _steps = new Dictionary<StepType, IList<StepInfo>>();
        
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
                throw new NotSupportedException(string.Format(
                    "Currently, only a single 'When' is supported, found: {0} and {1}", _steps[StepType.When].First().StepInvoker.SourceDescription, step.SourceDescription));
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

        public IEnumerable<IStepInvoker> GivenMethods
        {
            get { return GetSteps(StepType.Given); }
        }

        public IStepInvoker WhenMethod
        {
            get { return GetSteps(StepType.When).SingleOrDefault(); }
        }

        public IEnumerable<IStepInvoker> ThenMethods
        {
            get { return GetSteps(StepType.Then); }
        }

        public ScenarioReportingContext CreateReport()
        {
            FeatureReport featureReport = null;
            var scenarioReport = new List<string>();
            var stepReport = new List<string>();

            var scenario = ExtractAttributeFromScenarioTest<ScenarioAttribute>();
            var feature = scenario == null ? null : scenario.Feature;
            if (feature != null)
            {
                featureReport = new FeatureReport(feature.ToString());

                var featureAttribute = feature.GetType().GetField(featureReport.Name)
                    .AttributeOrDefault<FeatureDescriptionAttribute>();
                if (featureAttribute != null)
                {
                    featureReport.Set(featureAttribute.Summary, featureAttribute.Details);
                }
            }

            var tagAttributes = ExtractAttributesFromScenarioTest<TagAttribute>();
            if (tagAttributes != null)
            {
                scenarioReport.AddRange(tagAttributes.Select(tag => string.Format("@{0}", tag.Name)));
            }

            var scenarioAttribute = ExtractAttributeFromScenarioTest<ScenarioAttribute>();
            if (scenarioAttribute != null)
            {
                scenarioReport.Add(GetScenarioDescriptionOrDefaultValue(scenarioAttribute, _scenarioTestType));
            }

            stepReport.AddRange(GetStepReport(StepType.Given));
            stepReport.AddRange(GetStepReport(StepType.When));
            stepReport.AddRange(GetStepReport(StepType.Then));

            return new ScenarioReportingContext(
                featureReport,
                scenarioReport,
                stepReport,
                Settings);
        }

        private IEnumerable<IStepInvoker> GetSteps(StepType stepType)
        {
            return _steps[stepType].Select(s => s.StepInvoker);
        }

        private T ExtractAttributeFromScenarioTest<T>() where T : class
        {
            return _scenarioTestType.AttributeOrDefault<T>();
        }

        private IEnumerable<T> ExtractAttributesFromScenarioTest<T>() where T : class
        {
            return _scenarioTestType.GetCustomAttributes(
                typeof(T), true) as IEnumerable<T>;
        }

        private string GetScenarioDescriptionOrDefaultValue(ScenarioAttribute scenarioAttribute, Type declaringType)
        {
            return string.Format("{0}{1}",
                declaringType.HasAttribute<ExampleAttribute>()
                    ? Settings.GetToken(TokenType.ScenarioOutline)
                    : Settings.GetToken(TokenType.Scenario),
                scenarioAttribute == null || string.IsNullOrWhiteSpace(scenarioAttribute.Description)
                    ? declaringType.Name.WithSpaces()
                    : scenarioAttribute.Description);
        }

        private string GetStepNameWithTokenizedStepType(StepInfo stepInfo)
        {
            return string.Format("{0} {1}",
                                 Settings.GetStep(stepInfo.StepInvoker.Type),
                                 stepInfo.PrettyPrintedName);
        }

        private string GetStepNameWithTokenizedSeperators(StepInfo step)
        {
            return string.Format("{0} {1}",
                                 step.StepInvoker.Name.SeparatorToken,
                                 step.PrettyPrintedName);
        }

        private IEnumerable<string> GetStepReport(StepType stepType)
        {
            var lines = new List<string>();
            int insertedStepsCount = 0;
            foreach (var step in _steps[stepType]
                .ToList()
                .Where(s => !string.IsNullOrEmpty(s.PrettyPrintedName)))
            {
                lines.Add(insertedStepsCount == 0
                                      ? GetStepNameWithTokenizedStepType(step)
                                      : string.Format("{0}{1}", Settings.GetSeperator(SeperatorType.Indent),
                                                      GetStepNameWithTokenizedSeperators(step)));
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

            // string looks like it was intended to remain uppercase
            if (str.TakeWhile(Char.IsLetterOrDigit).Count(Char.IsUpper) > 1)
            {
                return str;
            }

            return string.Format("{0}{1}", char.ToLower(str[0]), str.Length == 1 ? null : str.Substring(1));
        }

        public static string ToLowerExceptFirstLetter(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            return string.Format("{0}{1}", str[0], str.Length == 1 ? null : str.Substring(1).ToLower());
        }

        public static string WithSpaces(this string str)
        {
            bool usingUnderscoreNamingConvention = str.Contains("_");

            if (usingUnderscoreNamingConvention)
            {
                return str.Replace("_", " ").TrimStart();
            }
            
            // pascal casing -- Adapted from: http://stackoverflow.com/questions/272633/add-spaces-before-capital-letters#272929
            var sentence = Regex.Replace(
                str, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

            return sentence.ToLowerExceptFirstLetter();
        }
    }
}
