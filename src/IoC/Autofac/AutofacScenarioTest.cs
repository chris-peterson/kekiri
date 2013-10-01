namespace Kekiri.IoC.Autofac
{
    public class AutofacScenarioTest : DependencyInjectionScenarioTest
    {
        public AutofacScenarioTest() : base(new AutofacTestingContext())
        {
        }
    }
}