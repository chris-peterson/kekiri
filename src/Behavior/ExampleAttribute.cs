using System;

namespace Behavior;

/// <summary>
/// One row of a scenario's Examples table, positional against the method's parameters. A scenario
/// carrying these is Gherkin's Scenario Outline, and the runner reports one test per row.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ExampleAttribute : Attribute
{
    public ExampleAttribute(params object[] data) => Data = data ?? new object[] { null };

    public object[] Data { get; }
}
