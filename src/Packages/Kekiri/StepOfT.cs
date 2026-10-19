using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kekiri.Internal;
using Kekiri;

namespace Kekiri;

public abstract class Step<TContext> : Step
{
    protected new TContext Context => (TContext)base.Context;
}
