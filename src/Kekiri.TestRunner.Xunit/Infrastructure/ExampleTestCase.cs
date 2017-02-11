using System;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kekiri.TestRunner.Xunit.Infrastructure
{
    public class ExampleTestCase : XunitTestCase
    {
        public ExampleAttribute Example { get; private set; }

        [Obsolete("For deserialization only")]
        public ExampleTestCase()
        {
        }

        public ExampleTestCase(ExampleAttribute example, IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, object[] testMethodArguments = null) : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, testMethodArguments)
        {
            Example = example;
        }

        protected override string GetDisplayName(IAttributeInfo factAttribute, string displayName)
        {
            displayName = TestMethod.Method.GetDisplayNameWithArguments(displayName, Example.Values, MethodGenericTypes);
            var parameters = this.TestMethod.TestClass.Class
                .ToRuntimeType()
                .GetTypeInfo()
                .GetConstructors()
                .Single(c => c.GetParameters().Length == Example.Values.Length)
                .GetParameters();
            for (int i = 0; i < Example.Values.Length; i++)
            {
                displayName = displayName.ReplaceFirst("???:", $"{parameters[i].Name}:");
            }

            return displayName;
        }

        public override void Serialize(IXunitSerializationInfo data)
        {
            data.AddValue("ExampleValues", Example.Values);
            base.Serialize(data);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            var exampleValues = data.GetValue<object[]>("ExampleValues");
            Example = new ExampleAttribute(exampleValues);
            base.Deserialize(data);
        }
    }
}