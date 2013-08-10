using System.Reflection;

namespace Kekiri.Exceptions
{
    public class ThensShouldBePublic : ScenarioTestException
    {
        public ThensShouldBePublic(ScenarioTest test, MethodBase nonPublicGiven)
            : base(test, string.Format("'{0}' is not public", nonPublicGiven.Name))
        {
        }
    }
}