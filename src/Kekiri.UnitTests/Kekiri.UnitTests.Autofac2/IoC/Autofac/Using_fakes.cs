using System;
using System.Data.SqlClient;
using FluentAssertions;
using Kekiri;
using Kekiri.IoC.Autofac;

namespace UnitTests.Autofac2.IoC.Autofac
{
    public class Using_fakes : AutofacTest
    {
        private Orchestrator _orchestrator;

        public class Orchestrator
        {
            public Validator Validator { get; private set; }
            public Executor Executor { get; private set; }
            public IDataComponent DataComponent { get; private set; }

            public Orchestrator(Validator validator, Executor executor, IDataComponent dataComponent)
            {
                Validator = validator;
                Executor = executor;
                DataComponent = dataComponent;
            }

            public int Process()
            {
                var data = DataComponent.GetData();

                Validator.Validate(data);
                return Executor.Execute(data);
            }
        }

        public class Validator
        {
            public void Validate(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentException("Must have a value", "input");
                }
            }
        }

        public class Executor
        {
            public WordCounter WordCounter { get; set; }

            public Executor(WordCounter wordCounter)
            {
                WordCounter = wordCounter;
            }

            public int Execute(string input)
            {
                return WordCounter.CountWords(input);
            }
        }

        public class WordCounter
        {
            public int CountWords(string sentence)
            {
                return sentence.Split(' ').Length;
            }
        }

        public interface IDataComponent
        {
            string GetData();
        }

        public class RealDataComponent : IDataComponent
        {
            public string GetData()
            {
                using (var connection = new SqlConnection("[YourDatabase]"))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT YourField from [YourTable]";
                        return (string)command.ExecuteScalar();
                    }
                }
            }
        }

        public class FakeDataComponent : IDataComponent
        {
            public string GetData()
            {
                return "all your base are belong to us";
            }
        }

        [Given]
        public void Given()
        {
            Container.Register(new FakeDataComponent());
        }

        [When]
        public void When_resolving_an_instance()
        {
            _orchestrator = Container.Resolve<Orchestrator>();
        }

        [Then]
        public void It_uses_reals_for_everything()
        {
            _orchestrator.Validator.Should().BeOfType<Validator>();
            _orchestrator.Executor.Should().BeOfType<Executor>();
            _orchestrator.Executor.WordCounter.Should().BeOfType<WordCounter>();
        }

        [Then]
        public void But_explicitly_faked_objects()
        {
            _orchestrator.DataComponent.Should().BeOfType<FakeDataComponent>();
        }

        [Then]
        public void And_it_computes_the_right_result()
        {
            _orchestrator.Process().Should().Be(7);
        }
    }
}