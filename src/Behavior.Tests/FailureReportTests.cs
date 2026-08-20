using System;
using System.Threading.Tasks;
using Behavior.Internal;
using Behavior.Internal.Exceptions;
using Behavior.Tests.Samples.Broken;

namespace Behavior.Tests;

public class FailureReportTests
{
    [Test]
    public async Task A_step_wrapper_is_unwrapped_to_its_cause()
    {
        var cause = new InvalidOperationException("what actually went wrong");
        var wrapper = new GivenFailed(new Failing_then(), "a step", cause);

        await Assert.That(ReferenceEquals(FailureReport.Cause(wrapper), cause)).IsTrue();
        await Assert.That(FailureReport.OneLine(wrapper)).IsEqualTo("what actually went wrong");
    }

    [Test]
    public async Task A_diagnosis_is_the_failure_and_stays_wrapped()
    {
        var arrived = new InvalidOperationException("not what was asked for");
        var diagnosis = new WrongExceptionType(new Failing_then(), typeof(DivideByZeroException), arrived);

        await Assert.That(ReferenceEquals(FailureReport.Cause(diagnosis), diagnosis)).IsTrue();
        await Assert.That(FailureReport.OneLine(diagnosis))
            .IsEqualTo("Expected DivideByZeroException, but was InvalidOperationException");
    }

    [Test]
    public async Task Only_the_first_line_of_a_reason_reaches_the_step_marker()
    {
        var cause = new InvalidOperationException("first line" + Environment.NewLine + "second line");
        var wrapper = new GivenFailed(new Failing_then(), "a step", cause);

        await Assert.That(FailureReport.OneLine(wrapper)).IsEqualTo("first line");
    }
}
