using System;

namespace Kekiri.TestSupport.Scenarios.Fluent
{
    class Throws_exception_with_classes : ScenarioBase
    {
        public Throws_exception_with_classes()
        {
            When<Doing_the_deed>().Throws();
            Then<The_correct_exception_occurs>();
        }
    }

    class Doing_the_deed : Step
    {
        public override void Execute()
        {
            throw new ApplicationException();
        }
    }

    class The_correct_exception_occurs : Step
    {
        public override void Execute()
        {
            Catch<ApplicationException>();
        }
    }
}
