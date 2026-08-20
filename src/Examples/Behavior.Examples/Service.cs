namespace Behavior.Examples.Resolution;

public class Service
{
    readonly IRepository _repository;

    public Service(IRepository repository)
    {
        _repository = repository;
    }

    public string DoWork() => _repository.GetData();
}
