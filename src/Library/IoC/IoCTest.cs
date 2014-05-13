namespace Kekiri.IoC
{
    /// <summary>
    /// Base class for an IoC capable <c>ScenarioTest</c>.
    /// </summary>
    public abstract class IoCTest : Test, IContainerAccessor
    {
        protected internal Container Container;

        protected IoCTest(Container container)
        {
            Container = container;
        }

        Container IContainerAccessor.Container
        {
            get { return Container; }
        }
    }
}