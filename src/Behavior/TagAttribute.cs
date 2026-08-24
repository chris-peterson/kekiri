using System;

namespace Behavior;

/// <summary>
/// A name/value tag on a scenario or a fixture, reported as test metadata. This is Gherkin's own
/// @tag, and IDEs and reports already know how to group and filter on the metadata it becomes.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class TagAttribute : Attribute
{
    public TagAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    public string Value { get; }
}
