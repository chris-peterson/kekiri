namespace Behavior.Examples.Orchestration;

public class Validator
{
    public void Validate(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new System.ArgumentException("Must have a value", nameof(input));
        }
    }
}
