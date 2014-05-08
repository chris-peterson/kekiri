using System;

namespace Kekiri.Exceptions
{
    public class StepNotFound : Exception
    {
        public StepNotFound(StepType stepType, string stepName) : base("Could not find a step in the step library for " + stepType + " " + stepName)
        {
            
        }
    }
}