using System;

namespace Kekiri.Impl.Exceptions
{
    class WrongExceptionType : ScenarioException
    {
        public WrongExceptionType(
            ScenarioBase scenario,
            Type expectedExceptionType,
            Exception innerException)
            : base(
                scenario,
                $"Expected '{expectedExceptionType.Name}', but was '{innerException.GetType().Name}'",
                innerException)
        {
            ExpectedExceptionType = expectedExceptionType;
            ActualExceptionType = innerException.GetType();
        }

        public Type ExpectedExceptionType { get; private set; }
        public Type ActualExceptionType { get; private set; }
    }
}