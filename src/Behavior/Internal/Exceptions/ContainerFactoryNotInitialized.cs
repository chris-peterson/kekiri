namespace Behavior.Internal.Exceptions;

class ContainerFactoryNotInitialized : ScenarioException
{
    public ContainerFactoryNotInitialized(ScenarioBase scenario) : base(scenario, "ContainerFactory was not initialized (are you missing an integration assembly, e.g. Behavior.Autofac)?")
    {
    }
}
