using System;

namespace Kekiri.Impl.Config
{
    internal class Settings
    {
        private readonly ConfigFileBasedSettings _settings;

        public static Settings GetInstance()
        {
            var settings = new ConfigFileBasedSettings();

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
    internal class ConfigFileBasedSettings
    {
        public string Given => "Given";

        public string When => "When";

        public string Then => "Then";

        public string Line => "\r\n";

        public string Indent => "  ";

        public string And => "And";

        public string But => "But";

        public string Feature => "Feature: ";

        public string Scenario => "Scenario: ";

        public string ScenarioOutline => "Scenario Outline: ";
    }
}