using System;
using Kekiri.Config;

namespace Kekiri.Impl
{
    public class StepName
    {
        private readonly GherkinTestFrameworkSettingsFacade _settings = GherkinTestFrameworkSettingsFacade.GetInstance();
        
        public StepName(StepType stepType, string name)
        {
            StepType = stepType;
            SeparatorToken = GetSeparatorToken(stepType, name);
            PrettyName = GetPrettyName(stepType, name);
        }

        public StepType StepType { get; private set; }

        public string PrettyName { get; private set; }

        public TokenType SeparatorToken { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", StepType, PrettyName);
        }

        protected bool Equals(StepName other)
        {
            return StepType == other.StepType && string.Equals(PrettyName, other.PrettyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StepName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) StepType*397) ^ PrettyName.GetHashCode();
            }
        }

        private TokenType GetSeparatorToken(StepType stepType, string name)
        {
            if(stepType == StepType.Then &&
               name.StartsWith(_settings.GetToken(TokenType.But), StringComparison.OrdinalIgnoreCase))
            {
                return TokenType.But;
            }

            return TokenType.And;
        }

        private string GetPrettyName(StepType stepType, string name)
        {
            var stepNameSansStepType = GetStepNameWithoutTypeNorSeperators(stepType, name);

            if (string.IsNullOrWhiteSpace(stepNameSansStepType))
            {
                // no conversion to _ or PascalCase necessary -- bail
                return string.Empty;
            }

            return stepNameSansStepType.WithSpaces().WithFirstLetterLowercase();
        }

        private string GetStepNameWithoutTypeNorSeperators(StepType stepType, string stepName)
        {
            stepName = stepName.RemovePrefix(_settings.GetStep(stepType));
            stepName = stepName.RemovePrefix(_settings.GetToken(TokenType.And));
            stepName = stepName.RemovePrefix(_settings.GetToken(TokenType.But));

            return stepName;
        }
    }
}