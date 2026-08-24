using System.Runtime.CompilerServices;

namespace Behavior.Tests;

/// <summary>
/// A typed-context scenario resolves its context from the container, so one has to exist before
/// any scenario runs. Set once for the assembly rather than per test, because the factory is
/// static and the tests run in parallel.
/// </summary>
static class ContainerSetup
{
    [ModuleInitializer]
    internal static void Initialize() => ScenarioBase.ContainerFactory = () => new TestContainer();
}
