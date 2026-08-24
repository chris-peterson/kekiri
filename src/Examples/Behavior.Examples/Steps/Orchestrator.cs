namespace Behavior.Examples.Orchestration;

public class Orchestrator
{
    public Orchestrator(Validator validator, Executor executor, IDataComponent dataComponent)
    {
        Validator = validator;
        Executor = executor;
        DataComponent = dataComponent;
    }

    public Validator Validator { get; }

    public Executor Executor { get; }

    public IDataComponent DataComponent { get; }

    public int Process()
    {
        var data = DataComponent.GetData();

        Validator.Validate(data);

        return Executor.Execute(data);
    }
}
