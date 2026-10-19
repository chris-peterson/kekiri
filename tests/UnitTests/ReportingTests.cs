using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kekiri.Internal;
using Kekiri.Internal.Config;
using Kekiri.Internal.Reporting;
using Kekiri.Tests.Fixtures;

namespace Kekiri.Tests;

public class ReportingTests
{
    [Test]
    public async Task A_scenario_reports_as_gherkin()
    {
        var run = await Run.Scenario<Reporting>(f => f.Every_step_kind());

        await Assert.That(run.Failure).IsNull();
        await Assert.That(string.Join("|", run.Steps)).IsEqualTo(string.Join("|",
            "Given a calculator",
            "  And a second operand",
            "When reporting",
            "Then it works",
            "  But nothing is saved",
            "  But nothing is sent",
            "  But this one was named but"));
    }

    [Test]
    public async Task The_report_is_one_line_per_step()
    {
        var report = new ScenarioReportingContext("Feature", new[] { "Given a", "When b" }, Settings.GetInstance());

        await Assert.That(report.CreateReport()).IsEqualTo("Given a\r\nWhen b\r\n");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("console")]
    public async Task Trace_is_the_only_target_by_default(string outputConfig)
    {
        var targets = Targets(CompositeReportTarget.Configured(outputConfig));

        await Assert.That(targets.Length).IsEqualTo(1);
        await Assert.That(targets[0]).IsTypeOf<TraceReportTarget>();
    }

    [Test]
    [Arguments("files")]
    [Arguments("console, FILES")]
    public async Task Files_adds_the_feature_file_target(string outputConfig)
    {
        var targets = Targets(CompositeReportTarget.Configured(outputConfig));

        await Assert.That(targets.Length).IsEqualTo(2);
        await Assert.That(targets[1]).IsTypeOf<FeatureFileReportTarget>();
    }

    [Test]
    public async Task A_fixture_reports_to_its_own_target_ahead_of_the_configured_ones()
    {
        var first = new CapturingReportTarget();

        var targets = Targets(CompositeReportTarget.Including(first));

        await Assert.That(ReferenceEquals(targets[0], first)).IsTrue();
        await Assert.That(ReferenceEquals(targets[1], CompositeReportTarget.GetInstance())).IsTrue();
    }

    [Test]
    public async Task A_composite_reports_to_every_target()
    {
        var first = new CapturingReportTarget();
        var second = new CapturingReportTarget();
        var context = new ScenarioReportingContext("Feature", new[] { "Given a" }, Settings.GetInstance());

        CompositeReportTarget.Including(first).Report(context);
        new CapturingReportTarget().Report(context);
        first.Report(context);
        second.Report(context);

        await Assert.That(first.FeatureName).IsEqualTo("Feature");
        await Assert.That(second.Steps.Single()).IsEqualTo("Given a");
    }

    [Test]
    public async Task The_trace_target_writes_the_report_to_trace_listeners()
    {
        var listener = new Recording_listener();
        Trace.Listeners.Add(listener);
        try
        {
            TraceReportTarget.GetInstance().Report(
                new ScenarioReportingContext("Feature", new[] { "Given a" }, Settings.GetInstance()));
        }
        finally
        {
            Trace.Listeners.Remove(listener);
        }

        await Assert.That(listener.Written.Contains("Given a")).IsTrue();
    }

    [Test]
    public async Task The_file_target_writes_one_feature_file_per_feature_and_appends_to_it()
    {
        var feature = $"Feature {Guid.NewGuid():N}";
        var path = Path.Combine(AppContext.BaseDirectory, $"{feature}.feature");
        try
        {
            var target = FeatureFileReportTarget.GetInstance();
            target.Report(new ScenarioReportingContext(feature, new[] { "Given a" }, Settings.GetInstance()));
            target.Report(new ScenarioReportingContext(feature, new[] { "Given b" }, Settings.GetInstance()));

            var lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToArray();

            await Assert.That(string.Join("|", lines)).IsEqualTo("Given a|Given b");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    [Arguments("a/b", "a_b")]
    [Arguments("CON.thing", "_reservedWord_.thing")]
    public async Task A_feature_name_becomes_a_valid_file_name(string feature, string fileName)
    {
        var unique = Guid.NewGuid().ToString("N");
        var path = Path.Combine(AppContext.BaseDirectory, $"{fileName}{unique}.feature");
        try
        {
            FeatureFileReportTarget.GetInstance().Report(
                new ScenarioReportingContext(feature + unique, new[] { "Given a" }, Settings.GetInstance()));

            await Assert.That(File.Exists(path)).IsTrue();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task An_unknown_keyword_is_refused_rather_than_rendered()
    {
        var settings = Settings.GetInstance();

        Assert.Throws<NotSupportedException>(() => settings.GetStep((StepType)99));
        Assert.Throws<NotSupportedException>(() => settings.GetSeperator((SeperatorType)99));
        Assert.Throws<NotSupportedException>(() => settings.GetToken((TokenType)99));

        await Assert.That(settings.GetToken(TokenType.But)).IsEqualTo("But");
    }

    static IReportTarget[] Targets(IReportTarget composite) =>
        ((CompositeReportTarget)composite).Targets.ToArray();

    sealed class Recording_listener : TraceListener
    {
        public string Written { get; private set; } = string.Empty;

        public override void Write(string message) => Written += message;

        public override void WriteLine(string message) => Written += message + Environment.NewLine;
    }
}
