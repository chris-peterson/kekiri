namespace Kekiri.IoC.Autofac
{
    public class AutofacFluentScenario : IoCFluentScenario
    {
        public AutofacFluentScenario() : base(new AutofacContainer())
        {
        }
    }
}