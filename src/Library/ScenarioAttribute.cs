using System;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScenarioAttribute : Attribute
    {
        public ScenarioAttribute() : this(null)
        {
        }

        public ScenarioAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; protected set; }
    }
}