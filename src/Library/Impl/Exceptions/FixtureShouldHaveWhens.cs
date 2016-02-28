namespace Kekiri.Impl.Exceptions
{
    class FixtureShouldHaveWhens : ScenarioException
    {
        public FixtureShouldHaveWhens(ScenarioBase scenario)
            : base(scenario, "No whens found; a when should be specified by calling When in the constructor")
        {
        }
    }
}