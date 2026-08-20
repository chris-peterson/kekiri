using System;
using System.Runtime.CompilerServices;

namespace Behavior;

/// <summary>
/// Marks a scenario. No base attribute to inherit from here, because there is no host test
/// framework to satisfy — the runner reads this directly.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ScenarioAttribute : Attribute
{
    /// <summary>
    /// The compiler fills these in, which is how the runner can hand the IDE a file and line to
    /// navigate to without reading a PDB. xUnit v3's FactAttribute takes the same approach.
    /// </summary>
    public ScenarioAttribute(
        [CallerFilePath] string sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        SourceFilePath = sourceFilePath;
        SourceLineNumber = sourceLineNumber;
    }

    public string SourceFilePath { get; }

    public int SourceLineNumber { get; }
}
