using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Behavior.Internal.Exceptions;
using Behavior.Internal.Reporting;

namespace Behavior.Internal;

interface IExceptionHandler
{
    void ExpectException();
    TException Catch<TException>() where TException : Exception;
    void AssertExceptionCompliance();
}
