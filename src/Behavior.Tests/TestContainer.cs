using System;

namespace Behavior.Tests;

/// <summary>
/// Enough of a container for a typed context to resolve. The scenarios here inject nothing, so
/// activating the context type is the whole job.
/// </summary>
class TestContainer : Container
{
    protected override T OnResolve<T>() => Activator.CreateInstance<T>();
}
