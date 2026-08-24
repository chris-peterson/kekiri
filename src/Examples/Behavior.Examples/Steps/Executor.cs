namespace Behavior.Examples.Orchestration;

public class Executor
{
    public Executor(WordCounter wordCounter)
    {
        WordCounter = wordCounter;
    }

    public WordCounter WordCounter { get; }

    public int Execute(string input) => WordCounter.CountWords(input);
}
