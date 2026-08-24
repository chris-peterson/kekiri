namespace Behavior.Examples.Orchestration;

public class WordCounter
{
    public int CountWords(string sentence) => sentence.Split(' ').Length;
}
