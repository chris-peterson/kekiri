using System;
using System.Collections.Generic;
using Kekiri.NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Kekiri.Examples.NUnit
{
    public class Fixture_reuse_after_a_failed_scenario : Scenarios<ReusedFixtureContext>
    {
        [Scenario]
        public void A_failed_scenario_does_not_leak_its_steps_into_the_next_case()
        {
            Given(A_fixture_whose_first_scenario_failed);
            When(Running_the_second_scenario_on_the_same_fixture);
            Then(It_should_have_run_each_scenarios_steps_once);
        }

        void A_fixture_whose_first_scenario_failed()
        {
            Context.Fixture = new ReusedFixture();

            Assert.Throws<AggregateException>(() => RunAsNUnitWould(
                Context.Fixture, nameof(ReusedFixture.First_scenario_that_fails)));
        }

        void Running_the_second_scenario_on_the_same_fixture()
        {
            RunAsNUnitWould(Context.Fixture, nameof(ReusedFixture.Second_scenario_that_passes));
        }

        void It_should_have_run_each_scenarios_steps_once()
        {
            Assert.That(Context.Fixture.StepsRun, Is.EqualTo(new[]
            {
                nameof(ReusedFixture.Setting_up_the_first_scenario),
                nameof(ReusedFixture.Doing_the_first_deed),
                nameof(ReusedFixture.It_should_fail),
                nameof(ReusedFixture.Setting_up_the_second_scenario),
                nameof(ReusedFixture.Doing_the_second_deed),
                nameof(ReusedFixture.It_should_pass)
            }));
        }

        // NUnit invokes the [Scenario] method, which only declares the steps, and then
        // runs the attribute's ITestAction.AfterTest -- that is where Kekiri executes them.
        static void RunAsNUnitWould(ReusedFixture fixture, string scenarioMethod)
        {
            fixture.GetType()
                .GetMethod(scenarioMethod)
                .Invoke(fixture, null);

            new ScenarioAttribute().AfterTest(
                new TestMethod(new MethodWrapper(fixture.GetType(), scenarioMethod))
                {
                    Fixture = fixture
                });
        }
    }

    public class ReusedFixtureContext
    {
        public ReusedFixture Fixture { get; set; }
    }

    // Stands in for a user's fixture class: NUnit builds one instance and reuses it for
    // every case in the class.  Not discovered as a fixture itself -- it derives from
    // ScenarioBase rather than Scenarios, and its methods carry no test attribute.
    public class ReusedFixture : ScenarioBase
    {
        public readonly List<string> StepsRun = new List<string>();

        public void First_scenario_that_fails()
        {
            Given(Setting_up_the_first_scenario);
            When(Doing_the_first_deed);
            Then(It_should_fail);
        }

        public void Second_scenario_that_passes()
        {
            Given(Setting_up_the_second_scenario);
            When(Doing_the_second_deed);
            Then(It_should_pass);
        }

        public void Setting_up_the_first_scenario() => StepsRun.Add(nameof(Setting_up_the_first_scenario));

        public void Doing_the_first_deed() => StepsRun.Add(nameof(Doing_the_first_deed));

        public void It_should_fail()
        {
            StepsRun.Add(nameof(It_should_fail));
            throw new InvalidOperationException("this scenario fails on purpose");
        }

        public void Setting_up_the_second_scenario() => StepsRun.Add(nameof(Setting_up_the_second_scenario));

        public void Doing_the_second_deed() => StepsRun.Add(nameof(Doing_the_second_deed));

        public void It_should_pass() => StepsRun.Add(nameof(It_should_pass));
    }
}
