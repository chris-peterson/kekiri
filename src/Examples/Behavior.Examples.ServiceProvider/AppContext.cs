using Behavior.Examples.WebApp;

namespace Behavior.Examples.ServiceProvider.Injection;

public class AppContext
{
    readonly IFoo _foo;

    public AppContext(IFoo foo)
    {
        _foo = foo;
    }

    public IFoo Foo { get; private set; }

    public void Resolve() => Foo = _foo;
}
