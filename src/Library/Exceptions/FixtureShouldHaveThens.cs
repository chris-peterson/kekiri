namespace Kekiri.Exceptions
{
    class FixtureShouldHaveThens : ScenarioException
    {
        public FixtureShouldHaveThens(object test)
            : base(test, "No thens found; a then should be specified by calling Then in the constructor")
        {
        }
    }
}