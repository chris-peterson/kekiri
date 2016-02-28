namespace Kekiri.Exceptions
{
    class ContainerFactoryNotInitialized : ScenarioException
    {
        public ContainerFactoryNotInitialized(object test) : base(test, "ContainerFactory was not initialized (are you missing an integration assembly, e.g. Kekiri.IoC.Autofac)?")
        {
        }
    }
}