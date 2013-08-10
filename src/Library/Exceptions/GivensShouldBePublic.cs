using System.Reflection;

namespace Kekiri.Exceptions
{
    public class GivensShouldBePublic : ScenarioTestException
    {
        public GivensShouldBePublic(ScenarioTest test, MethodBase nonPublicGiven)
            : base(test, string.Format("'{0}' is not public", nonPublicGiven.Name))
        {
        }
    }
}