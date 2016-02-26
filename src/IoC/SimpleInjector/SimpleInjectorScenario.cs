namespace Kekiri.IoC.SimpleInjector
{
    public class SimpleInjectorScenario : IoCScenario
    {
        public SimpleInjectorScenario() : base(new SimpleInjectorContainer())
        {
        }
    }

    public class SimpleInjectorScenario<TContext> : IoCScenario<TContext> where TContext : class
    {
        public SimpleInjectorScenario()
            : base(new SimpleInjectorContainer())
        {
        }
    }
}