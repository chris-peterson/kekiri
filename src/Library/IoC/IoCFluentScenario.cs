namespace Kekiri.IoC
{
    public abstract class IoCFluentScenario : FluentScenario, IContainerAccessor
    {
        protected internal Container Container;

        protected IoCFluentScenario(Container container)
        {
            Container = container;
        }

        Container IContainerAccessor.Container
        {
            get { return Container; }
        }
    }
}