using NUnit.Framework;

namespace Kekiri.IoC.Autofac
{
    public class AutofacFluentScenario : IoCFluentScenario
    {
        public AutofacFluentScenario() : base(new AutofacContainer())
        {
        }
    }

    public class AutofacFluentScenario<TContext> : IoCFluentScenario<TContext> where TContext : new()
    {
        public AutofacFluentScenario()
            : base(new AutofacContainer())
        {
        }
    }
}