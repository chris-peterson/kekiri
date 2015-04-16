using System;
using System.Reflection;

namespace Kekiri.IoC
{
    public abstract class IoCFluentTest : FluentTest, IContainerAccessor
    {
        protected internal Container Container;

        protected IoCFluentTest(Container container)
        {
            Container = container;
        }

        Container IContainerAccessor.Container
        {
            get { return Container; }
        }
    }

    public abstract class IoCFluentTest<TContext> : FluentTest, IContainerAccessor where TContext : class
    {
        protected internal Container Container;

        protected IoCFluentTest(Container container)
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
            var defaultConstructor = typeof (TContext).GetConstructor(Type.EmptyTypes);
            return defaultConstructor == null ? Container.Resolve<TContext>() : defaultConstructor.Invoke(null);
        }
    }
}