using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Behavior.Internal.Config;
using Behavior.Internal.Reporting;

namespace Behavior.Internal;

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
        var stepReport = new List<string>();

        stepReport.AddRange(GetStepReport(StepType.Given));
        stepReport.AddRange(GetStepReport(StepType.When));
        stepReport.AddRange(GetStepReport(StepType.Then));

        return new ScenarioReportingContext(
            _scenarioTestType
                .Namespace.Split('.')
                .Last(),
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
