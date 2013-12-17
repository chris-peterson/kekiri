using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Kekiri.Config;
using Kekiri.Reporting;
using NUnit.Framework;

namespace Kekiri.Impl
{
    internal class ScenarioTestMetadata
    {
        private readonly Type _scenarioTestType;

        private class StepInfo
        {
            public MethodBase MethodBase { get; set; }
            public IStepAttribute StepAttribute { get; set; }
            public SuppressOutputAttribute SuppressOutputAttribute { get; set; }
            public string PrettyPrintedName { get; set; }
        }

        private GherkinTestFrameworkSettingsFacade Settings { get; set; }

        private readonly IDictionary<StepType, IList<StepInfo>> _steps = new Dictionary<StepType, IList<StepInfo>>();
        private readonly IDictionary<string, string> _parameters = new Dictionary<string, string>();

        public ScenarioTestMetadata(Type scenarioTestType)
        {
            Settings = GherkinTestFrameworkSettingsFacade.GetInstance();

            _scenarioTestType = scenarioTestType;
            foreach (StepType stepType in Enum.GetValues(typeof(StepType)))
            {
                _steps.Add(stepType, new List<StepInfo>());
            }

            IsOutputSuppressed = ExtractSuppressOutputAttribute(_scenarioTestType) != null;
        }

        public IEnumerable<MethodBase> GivenMethods
        {
            get { return GetMethodBases(StepType.Given); }
            set { SetStepInfos<GivenAttribute>(StepType.Given, value); }
        }

        public IEnumerable<MethodBase> WhenMethods
        {
            get { return GetMethodBases(StepType.When); }
            set 
            {
                SetStepInfos<WhenAttribute>(StepType.When, value);

                var whenInfos = _steps[StepType.When];
                if ((whenInfos.Count == 1) && string.IsNullOrEmpty(whenInfos[0].PrettyPrintedName))
                {
                    whenInfos[0].PrettyPrintedName = PrettyPrintStepName(StepType.When, _scenarioTestType.Name, whenInfos[0].SuppressOutputAttribute != null);
                }
            }
        }

        public IEnumerable<MethodBase> ThenMethods
        {
            get { return GetMethodBases(StepType.Then); }
            set { SetStepInfos<ThenAttribute>(StepType.Then, value); }
        }

        public IDictionary<string, string> Parameters
        {
            get { return _parameters; }
        }

        public bool IsOutputSuppressed { get; private set; }

        public ScenarioReportingContext CreateReportForEntireScenario()
        {
            return CreateReport(ReportType.EntireScenario);
        }

        public ScenarioReportingContext CreateReportForCurrentTest()
        {
            return CreateReport(ReportType.CurrentTest);
        }

        private string PrettyPrintStepName(StepType stepType, string stepName, bool suppressOutput)
        {
            if (suppressOutput)
            {
                return string.Empty;
            }
            
            var stepNameSansStepType = GetStepNameWithoutTypeNorSeperators(stepType, stepName);

            if (string.IsNullOrWhiteSpace(stepNameSansStepType))
            {
                // no conversion to _ or PascalCase necessary -- bail
                return string.Empty;
            }

            var prettyPrinted = stepNameSansStepType.WithSpaces();

            foreach (var parameter in _parameters)
            {
                foreach (var word in prettyPrinted.Split(' '))
                {
                    if (word == parameter.Key)
                    {
                        prettyPrinted = prettyPrinted.Replace(word, parameter.Value);
                    }
                }
            }

            return prettyPrinted.WithFirstLetterLowercase();
        }

        private IEnumerable<MethodBase> GetMethodBases(StepType stepType)
        {
            return _steps[stepType].Select(s => s.MethodBase);
        }

        private void SetStepInfos<TAttribute>(StepType stepType, IEnumerable<MethodBase> methodBases)
            where TAttribute : class, IStepAttribute
        {
            _steps[stepType] = methodBases.Select(methodBase =>
                {
                    var suppressOutputAttribute = ExtractSuppressOutputAttribute(methodBase);
                    return new StepInfo
                        {
                            MethodBase = methodBase,
                            StepAttribute = methodBase.GetCustomAttributes(typeof (TAttribute), false).SingleOrDefault() as TAttribute,
                            SuppressOutputAttribute = suppressOutputAttribute,
                            PrettyPrintedName = PrettyPrintStepName(stepType, methodBase.Name, suppressOutputAttribute != null)
                        };
                }).ToList();
        }

        private SuppressOutputAttribute ExtractSuppressOutputAttribute(MethodBase methodBase)
        {
            var attributeOnMethod = methodBase
                                        .GetCustomAttributes(typeof (SuppressOutputAttribute), false)
                                        .SingleOrDefault() as SuppressOutputAttribute;

            if (attributeOnMethod == null)
            {
                return ExtractSuppressOutputAttribute(methodBase.DeclaringType);
            }

            return attributeOnMethod;
        }

        private SuppressOutputAttribute ExtractSuppressOutputAttribute(Type declaringType)
        {
            return declaringType.GetCustomAttributes(typeof (SuppressOutputAttribute), true)
                                .SingleOrDefault() as SuppressOutputAttribute;
        }

        private T ExtractAttributeFromScenarioTest<T>() where T : class
        {
            return _scenarioTestType.GetCustomAttributes(
                typeof (T), true).SingleOrDefault() as T;
        }

        private IEnumerable<T> ExtractAttributesFromScenarioTest<T>() where T : class
        {
            return _scenarioTestType.GetCustomAttributes(
                typeof(T), true) as IEnumerable<T>;
        }

        private string GetScenarioDescriptionOrDefaultValue(ScenarioAttribute scenarioAttribute, Type declaringType)
        {
            return string.Format("{0}{1}",
                scenarioAttribute is ScenarioOutlineAttribute
                    ? Settings.GetToken(TokenType.ScenarioOutline)
                    : Settings.GetToken(TokenType.Scenario),
                string.IsNullOrWhiteSpace(scenarioAttribute.Description)
                    ? declaringType.Name.WithSpaces()
                    : scenarioAttribute.Description);
        }

        private string GetStepNameWithTokenizedStepType(StepInfo stepInfo)
        {
            return string.Format("{0} {1}",
                                 Settings.GetStep(stepInfo.StepAttribute.StepType),
                                 stepInfo.PrettyPrintedName);
        }

        private string GetStepNameWithTokenizedSeperators(StepInfo step)
        {
            return string.Format("{0} {1}",
                                 GetPrefixTokenForStep(step.MethodBase.Name),
                                 step.PrettyPrintedName);
        }

        private string GetPrefixTokenForStep(string methodName)
        {
            var but = Settings.GetToken(TokenType.But);
            if (methodName.StartsWith(but, StringComparison.InvariantCultureIgnoreCase))
            {
                return but;
            }

            return Settings.GetToken(TokenType.And);
        }

        private string GetStepNameWithoutTypeNorSeperators(StepType stepType, string stepName)
        {
            stepName = stepName.RemovePrefix(Settings.GetStep(stepType));
            stepName = stepName.RemovePrefix(Settings.GetToken(TokenType.And));
            stepName = stepName.RemovePrefix(Settings.GetToken(TokenType.But));

            return stepName;
        }

        private ScenarioReportingContext CreateReport(ReportType reportType)
        {
            var featureReport = new List<string>();
            var scenarioReport = new List<string>();
            var stepReport = new List<string>();

            var featureAttribute = ExtractAttributeFromScenarioTest<FeatureAttribute>();
            if (featureAttribute != null)
            {
                featureReport.Add(string.Format("{0}{1}",
                                                Settings.GetToken(TokenType.Feature), featureAttribute.FeatureSummary));
                featureReport.AddRange(
                    featureAttribute.FeatureDetails
                                    .Select(line =>
                                            string.Format(
                                                "{0}{1}",
                                                Settings.GetSeperator(SeperatorType.Indent), line)));
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
            switch (reportType)
            {
                case ReportType.EntireScenario:
                    stepReport.AddRange(GetStepReport(StepType.Then));
                    break;
                case ReportType.CurrentTest:
                    stepReport.Add(GetReportForCurrentThen());
                    break;
                default:
                    throw new NotSupportedException(string.Format("Unknown report type '{0}'", reportType));
            }

            return new ScenarioReportingContext(
                featureReport,
                scenarioReport,
                stepReport,
                Settings);
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

        private string GetReportForCurrentThen()
        {
            if (TestContext.CurrentContext == null || TestContext.CurrentContext.Test == null)
            {
                return "!!! Test Context Unknown -- check your test runner's NUnit version !!!";
            }

            string testName;
            try
            {
                testName = TestContext.CurrentContext.Test.Name;
            }
            catch (NullReferenceException)
            {
                return "!!! Cannot get test name -- check your NUnit version !!!";
            }

            var currentTestNameSplit = testName.Split('.');
            string currentTestName = currentTestNameSplit.Last();

            var step = _steps[StepType.Then].FirstOrDefault(s => s.MethodBase.Name == currentTestName);
            if (step == null)
            {
                return string.Format("!!! Unknown Test '{0}' !!!", currentTestName);
            }

            return GetStepNameWithTokenizedStepType(step);
        }
    }

    public static class StepNameStringHelpers
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
            return string.Format("{0}{1}", char.ToLower(str[0]), str.Length == 1 ? null : str.Substring(1));
        }

        public static string WithSpaces(this string str)
        {
            bool usingUnderscoreNamingConvention = str.Contains("_");

            if (usingUnderscoreNamingConvention)
            {
                return str.Replace("_", " ").TrimStart();
            }
            
            // pascal casing -- Adapted from: http://stackoverflow.com/questions/272633/add-spaces-before-capital-letters#272929
            var splitIntoWords = Regex.Replace(
                str, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");

            return splitIntoWords.ToLower();
        }
    }
}
