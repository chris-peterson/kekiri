using System;

namespace Kekiri.Exceptions
{
    class WrongExceptionType : ScenarioException
    {
        public WrongExceptionType(
            object test,
            Type expectedExceptionType,
            Exception innerException)
            : base(
                test,
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