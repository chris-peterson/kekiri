using Kekiri.IoC.Autofac;
using Kekiri.Xunit;
using System.Threading.Tasks;

namespace Kekiri.Examples.Xunit
{
    public class ExampleScenarios : Scenarios
    {
        protected override Task BeforeAsync()
        {
            return BootstrapHelper.EnsureBootstrapped();
        }
    }

    public class ExampleScenariosTyped<T> : Scenarios<T>
    {
        protected override Task BeforeAsync()
        {
            return BootstrapHelper.EnsureBootstrapped();
        }
    }

    public static class BootstrapHelper
    {
        static readonly object _lockObject = new object();
        static bool _isInitialized = false;

        public static Task EnsureBootstrapped()
        {
            if (!_isInitialized)
            {
                lock (_lockObject)
                {
                    if (!_isInitialized)
                    {
                        AutofacBootstrapper.Initialize();
                        _isInitialized = true;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}