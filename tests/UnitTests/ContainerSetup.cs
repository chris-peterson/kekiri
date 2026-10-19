using System;

// ScenarioBase.ContainerFactory is process-wide, and the IoC tests replace it. Running the
// suite serially is what keeps one test's factory from reaching another test's scenario.
[assembly: NotInParallel]

namespace Kekiri.Tests;

/// <summary>
/// A typed-context scenario resolves its context from the container, so one has to exist before
/// any scenario runs.
/// </summary>
static class ContainerSetup
{
    public static readonly Func<Container> Default = () => new TestContainer();

    [Before(Assembly)]
    public static void Initialize() => ScenarioBase.ContainerFactory = Default;

    /// <summary>Swaps the factory for one test and puts the default back afterwards.</summary>
    public static IDisposable Use(Func<Container> factory)
    {
        ScenarioBase.ContainerFactory = factory;
        return new Restore();
    }

    sealed class Restore : IDisposable
    {
        public void Dispose() => ScenarioBase.ContainerFactory = Default;
    }
}
