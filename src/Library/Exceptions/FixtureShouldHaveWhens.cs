namespace Kekiri.Exceptions
{
    class FixtureShouldHaveWhens : ScenarioException
    {
        public FixtureShouldHaveWhens(object test)
            : base(test, "No whens found; a when should be specified by calling When in the constructor")
        {
        }
    }
}