using FluentAssertions;
using Kekiri;
using Kekiri.IoC.Autofac;
using Moq;

namespace UnitTests.Autofac2.IoC.Autofac
{
    [ScenarioBase(Feature.IoC)]
    public class Using_mocks : AutofacTest
    {
        private string _result;

        public interface ISimpleFeature
        {
            string TellMeASecret();
        }

        public class Executor
        {
            private readonly ISimpleFeature _simpleFeature;

            public Executor(ISimpleFeature simpleFeature)
            {
                _simpleFeature = simpleFeature;
            }

            public string Execute()
            {
                return _simpleFeature.TellMeASecret();
            }
        }

        [Given]
        public void Given()
        {
            var mock = new Mock<ISimpleFeature>();
            mock.Setup(x => x.TellMeASecret()).Returns("I did it!");
            Container.Register(mock);
        }

        [When]
        public void When()
        {
            var executor = Container.Resolve<Executor>();
            _result = executor.Execute();
        }

        [Then]
        public void It_should_get_the_correct_result()
        {
            _result.Should().Be("I did it!");
        }
    }
}