using NUnit.Framework;

namespace Kekiri
{
    public class ExampleAttribute : TestFixtureAttribute
    {
        public ExampleAttribute(params object [] values) : base(values)
        {
        }
    }
}