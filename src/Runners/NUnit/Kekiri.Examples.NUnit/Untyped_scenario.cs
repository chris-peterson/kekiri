using Kekiri.NUnit;
using NUnit.Framework;

namespace Kekiri.Examples.NUnit
{
    public class Untyped_scenario : Scenarios
    {
        [Scenario]
        public void Can_use_untyped_scenario()
        {
            Given(Precondition_1);
            When(Doing_the_deed);
            Then(It_should_do_it);
        }

        void Precondition_1()
        {
            Container.Register(new FakeRepository());
        }

        void Doing_the_deed()
        {
            Context.Result =
                Container.Resolve<Service>()
                    .DoWork();
        }

        void It_should_do_it()
        {
            Assert.That(Context.Result, Is.EqualTo("data"));
        }
    }

    class Service
    {
        readonly IRepository _repository;

        public Service(IRepository repository)
        {
            _repository = repository;
        }

        public string DoWork()
        {
            return _repository.GetData();
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
