using System;
using FluentAssertions;
using Kekiri.Impl;
using Kekiri.Impl.Exceptions;
using Kekiri.TestSupport.Reporting.Targets;
using NUnit.Framework;

namespace Kekiri.UnitTests.Exceptions
{
    [TestFixture]
    public class No_when
    {
        [Ignore]
        public class FluentFixture : ScenarioBase
        {
            public FluentFixture()
            {
                Given(() => { });
                Then(() => { });
            }
        }

        [Test]
        public void Fluent_fixture()
        {
            RunTest(new FluentFixture())
                .Should().Contain("calling When in the constructor");
        }

        [Ignore]
        public class StandardFixture : Test
        {
            [Given]
            public void Given() { }

            [Then]
            public void Then() { }
        }

        [Test]
        public void Standard_fixture()
        {
            RunTest(new StandardFixture())
                .Should().Contain("parameterless public method that uses the [When] attribute");
        }

        private static string RunTest(object test)
        {
            try
            {
                var reportingTarget = new StringReportTarget();
                new ScenarioRunner(test, reportingTarget).Run();

                throw new Exception("wrong exception");
            }
            catch (FixtureShouldHaveWhens ex)
            {
                return ex.Message;
            }
        }
    }
}