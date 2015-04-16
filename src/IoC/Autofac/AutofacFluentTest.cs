namespace Kekiri.IoC.Autofac
{
    public class AutofacFluentTest : IoCFluentTest
    {
        public AutofacFluentTest() : base(new AutofacContainer())
        {
        }
    }

    public class AutofacFluentTest<TContext> : IoCFluentTest<TContext> where TContext : class
    {
        public AutofacFluentTest()
            : base(new AutofacContainer())
        {
        }
    }
}