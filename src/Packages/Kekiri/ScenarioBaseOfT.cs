using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kekiri.Internal;
using Kekiri.Internal.Exceptions;
using Kekiri.Internal.Reporting;
using Kekiri;

namespace Kekiri;

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
