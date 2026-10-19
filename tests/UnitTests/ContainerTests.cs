using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kekiri.Tests;

public class ContainerTests
{
    [Test]
    public async Task Registered_instances_are_the_fakes_a_container_resolves_with()
    {
        var container = new Recording_container();
        var first = new object();
        var second = new object();

        container.Register(first, second);

        await Assert.That(container.Registered.SequenceEqual(new[] { first, second })).IsTrue();
    }

    [Test]
    public async Task Registering_after_resolving_is_refused()
    {
        var container = new Recording_container();
        container.Resolve<object>();

        var failure = Assert.Throws<Exception>(() => container.Register(new object()));

        await Assert.That(failure.Message).IsEqualTo("Cannot call Register after Resolve");
    }

    [Test]
    public async Task A_null_registration_is_refused()
    {
        var container = new Recording_container();

        Assert.Throws<ArgumentNullException>(() => container.Register((object)null));

        await Assert.That(container.Registered.Count()).IsEqualTo(0);
    }

    /// <summary>
    /// Registering a Moq Mock registers the object it mocks. The check is by name so Kekiri
    /// takes no dependency on Moq.
    /// </summary>
    [Test]
    public async Task A_mock_registers_the_object_it_mocks()
    {
        var container = new Recording_container();
        var mock = new Moq_shaped.Mock<IDisposable>();

        container.Register(mock);

        await Assert.That(ReferenceEquals(container.Registered.Single(), mock.Object)).IsTrue();
    }

    [Test]
    public async Task A_type_named_mock_without_an_object_is_registered_as_itself()
    {
        var container = new Recording_container();
        var notReallyAMock = new Not_moq.Mock();

        container.Register(notReallyAMock);

        await Assert.That(ReferenceEquals(container.Registered.Single(), notReallyAMock)).IsTrue();
    }

    sealed class Recording_container : Container
    {
        public System.Collections.Generic.IEnumerable<object> Registered => Fakes;

        protected override T OnResolve<T>() => null;
    }

    /// <summary>
    /// Shaped like Moq's: a generic Mock&lt;T&gt; deriving from a non-generic Mock that carries
    /// the Object property.
    /// </summary>
    public static class Moq_shaped
    {
        public abstract class Mock
        {
            public object Object { get; } = new object();
        }

        public sealed class Mock<T> : Mock
        {
        }
    }

    public static class Not_moq
    {
        public sealed class Mock
        {
        }
    }
}
