﻿using System;
using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    public class Untyped_scenario : ExampleScenarios
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

            Assert.Equal("ServiceProviderContainer does not allow just-in-time service registrations", ex.Message);
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