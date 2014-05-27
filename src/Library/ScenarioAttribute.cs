using System;

namespace Kekiri
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScenarioAttribute : Attribute
    {
        [Obsolete("Use overload that takes an enum literal instead", true)]
        // ReSharper disable once UnusedParameter.Local
        public ScenarioAttribute(string description)
        {
        }

        public ScenarioAttribute(object featureEnum) : this(featureEnum, null)
        {
        }

        public ScenarioAttribute(object featureEnum, string description)
        {
            Feature = featureEnum;
            Description = description;
        }

        public string Description { get; protected set; }

        public object Feature { get; private set; }
    }
}