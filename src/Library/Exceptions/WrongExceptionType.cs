using System;

namespace Kekiri.Exceptions
{
    public class WrongExceptionType : ScenarioTestException
    {
        public WrongExceptionType(
            object test,
            Type expectedExceptionType,
            Exception innerException)
            : base(
                test,
                string.Format("Expected '{0}', but was '{1}'", expectedExceptionType.Name, innerException.GetType().Name),
                innerException)
        {
            ExpectedExceptionType = expectedExceptionType;
            ActualExceptionType = innerException.GetType();
        }

        public Type ExpectedExceptionType { get; private set; }
        public Type ActualExceptionType { get; private set; }
    }
}