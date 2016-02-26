using NUnit.Framework;

namespace Kekiri.NUnit
{
    public class ExampleAttribute : TestFixtureAttribute, IExampleAttribute
    {
        public ExampleAttribute(params object [] values) : base(values)
        {
        }
    }
}