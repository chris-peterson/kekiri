using System;
using FluentAssertions;
using Kekiri;
using Kekiri.IoC.SimpleInjector;
using Moq;
using NUnit.Framework;
using SimpleInjector;

namespace UnitTests.SimpleInjector
{
    [ScenarioBase(Feature.IoC)]
    public class Using_mocks : SimpleInjectorTest
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

        [TestFixtureSetUp]
        public void Setup()
        {
            IocConfig.BuildContainer = () =>
            {
                var container = new Container();
                container.Register<Executor>();
                return container;
            };
        }

        [Given]
        public void Given()
        {
            var mock = new Mock<ISimpleFeature>();
            mock.Setup(x => x.TellMeASecret()).Returns("I did it!");
            Container.Register(mock.Object);
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