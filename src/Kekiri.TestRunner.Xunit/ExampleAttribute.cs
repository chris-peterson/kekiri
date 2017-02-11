using System;

namespace Kekiri.TestRunner.Xunit
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExampleAttribute : Attribute, IExampleAttribute
    {
        public object[] Values { get; }

        public ExampleAttribute(params object[] values)
        {
            Values = values;
        }
    }
}