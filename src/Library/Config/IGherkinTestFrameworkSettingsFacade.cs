using System;
using System.Configuration;

namespace Kekiri.Config
{
    public interface IGherkinTestFrameworkSettingsFacade
    {
        string GetStep(StepType stepType);
        string GetSeperator(SeperatorType seperatorType);
        string GetToken(TokenType tokenType);
    }

    public class GherkinTestFrameworkSettingsFacade : IGherkinTestFrameworkSettingsFacade
    {
        private readonly GherkinTestFrameworkSettings _settings;

        public static IGherkinTestFrameworkSettingsFacade GetInstance()
        {
            var settings =
                ConfigurationManager.GetSection("gherkinTestFrameworkSettings") as GherkinTestFrameworkSettings ??
                GherkinTestFrameworkSettings.GetInstanceWithDefaultValues();

            return new GherkinTestFrameworkSettingsFacade(settings);
        }

        private GherkinTestFrameworkSettingsFacade(GherkinTestFrameworkSettings settings)
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
                default:
                    throw new NotSupportedException(string.Format("Unknown token type: {0}", tokenType));
            }
        }
    }
}