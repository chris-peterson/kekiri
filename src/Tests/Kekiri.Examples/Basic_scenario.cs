
using Kekiri.NUnit;

namespace Kekiri.Examples
{
    public class Basic_scenario : Scenario
    {
        public Basic_scenario()
        {
            Given(Precondition_1);
            When(Doing_the_deed);
            Then(It_should_do_it);
        }

        void Precondition_1()
        {
        }

        void Doing_the_deed()
        {
        }

        void It_should_do_it()
        {
        }
    }
}
