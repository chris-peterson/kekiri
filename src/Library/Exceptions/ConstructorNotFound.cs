using System;

namespace Kekiri.Exceptions
{
    class ConstructorNotFound : Exception
    {
        public ConstructorNotFound(string message) : base(message)
        {
        }
    }
}