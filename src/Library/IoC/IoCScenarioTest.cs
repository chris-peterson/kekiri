namespace Kekiri.IoC
{
    /// <summary>
    /// Base class for an IoC capable <c>ScenarioTest</c>.
    /// </summary>
    public abstract class IoCScenarioTest : ScenarioTest
    {
        protected Container Container;

        protected IoCScenarioTest(Container container)
        {
            Container = container;
        }
    }
}