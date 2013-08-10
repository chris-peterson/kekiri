using System.Reflection;

namespace Kekiri.Exceptions
{
    public class WhensShouldBePublic : ScenarioTestException
    {
        public WhensShouldBePublic(ScenarioTest test, MethodBase nonPublicGiven)
            : base(test, string.Format("'{0}' is not public", nonPublicGiven.Name))
        {
        }
    }
}