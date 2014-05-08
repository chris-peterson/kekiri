namespace Kekiri.IoC
{
    /// <summary>
    /// Base class for an IoC capable <c>ScenarioTest</c>.
    /// </summary>
    public abstract class IoCScenarioTest : ScenarioTest, IContainerAccessor
    {
        protected internal Container Container;

        protected IoCScenarioTest(Container container)
        {
            Container = container;
        }

        Container IContainerAccessor.Container
        {
            get { return Container; }
        }
    }
}