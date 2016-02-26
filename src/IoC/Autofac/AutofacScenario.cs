namespace Kekiri.IoC.Autofac
{
    public class AutofacScenario : IoCScenario
    {
        public AutofacScenario() : base(new AutofacContainer())
        {
        }
    }

    public class AutofacScenario<TContext> : IoCScenario<TContext> where TContext : class
    {
        public AutofacScenario()
            : base(new AutofacContainer())
        {
        }
    }
}