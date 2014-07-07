using System;
using System.Collections.Generic;
using Kekiri.Config;

namespace Kekiri.Impl
{
    internal class StepName
    {
        private readonly Settings _settings = Settings.GetInstance();

        public StepName(StepType stepType, string name, IEnumerable<KeyValuePair<string, object>> substitutionParameters = null)
        {
            StepType = stepType;
            SeparatorToken = GetSeparatorToken(stepType, name);
            Outline = GetOutline(stepType, name);
            PrettyName = SubstituteParameters(Outline, substitutionParameters);
        }

        public StepType StepType { get; private set; }

        public string PrettyName { get; private set; }

        public string Outline { get; private set; }

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

        private string GetOutline(StepType stepType, string name)
        {
            var stepNameSansStepType = GetStepNameWithoutTypeNorSeperators(stepType, name);

            if (string.IsNullOrWhiteSpace(stepNameSansStepType))
            {
                // no conversion to _ or PascalCase necessary -- bail
                return string.Empty;
            }

            var outline = stepNameSansStepType.AsSentence();

            return outline.StartsWithMultipleUppercaseLetters()
                ? outline
                : outline.WithFirstLetterLowercase();
        }

        private string GetStepNameWithoutTypeNorSeperators(StepType stepType, string stepName)
        {
            stepName = stepName.RemovePrefix(_settings.GetStep(stepType));
            stepName = stepName.RemovePrefix(_settings.GetToken(TokenType.And));
            stepName = stepName.RemovePrefix(_settings.GetToken(TokenType.But));

            return stepName;
        }

        private string SubstituteParameters(string stepName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
                return stepName;
            
            foreach (var parameter in parameters)
            {
                foreach (var word in stepName.Split(' '))
                {
                    if (word == parameter.Key.ToUpperInvariant())
                    {
                        stepName = stepName.Replace(word, parameter.Value.ToString());
                    }
                }
            }

            return stepName;
        }
    }
}