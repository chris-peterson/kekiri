namespace Kekiri.Internal.Exceptions;

class FixtureShouldHaveWhens : ScenarioException
{
    public FixtureShouldHaveWhens(ScenarioBase scenario)
        : base(scenario, "No whens found; call When in the scenario method")
    {
    }
}
