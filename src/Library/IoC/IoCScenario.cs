using System;

namespace Kekiri.IoC
{
    public abstract class IoCScenario : ScenarioBase, IContainerAccessor
    {
        protected internal Container Container;

        protected IoCScenario(Container container)
        {
            Container = container;
        }

        Container IContainerAccessor.Container => Container;
    }

    public abstract class IoCScenario<TContext> : ScenarioBase, IContainerAccessor where TContext : class
    {
        protected internal Container Container;

        protected IoCScenario(Container container)
        {
            Container = container;
        }

        Container IContainerAccessor.Container => Container;

        protected new TContext Context => (TContext) base.Context;

        protected override object CreateContextObject()
        {
            var defaultConstructor = typeof (TContext).GetConstructor(Type.EmptyTypes);
            return defaultConstructor == null ? Container.Resolve<TContext>() : defaultConstructor.Invoke(null);
        }
    }
}