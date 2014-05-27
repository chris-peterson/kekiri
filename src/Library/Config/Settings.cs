using System;
using System.Configuration;
using System.Linq;

namespace Kekiri.Config
{
    internal class Settings
    {
        private readonly ConfigFileBasedSettings _settings;

        public static Settings GetInstance()
        {
            var settings =
                ConfigurationManager.GetSection("kekiriSettings") as ConfigFileBasedSettings ??
                ConfigFileBasedSettings.GetInstanceWithDefaultValues();

            return new Settings(settings);
        }

        private Settings(ConfigFileBasedSettings settings)
        {
            _settings = settings;
        }

        public string GetStep(StepType stepType)
        {
            switch (stepType)
            {
                case StepType.Given:
                    return _settings.Given;
                case StepType.When:
                    return _settings.When;
                case StepType.Then:
                    return _settings.Then;
                default:
                    throw new NotSupportedException(string.Format("Unknown step type: {0}", stepType));
            }
        }

        public string GetSeperator(SeperatorType seperatorType)
        {
            switch (seperatorType)
            {
                case SeperatorType.Line:
                    return _settings.Line;
                case SeperatorType.Indent:
                    return _settings.Indent;
                default:
                    throw new NotSupportedException(string.Format("Unknown seperator type: {0}", seperatorType));
            }
        }

        public string GetToken(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.And:
                    return _settings.And;
                case TokenType.But:
                    return _settings.But;
                case TokenType.Feature:
                    return _settings.Feature;
                case TokenType.Scenario:
                    return _settings.Scenario;
                case TokenType.ScenarioOutline:
                    return _settings.ScenarioOutline;
                default:
                    throw new NotSupportedException(string.Format("Unknown token type: {0}", tokenType));
            }
        }
    }

    // removed support for clients specifying this, but since it was developed as a config section, keep it around JIC.
    internal class ConfigFileBasedSettings : ConfigurationSection
    {
        public static ConfigFileBasedSettings GetInstanceWithDefaultValues()
        {
            var settings = new ConfigFileBasedSettings();

            foreach (var propertyInfo in typeof(ConfigFileBasedSettings).GetProperties())
            {
                var attribute = (ConfigurationPropertyAttribute)
                                propertyInfo
                                    .GetCustomAttributes(typeof(ConfigurationPropertyAttribute), false)
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
            get { return (string)this["given"]; }
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

        [ConfigurationProperty("scenarioOutline", DefaultValue = "Scenario Outline: ", IsRequired = false)]
        public string ScenarioOutline
        {
            get { return (string)this["scenarioOutline"]; }
            set { this["scenarioOutline"] = value; }
        }
    }
}