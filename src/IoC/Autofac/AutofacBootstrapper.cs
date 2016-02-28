namespace Kekiri.IoC.Autofac
{
    public class AutofacBootstrapper
    {
        public static void Initialize()
        {
            ScenarioBase.ContainerFactory = () => new AutofacContainer();
        }
    }
}