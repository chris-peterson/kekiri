namespace Kekiri.Impl.Exceptions
{
    class FixtureShouldHaveThens : ScenarioException
    {
        public FixtureShouldHaveThens(ScenarioBase scenario)
            : base(scenario, "No thens found; a then should be specified by calling Then in the constructor")
        {
        }
    }
}