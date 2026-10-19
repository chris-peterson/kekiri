using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kekiri.Internal.Exceptions;
using Kekiri.Internal.Reporting;

namespace Kekiri.Internal;

interface IExceptionHandler
{
    void ExpectException();
    TException Catch<TException>() where TException : Exception;
    void AssertExceptionCompliance();
}
