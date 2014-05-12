namespace Kekiri.Exceptions
{
    internal class FixtureShouldHaveThens : ScenarioTestException
    {
        public FixtureShouldHaveThens(object test)
            : base(test, string.Format("No thens found; thens should be parameterless public methods that start use [Then] attribute"))
        {
        }
    }
}