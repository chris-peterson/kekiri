using NUnit.Framework;

namespace Kekiri.TestRunner.NUnit
{
    public class ExampleAttribute : TestFixtureAttribute, IExampleAttribute
    {
        public ExampleAttribute(params object [] values) : base(values)
        {
        }
    }
}