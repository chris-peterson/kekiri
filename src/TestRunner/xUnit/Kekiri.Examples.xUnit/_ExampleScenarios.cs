using Kekiri.IoC.Autofac;
using Kekiri.Xunit;
using System.Threading.Tasks;

namespace Kekiri.Examples.Xunit
{
    public class ExampleScenarios : Scenarios
    {
        static readonly object _lockObject = new object();
        static bool _isInitialized = false;

        protected override Task BeforeAsync()
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