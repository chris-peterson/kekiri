using System;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScenarioAttribute : Attribute
    {
        public ScenarioAttribute()
        {
        }

        public ScenarioAttribute(object featureEnum)
        {
            Feature = featureEnum;
        }

        public ScenarioAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; protected set; }

        public object Feature { get; private set; }
    }
}