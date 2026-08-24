using System.Reflection;

namespace Behavior.Internal;

/// <summary>
/// What the runner reports itself as. Read from the assembly so it tracks the package version
/// rather than a literal someone has to remember at release time, minus the "+&lt;sha&gt;" suffix
/// the SDK stamps on, which says nothing to a reader of the run.
/// </summary>
static class PackageVersion
{
    public static string Current { get; } = WithoutBuildMetadata(
        typeof(PackageVersion).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion);

    static string WithoutBuildMetadata(string version)
    {
        var separator = version.IndexOf('+');

        return separator < 0 ? version : version.Substring(0, separator);
    }
}
