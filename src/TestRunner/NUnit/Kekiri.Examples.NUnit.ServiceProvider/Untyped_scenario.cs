using System;
using Kekiri.NUnit;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    public class Untyped_scenario : Scenarios
    {
        [Scenario]
        public void Cannot_use_jit_service_registrations()
        {
            When(Attempting_Jit_service_registration).Throws();
            Then(An_exception_is_thrown);
        }

        void Attempting_Jit_service_registration()
        {
            Container.Register(new FakeRepository());
        }

        void An_exception_is_thrown()
        {
            var ex = Catch<Exception>();

            Assert.AreEqual("ServiceProviderContainer does not allow just-in-time service registrations", ex.Message);
        }
    }

    interface IRepository
    {
        string GetData();
    }

    class FakeRepository : IRepository
    {
        public string GetData()
        {
            return "data";
        }
    }
}