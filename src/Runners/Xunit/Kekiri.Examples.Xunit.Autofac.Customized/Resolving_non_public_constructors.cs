using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    /// <summary>
    /// The other half of Kekiri.Examples.Xunit's Autofac_constructor_visibility: with
    /// IncludeNonPublicConstructors, a domain type that keeps its constructor internal is
    /// auto-registered and activated. Several Getty services hand-rolled this finder to get here.
    /// </summary>
    public class Resolving_non_public_constructors : Scenarios
    {
        [Scenario]
        public void An_internally_constructed_type_is_resolved()
        {
            When(resolving_the_internally_constructed_type);
            Then(it_is_resolved);
        }

        [Scenario]
        public void Its_dependencies_are_injected_through_the_non_public_constructor()
        {
            When(resolving_the_type_with_an_internal_dependency_constructor);
            Then(the_dependency_was_injected);
        }

        void resolving_the_internally_constructed_type()
        {
            Context.Resolved = Container.Resolve<InternallyConstructed>();
        }

        void resolving_the_type_with_an_internal_dependency_constructor()
        {
            Context.Resolved = Container.Resolve<InternallyConstructedWithDependency>();
        }

        void it_is_resolved()
        {
            Assert.NotNull(Context.Resolved);
        }

        void the_dependency_was_injected()
        {
            var resolved = (InternallyConstructedWithDependency)Context.Resolved;

            Assert.NotNull(resolved.Dependency);
        }
    }

    public class InternallyConstructed
    {
        internal InternallyConstructed() { }
    }

    public class InternallyConstructedWithDependency
    {
        internal InternallyConstructedWithDependency(InternallyConstructed dependency)
        {
            Dependency = dependency;
        }

        public InternallyConstructed Dependency { get; }
    }
}
