using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    public class ScenarioFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public ScenarioFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink) : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {

        }

        protected override ITestFrameworkDiscoverer CreateDiscoverer()
        {
            return new ScenarioFrameworkDiscoverer(AssemblyInfo, SourceInformationProvider, DiagnosticMessageSink);
        }

        protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            var runner = new ScenarioAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink,
                executionOptions);

            try
            {
                await runner.RunAsync();
            }
            finally
            {
                runner.Dispose();
            }
        }
    }
}