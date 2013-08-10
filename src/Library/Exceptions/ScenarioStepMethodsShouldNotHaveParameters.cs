namespace Kekiri.Exceptions
{
    public class ScenarioStepMethodsShouldNotHaveParameters : ScenarioTestException
    {
        public ScenarioStepMethodsShouldNotHaveParameters(ScenarioTest test, string message) :
            base(test, message)
        {
        }
    }
}