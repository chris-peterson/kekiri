using Behavior.Internal;

namespace Behavior;

/// <summary>
/// How the runner reaches the recorded Gherkin without caring which Scenarios base a test used.
/// Reflecting for the field instead would miss it: a non-public member declared on a base class
/// isn't returned by GetField on the derived type, and consumers subclass several deep.
/// </summary>
interface IRecordsSteps
{
    CapturingReportTarget Captured { get; }
}
