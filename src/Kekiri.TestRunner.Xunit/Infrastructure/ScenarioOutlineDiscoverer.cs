using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    public class ScenarioOutlineDiscoverer : TheoryDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public ScenarioOutlineDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
            IAttributeInfo theoryAttribute, object[] dataRow)
        {
            if(!typeof(Scenarios).GetTypeInfo().IsAssignableFrom(testMethod.TestClass.Class.ToRuntimeType()))
                throw new NotSupportedException("The ScenarioOutline attribute can only be placed on a class inheriting from Kekiri.TestRunner.Xunit.Scenarios");

            return new IXunitTestCase[] {new ScenarioTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, dataRow)};
        }
    }
}