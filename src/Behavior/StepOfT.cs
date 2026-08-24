using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Behavior.Internal;
using Behavior;

namespace Behavior;

public abstract class Step<TContext> : Step
{
    protected new TContext Context => (TContext)base.Context;
}
