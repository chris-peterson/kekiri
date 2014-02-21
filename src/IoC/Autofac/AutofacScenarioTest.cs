namespace Kekiri.IoC.Autofac
{
    public class AutofacScenarioTest : IoCScenarioTest
    {
        public AutofacScenarioTest() : base(new AutofacContainer())
        {
        }
    }
}