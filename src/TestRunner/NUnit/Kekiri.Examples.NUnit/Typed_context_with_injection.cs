using System.Collections.Generic;
using System.Linq;
using Kekiri.TestRunner.NUnit;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    public class Typed_context_with_injection : Scenario<AdvancedCalculatorContext>
    {
        public Typed_context_with_injection()
        {
            Given(Two_integers);
            When(Adding);
            Then(It_should_have_correct_result);
        }

        void Two_integers()
        {
            Context.GetOperands(1);
            Context.GetOperands(2);
        }

        void Adding()
        {
            Context.ComputeSum();
        }

        void It_should_have_correct_result()
        {
            Assert.That(Context.Result, Is.EqualTo(3));
        }
    }

    public interface IDataRepository
    {
        IEnumerable<int> GetOperands();
    }

    public class FakeDataResository : IDataRepository
    {
        public IEnumerable<int> GetOperands()
        {
            return new[] { 1, 2 };
        }
    }

    public class AdvancedCalculatorContext
    {
        readonly IDataRepository _repository;

        public AdvancedCalculatorContext(FakeDataResository repository)
        {
            _repository = repository;
        }

        public void GetOperands(int operand)
        {
            Operands = _repository.GetOperands();
        }

        public IEnumerable<int> Operands { get; set; }

        public void ComputeSum()
        {
            Result = Operands.Sum();
        }

        public int Result { get; set; }
    }
}