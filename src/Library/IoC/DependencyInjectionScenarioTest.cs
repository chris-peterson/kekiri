namespace Kekiri.IoC
{
    /// <summary>
    /// Base class for a DI-aware <c>ScenarioTest</c>.
    /// </summary>
    public abstract class DependencyInjectionScenarioTest : ScenarioTest
    {
        protected DepenencyInjectionContainer Container;

        protected DependencyInjectionScenarioTest(DepenencyInjectionContainer container)
        {
            Container = container;
        }
    }
}