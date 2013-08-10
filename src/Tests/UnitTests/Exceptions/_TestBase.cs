namespace Kekiri.UnitTests.Exceptions
{
    public abstract class ExceptionScenarioTest : ScenarioTest
    {
    }

    [Feature("Fixture exception handling", "In order to facilitate rapid diagnosis of failing test initialization,", "The framework provides exceptions with rich information to diagnose the problem")]
    public class FixtureExceptionScenarioTest : ExceptionScenarioTest
    {
    }

    [Feature("Test exception handling", "In order to facilitate rapid diagnosis of a failing test,", "The framework provides rich information by way of exceptions")]
    public class TestExceptionScenarioTest : ExceptionScenarioTest
    {
    }
}