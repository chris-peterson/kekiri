using System.Configuration;
using System.Linq;

namespace Kekiri.Config
{
    public class GherkinTestFrameworkSettings : ConfigurationSection
    {
        public static GherkinTestFrameworkSettings GetInstanceWithDefaultValues()
        {
            var settings = new GherkinTestFrameworkSettings();

            foreach (var propertyInfo in typeof (GherkinTestFrameworkSettings).GetProperties())
            {
                var attribute = (ConfigurationPropertyAttribute)
                                propertyInfo
                                    .GetCustomAttributes(typeof (ConfigurationPropertyAttribute), false)
                                    .SingleOrDefault();
                if (attribute != null && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(settings, attribute.DefaultValue, null);
                }
            }

            return settings;
        }

        [ConfigurationProperty("given", DefaultValue = "Given", IsRequired = false)]
        public string Given
        {
            get { return (string) this["given"]; }
            set { this["given"] = value; }
        }

        [ConfigurationProperty("when", DefaultValue = "When", IsRequired = false)]
        public string When
        {
            get { return (string)this["when"]; }
            set { this["when"] = value; }
        }

        [ConfigurationProperty("then", DefaultValue = "Then", IsRequired = false)]
        public string Then
        {
            get { return (string)this["then"]; }
            set { this["then"] = value; }
        }

        [ConfigurationProperty("line", DefaultValue = "\r\n", IsRequired = false)]
        public string Line
        {
            get { return (string)this["line"]; }
            set { this["line"] = value; }
        }

        [ConfigurationProperty("indent", DefaultValue = "  ", IsRequired = false)]
        public string Indent
        {
            get { return (string)this["indent"]; }
            set { this["indent"] = value; }
        }

        [ConfigurationProperty("and", DefaultValue = "And", IsRequired = false)]
        public string And
        {
            get { return (string)this["and"]; }
            set { this["and"] = value; }
        }

        [ConfigurationProperty("but", DefaultValue = "But", IsRequired = false)]
        public string But
        {
            get { return (string)this["but"]; }
            set { this["but"] = value; }
        }

        [ConfigurationProperty("feature", DefaultValue = "Feature: ", IsRequired = false)]
        public string Feature
        {
            get { return (string)this["feature"]; }
            set { this["feature"] = value; }
        }

        [ConfigurationProperty("scenario", DefaultValue = "Scenario: ", IsRequired = false)]
        public string Scenario
        {
            get { return (string)this["scenario"]; }
            set { this["scenario"] = value; }
        }
    }
}