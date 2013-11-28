namespace Kekiri
{
    /// <summary>
    /// Intended to be used with 1 or more <see cref="ExampleAttribute"/>s.
    /// </summary>
    public class ScenarioOutlineAttribute : ScenarioAttribute
    {
        public ScenarioOutlineAttribute()
            : this(null)
        {
        }

        public ScenarioOutlineAttribute(string description)
        {
            Description = description;
        }
    }
}