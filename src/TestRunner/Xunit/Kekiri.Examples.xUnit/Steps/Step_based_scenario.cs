using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Kekiri.Xunit;
using Xunit;

namespace Kekiri.Examples.Xunit.Steps
{
    public class Orchestrator_scenarios : ExampleScenariosTyped<Context>
    {
        [Scenario]
        public void Using_fakes_with_autofac()
        {
            Given<Fake_data_access>();
            When<Resolving_an_instance>();
            Then<It_uses_reals_for_everything>()
                .But<data_access_object_is_fake>()
                .And<it_computes_the_right_result>();
        }
    }

    public class Context
    {
        public Orchestrator Orchestrator { get; set; }
    }

    public class Fake_data_access : Step<Context>
    {
        public override async Task ExecuteAsync()
        {
            this.Container.Register(new FakeDataComponent());
            await Task.CompletedTask;
        }
    }

    public class Resolving_an_instance : Step<Context>
    {
        public override async Task ExecuteAsync()
        {
            await Task.FromResult(Context.Orchestrator = Container.Resolve<Orchestrator>());
        }
    }

    public class It_uses_reals_for_everything : Step<Context>
    {
        public override async Task ExecuteAsync()
        {
            Assert.IsType<Validator>(Context.Orchestrator.Validator);
            Assert.IsType<Executor>(Context.Orchestrator.Executor);
            Assert.IsType<WordCounter>(Context.Orchestrator.Executor.WordCounter);
            await Task.CompletedTask;
        }
    }
    public class data_access_object_is_fake : Step<Context>
    {
        public override async Task ExecuteAsync()
        {
            Assert.IsType<FakeDataComponent>(Context.Orchestrator.DataComponent);
            await Task.CompletedTask;
        }
    }

    public class it_computes_the_right_result : Step<Context>
    {
        public override async Task ExecuteAsync()
        {
            Assert.Equal(7, Context.Orchestrator.Process());
            await Task.CompletedTask;
        }
    }

    public class Orchestrator {
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

    public class Validator {
        public void Validate(string input) {
            if (string.IsNullOrEmpty(input)) {
                throw new ArgumentException("Must have a value", "input");
            }
        }
    }

    public class Executor {
        public WordCounter WordCounter { get; set; }

        public Executor(WordCounter wordCounter) {
            WordCounter = wordCounter;
        }

        public int Execute(string input) {
            return WordCounter.CountWords(input);
        }
    }

    public class WordCounter {
        public int CountWords(string sentence) {
            return sentence.Split(' ').Length;
        }
    }

    public interface IDataComponent {
        string GetData();
    }

    public class RealDataComponent : IDataComponent
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

    public class FakeDataComponent : IDataComponent
    {
        public string GetData() {
            return "all your base are belong to us";
        }
    }
}