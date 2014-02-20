using System.Collections.Generic;
using Kekiri.Config;

namespace Kekiri.Impl
{
    internal static class StepNameFormatter
    {
        public static string SubstituteParameters(string stepName, IDictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                foreach (var word in stepName.Split(' '))
                {
                    if (word == parameter.Key)
                    {
                        stepName = stepName.Replace(word, parameter.Value);
                    }
                }
            }

            return stepName;
        }
    }
}