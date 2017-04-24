using Kekiri.TestRunner.xUnit;
using Xunit;

namespace Kekiri.Examples.xUnit
{
    public class Untyped_scenario : ExampleScenarios
    {
        [Scenario]
        public void Can_resolve()
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
            Assert.Equal("data", Context.Result);
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
