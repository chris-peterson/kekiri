namespace Behavior;

/// <summary>
/// One-time setup for the whole run, which is where container bootstrapping goes. Implement it
/// on any public class with a parameterless constructor; the runner finds it and calls
/// <see cref="Setup"/> once, before the first scenario. The shape matches NUnit's
/// [SetUpFixture]/[OneTimeSetUp] pair.
/// </summary>
public interface IBeforeTestRun
{
    void Setup();
}
