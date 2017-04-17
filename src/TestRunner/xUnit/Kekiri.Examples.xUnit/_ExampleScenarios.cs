using Kekiri.IoC.Autofac;
using Kekiri.TestRunner.xUnit;

namespace Kekiri.Examples.xUnit
{
    public class ExampleScenarios : Scenarios
    {
        static readonly object _lockObject = new object();
        static bool _isInitialized = false;

        protected override void Before()
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
        }
    }
}