namespace Kekiri.Internal.Exceptions;

class FixtureShouldHaveThens : ScenarioException
{
    public FixtureShouldHaveThens(ScenarioBase scenario)
        : base(scenario, "No thens found; call Then in the scenario method")
    {
    }
}
