using System;
using Autofac.Core.Registration;
using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    /// <summary>
    /// Pins the default auto-registration contract. Autofac's default constructor finder considers
    /// only public constructors, and a type it finds none on is skipped — registering such a type
    /// makes building the whole container throw. Opt in with
    /// <c>AutofacBootstrapper.Initialize(x =&gt; x.IncludeNonPublicConstructors())</c>; see
    /// Kekiri.Examples.Xunit.Autofac.Customized for that side.
    /// </summary>
    public class Autofac_constructor_visibility : Scenarios
    {
        [Scenario]
        public void A_type_with_a_public_constructor_is_registered()
        {
            When(resolving_the_publicly_constructed_type);
            Then(it_is_resolved);
        }

        [Scenario]
        public void A_type_with_only_a_non_public_constructor_is_skipped()
        {
            When(resolving_the_internally_constructed_type).Throws();
            Then(it_was_not_registered);
        }

        void resolving_the_publicly_constructed_type()
        {
            Context.Resolved = Container.Resolve<PubliclyConstructed>();
        }

        void resolving_the_internally_constructed_type()
        {
            Container.Resolve<InternallyConstructed>();
        }

        void it_is_resolved()
        {
            Assert.NotNull(Context.Resolved);
        }

        void it_was_not_registered()
        {
            Assert.IsType<ComponentNotRegisteredException>(Catch<Exception>());
        }
    }

    public class PubliclyConstructed
    {
        public PubliclyConstructed() { }
    }

    public class InternallyConstructed
    {
        internal InternallyConstructed() { }
    }
}
