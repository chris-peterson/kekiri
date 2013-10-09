using FluentAssertions;
using Kekiri.Config;

namespace Kekiri.UnitTests.Config
{
    class When_using_custom_config_file : ConfigScenarioTest
    {
        private GherkinTestFrameworkSettingsFacade _settings;

        [When]
        public void When()
        {
            _settings = GherkinTestFrameworkSettingsFacade.GetInstance();
        }

        [Then]
        public void It_supports_custom_steps_given()
        {
            _settings.GetStep(StepType.Given).Should().Be("GIVEN");
        }

        [Then]
        public void And_when()
        {
            _settings.GetStep(StepType.When).Should().Be("WHEN");
        }

        [Then]
        public void And_then()
        {
            _settings.GetStep(StepType.Then).Should().Be("THEN");
        }

        [Then]
        public void It_supports_seperators_line()
        {
            _settings.GetSeperator(SeperatorType.Line).Should().Be("\n");
        }

        [Then]
        public void And_indent()
        {
            _settings.GetSeperator(SeperatorType.Indent).Should().Be("\t");
        }

        [Then]
        public void It_supports_custom_tokens_and()
        {
            _settings.GetToken(TokenType.And).Should().Be("AND");
        }

        [Then]
        public void And_but()
        {
            _settings.GetToken(TokenType.But).Should().Be("BUT");
        }

        [Then]
        public void And_feature()
        {
            _settings.GetToken(TokenType.Feature).Should().Be("FEATURE: ");
        }

        [Then]
        public void And_scenario()
        {
            _settings.GetToken(TokenType.Scenario).Should().Be("SCENARIO: ");
        }
    }
}