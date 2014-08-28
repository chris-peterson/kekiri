using System;

namespace Kekiri.Exceptions
{
    internal class StepNotFound : Exception
    {
        public StepNotFound(string message) : base(message)
        {
        }
    }
}