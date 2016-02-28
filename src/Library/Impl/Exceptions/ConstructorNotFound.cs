using System;

namespace Kekiri.Impl.Exceptions
{
    class ConstructorNotFound : Exception
    {
        public ConstructorNotFound(string message) : base(message)
        {
        }
    }
}