using System;
using System.Collections.Generic;
using System.Linq;

namespace Kekiri.Internal.Reporting;

class CompositeReportTarget : IReportTarget
{
    readonly List<IReportTarget> _targets;

    CompositeReportTarget(IEnumerable<IReportTarget> targets)
    {
        _targets = new List<IReportTarget>(targets);
    }

    readonly static Lazy<IReportTarget> _instance = new Lazy<IReportTarget>(() =>
        Configured(Environment.GetEnvironmentVariable("KEKIRI_OUTPUT")));

    /// <summary>
    /// The targets a KEKIRI_OUTPUT value asks for. The console is the runner's: it prints every
    /// scenario as Gherkin already, so the only target to opt into is <c>files</c>.
    /// </summary>
    internal static IReportTarget Configured(string outputConfig)
    {
        var targets = new List<IReportTarget> { TraceReportTarget.GetInstance() };

        var requested = (outputConfig ?? string.Empty)
            .Split(',')
            .Select(s => s.Trim());

        if (requested.Contains("files", StringComparer.OrdinalIgnoreCase))
        {
            targets.Add(FeatureFileReportTarget.GetInstance());
        }

        return new CompositeReportTarget(targets);
    }

    internal IReadOnlyList<IReportTarget> Targets => _targets;

    public static IReportTarget GetInstance()
    {
        return _instance.Value;
    }

    /// <summary>
    /// The configured targets with <paramref name="first"/> ahead of them. A fixture that captures
    /// its own Gherkin for the runner still has to reach the targets KEKIRI_OUTPUT asked for,
    /// so it composes rather than substitutes.
    /// </summary>
    public static IReportTarget Including(IReportTarget first)
    {
        return new CompositeReportTarget(new[] { first, GetInstance() });
    }

    public void Report(ScenarioReportingContext scenario)
    {
        foreach (var target in _targets)
        {
            target.Report(scenario);
        }
    }
}
