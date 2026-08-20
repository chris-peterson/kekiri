using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Behavior.Internal;
using Behavior.Internal.Exceptions;
using Behavior.Internal.Reporting;
using Behavior;

namespace Behavior;

public abstract class ScenarioBase<TContext> : ScenarioBase
{
    protected override object CreateContextObject()
    {
        var contextType = typeof (TContext);
        var ctor = contextType.GetTypeInfo().GetConstructor(new Type[] {});
        if (ctor != null)
            return ctor.Invoke(null);

        var method = typeof (Container).GetTypeInfo().GetMethod("Resolve").MakeGenericMethod(contextType);
        return method.Invoke(Container, null);
    }

    protected internal new TContext Context => base.Context;
}
