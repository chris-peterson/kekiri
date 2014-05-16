using System;

namespace Kekiri.TestSupport.Scenarios.Fluent
{
    public class Throws_exception : FluentTest
    {
        public Throws_exception()
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