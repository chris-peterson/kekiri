using NUnit.Framework;

namespace Kekiri.NUnit
{
    [TestFixture]
    public class Scenarios : ScenarioBase
    {
    }

    [TestFixture]
    public class Scenarios<TContext> : ScenarioBase<TContext>
    {
    }
}