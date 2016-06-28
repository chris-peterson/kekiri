using Kekiri.IoC.Autofac;

namespace $rootnamespace$
{
    #error Integrate with your test runner, e.g. uncomment the following line for NUnit
    //[SetUpFixture]
    public class AutofacTests
    {
        #error Integrate with your test runner, e.g. uncomment the following line for NUnit
        //[SetUp]
        public void RunBeforeAnyTests()
        {
            AutofacBootstrapper.Initialize(c => 
               //c.WithModules(...)
               //.AssemblyScanning(...)
            );
        }
    }
}
