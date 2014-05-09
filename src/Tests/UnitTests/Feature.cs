namespace Kekiri.UnitTests
{
    public enum Feature
    {
        [Feature("Customized fixture setup and teardown",
            "In order to allow developers to do custom steps during a fixture setup or teardown,",
            "The framework has a way of overriding the default behavior")]
        SetupAndTeardown,

        [Feature("Subclassed test fixtures",
            "In order to allow developers to have the freedom to structure test fixtures in a type hierarchy of their chosing,",
            "The framework supports subclassed ScenarioTest classes")]
        Subclassing,

        [Feature("Happy path test execution",
            "In order to be a viable test framework, it certainly must support tests that perform as expected, i.e. pass")]
        ExecuteTests,

        [Feature("Report generation", "In order to view the business rules of the system under test,",
            "The framework supports generating a human-readable report using Gherkin language semantics")]
        Reporting,

        [Feature("Fixture exception handling",
            "In order to facilitate rapid diagnosis of failing test initialization,",
            "The framework provides exceptions with rich information to diagnose the problem")]
        FixtureExceptionHandling,

        [Feature("Test exception handling",
            "In order to facilitate rapid diagnosis of a failing test,",
            "The framework provides rich information by way of exceptions")]
        TestExceptionHandling
    }
}