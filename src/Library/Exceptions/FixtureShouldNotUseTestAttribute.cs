using System.Reflection;

namespace Kekiri.Exceptions
{
    internal class FixtureShouldNotUseTestAttribute : ScenarioTestException
    {
        public FixtureShouldNotUseTestAttribute(MemberInfo method) : base(method.DeclaringType, string.Format("{0} should use [Then] rather than [Test]", method.Name))
        {
        }
    }
}