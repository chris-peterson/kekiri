using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace Kekiri.TestRunner.xUnit
{
    [DataDiscoverer("Xunit.Sdk.InlineDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : DataAttribute
    {
        private readonly object[] _data;

        public ExampleAttribute(params object[] data)
        {
            _data = data;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            return new object[1][]
            {
                _data
            };
        }
    }
}