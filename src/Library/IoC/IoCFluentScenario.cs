namespace Kekiri.IoC
{
    public abstract class IoCFluentScenario : FluentTest, IContainerAccessor
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

    public abstract class IoCFluentScenario<TContext> : FluentTest, IContainerAccessor where TContext : new()
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

        protected new TContext Context
        {
            get { return (TContext) base.Context; }
        }

        protected override object CreateContextObject()
        {
            return new TContext();
        }
    }
}