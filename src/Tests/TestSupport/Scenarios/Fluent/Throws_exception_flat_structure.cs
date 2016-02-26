using System;

namespace Kekiri.TestSupport.Scenarios.Fluent
{
    public class Throws_exception_flat_structure : ScenarioBase
    {
        public Throws_exception_flat_structure()
        {
            When(Doing_the_deed).Throws();
            Then(The_correct_exception_occurs);
        }

        private void The_correct_exception_occurs()
        {
            Catch<ApplicationException>();
        }

        private void Doing_the_deed()
        {
            throw new ApplicationException();
        }
    }
}