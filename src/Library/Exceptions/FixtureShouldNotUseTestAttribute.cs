using System.Reflection;

namespace Kekiri.Exceptions
{
    public class FixtureShouldNotUseTestAttribute : ScenarioTestException
    {
        public FixtureShouldNotUseTestAttribute(ScenarioTest test, MethodBase method) : base(test, string.Format("{0} should use [Then] rather than [Test]", method.Name))
        {
        }
    }
}