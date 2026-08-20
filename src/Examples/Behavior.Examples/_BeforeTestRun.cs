using Behavior.Autofac;
using Behavior;

namespace Behavior.Examples;

/// <summary>
/// Container bootstrapping for the whole assembly. Under a host framework this was an xUnit
/// assembly fixture or an NUnit [SetUpFixture]; the runner calls this itself now.
/// </summary>
public class _BeforeTestRun : IBeforeTestRun
{
    public void Setup() => AutofacBootstrapper.Initialize();
}
