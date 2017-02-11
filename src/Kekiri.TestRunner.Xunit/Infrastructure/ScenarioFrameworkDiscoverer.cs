using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    [TestFrameworkDiscovererAttribute("Kekiri.TestRunner.Infrastructure.ScenarioFrameworkDiscoverer", "Kekiri.TestRunner.Xunit")]
    public class ScenarioFrameworkDiscoverer : XunitTestFrameworkDiscoverer
    {
        public ScenarioFrameworkDiscoverer(IAssemblyInfo assemblyInfo,
            ISourceInformationProvider sourceProvider,
            IMessageSink diagnosticMessageSink,
            IXunitTestCollectionFactory collectionFactory = null)
            : base(assemblyInfo, sourceProvider, diagnosticMessageSink, collectionFactory)
        {
        }

        protected override bool FindTestsForType(ITestClass testClass,
            bool includeSourceInformation,
            IMessageBus messageBus,
            ITestFrameworkDiscoveryOptions discoveryOptions)
        {
            var exampleAttributes =
                testClass.Class.ToRuntimeType()
                    .GetTypeInfo()
                    .GetCustomAttributes<ExampleAttribute>()
                    .ToArray();

            if (!exampleAttributes.Any())
            {
                return base.FindTestsForType(testClass, includeSourceInformation, messageBus, discoveryOptions);
            }

            var testMethod = GetTestMethod(testClass);

            foreach (var exampleAttribute in exampleAttributes)
            {
                var testCase = new ExampleTestCase(exampleAttribute, DiagnosticMessageSink, TestMethodDisplay.ClassAndMethod, testMethod);

                ReportDiscoveredTestCase(testCase, includeSourceInformation, messageBus);
            }

            return true;
        }

        private ITestMethod GetTestMethod(ITestClass testClass)
        {
            var runMethod = testClass.Class.GetMethod("Run", includePrivateMethod:false);

            return new TestMethod(testClass, runMethod);
        }
    }
}