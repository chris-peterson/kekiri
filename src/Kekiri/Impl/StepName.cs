using System;
using System.Collections.Generic;
using System.Linq;
using Kekiri.Impl.Config;

namespace Kekiri.Impl
{
    class StepName
    {
        readonly Settings _settings = Settings.GetInstance();

        public StepName(StepType stepType, string name, IEnumerable<KeyValuePair<string, object>> substitutionParameters = null)
        {
            StepType = stepType;
            SeparatorToken = GetSeparatorToken(stepType, name);
            Outline = GetOutline(stepType, name);
            PrettyName = SubstituteParameters(Outline, substitutionParameters);
        }

        public StepType StepType { get; }

        public string PrettyName { get; }

        public string Outline { get; }

        public TokenType SeparatorToken { get; private set; }

        public override string ToString()
        {
            return $"{StepType} {PrettyName}";
        }

        protected bool Equals(StepName other)
        {
            return StepType == other.StepType && string.Equals(PrettyName, other.PrettyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((StepName) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) StepType*397) ^ PrettyName.GetHashCode();
            }
        }

        TokenType GetSeparatorToken(StepType stepType, string name)
        {
            if(stepType == StepType.Then &&
               name.StartsWith(_settings.GetToken(TokenType.But), StringComparison.OrdinalIgnoreCase))
            {
                return TokenType.But;
            }

            return TokenType.And;
        }

        string GetOutline(StepType stepType, string name)
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

        string GetStepNameWithoutTypeNorSeperators(StepType stepType, string stepName)
        {
            stepName = stepName.RemovePrefix(_settings.GetStep(stepType));
            stepName = stepName.RemovePrefix(_settings.GetToken(TokenType.And));
            stepName = stepName.RemovePrefix(_settings.GetToken(TokenType.But));

            return stepName;
        }

        static string SubstituteParameters(string stepName, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
                return stepName;
            
            foreach (var parameter in parameters)
            {
                foreach (var word in stepName.Split(' ')
                    .Where(word => word == parameter.Key.ToUpperInvariant()))
                {
                    stepName = stepName.Replace(word,
                        parameter.Value?.ToString() ?? "{null}");
                }
            }

            return stepName;
        }
    }
}