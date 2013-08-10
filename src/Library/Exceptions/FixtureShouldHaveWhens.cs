namespace Kekiri.Exceptions
{
    public class FixtureShouldHaveWhens : ScenarioTestException
    {
        public FixtureShouldHaveWhens(ScenarioTest test)
            : base(test, string.Format("No whens found; whens should be parameterless public methods that use the [When] attribute"))
        {
        }
    }
}