using Kekiri.IoC.Autofac;
using Xunit;

[assembly: AssemblyFixture(typeof(Kekiri.Examples.Xunit.Bootstrap))]

namespace Kekiri.Examples.Xunit
{
    public class Bootstrap
    {
        public Bootstrap()
        {
            AutofacBootstrapper.Initialize(x => x.IncludeNonPublicConstructors());
        }
    }
}
