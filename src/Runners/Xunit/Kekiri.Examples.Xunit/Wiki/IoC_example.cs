// Supports https://github.com/chris-peterson/kekiri/wiki/IoC-Support
using System;
using System.Data.SqlClient;
using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit
{
    public class Orchestrator_scenarios : ExampleScenarios
    {
        Orchestrator _orchestrator;

        [Scenario]
        public void Using_fakes_with_autofac()
        {
            Given(Fake_data_access);
            When(resolving_an_instance);
            Then(It_uses_reals_for_everything)
                .But(data_access_object_is_fake)
                .And(it_computes_the_right_result);
        }

        void Fake_data_access() {
            Container.Register(new FakeDataComponent());
        }

        void resolving_an_instance() {
            _orchestrator = Container.Resolve<Orchestrator>();
        }

        void It_uses_reals_for_everything() {
            Assert.IsType<Validator>(_orchestrator.Validator);
            Assert.IsType<Executor>(_orchestrator.Executor);
            Assert.IsType<WordCounter>(_orchestrator.Executor.WordCounter);
        }

        void data_access_object_is_fake() {
            Assert.IsType<FakeDataComponent>(_orchestrator.DataComponent);
        }

        void it_computes_the_right_result() {
            Assert.Equal(7, _orchestrator.Process());
        }
    }

    class Orchestrator {
        public Validator Validator { get; private set; }
        public Executor Executor { get; private set; }
        public IDataComponent DataComponent { get; private set; }

        public Orchestrator(
            Validator validator, Executor executor, IDataComponent dataComponent) {
            Validator = validator;
            Executor = executor;
            DataComponent = dataComponent;
        }

        public int Process() {
            var data = DataComponent.GetData();

            Validator.Validate(data);
            return Executor.Execute(data);
        }
    }

    class Validator {
        public void Validate(string input) {
            if (string.IsNullOrEmpty(input)) {
                throw new ArgumentException("Must have a value", "input");
            }
        }
    }

    class Executor {
        public WordCounter WordCounter { get; set; }

        public Executor(WordCounter wordCounter) {
            WordCounter = wordCounter;
        }

        public int Execute(string input) {
            return WordCounter.CountWords(input);
        }
    }

    class WordCounter {
        public int CountWords(string sentence) {
            return sentence.Split(' ').Length;
        }
    }

    interface IDataComponent {
        string GetData();
    }

    class RealDataComponent : IDataComponent
    {
        public string GetData() {
            using (var connection = new SqlConnection("[YourDatabase]")) {
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    command.CommandText = "SELECT YourField from [YourTable]";
                    return (string)command.ExecuteScalar();
                }
            }
        }
    }

    class FakeDataComponent : IDataComponent
    {
        public string GetData() {
            return "all your base are belong to us";
        }
    }
}