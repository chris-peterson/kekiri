namespace Kekiri.IoC.SimpleInjector
{
    public class SimpleInjectorFluentTest : IoCFluentTest
    {
        public SimpleInjectorFluentTest() : base(new SimpleInjectorContainer())
        {
        }
    }

    public class SimpleInjectorFluentTest<TContext> : IoCFluentTest<TContext> where TContext : class
    {
        public SimpleInjectorFluentTest()
            : base(new SimpleInjectorContainer())
        {
        }
    }
}