using System;
using SIContainer = SimpleInjector.Container;

namespace Kekiri.IoC.SimpleInjector
{
    internal class SimpleInjectorContainer : Container
    {
        private bool _fakesRegistered;

        private static readonly Lazy<SIContainer> Container = new Lazy<SIContainer>(() =>
        {
            return IocConfig.BuildContainer == null
                ? new SIContainer()
                : IocConfig.BuildContainer();
        });

        protected override T OnResolve<T>()
        {
            var container = Container.Value;

            if (!_fakesRegistered)
            {
                foreach (var obj in Fakes)
                {
                    container.Register(obj.GetType(), () => obj);
                    foreach (var i in obj.GetType().GetInterfaces())
                        container.Register(i, () => obj);
                }
                _fakesRegistered = true;
            }

            return Container.Value.GetInstance<T>();
        }
    }
}