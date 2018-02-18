using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kekiri.Impl
{
    static class ReflectionExtensions
    {
        public static KeyValuePair<string, object>[] BindParameters(this MethodBase method, KeyValuePair<string, object>[] supportedParameters)
        {
            supportedParameters = supportedParameters ?? new KeyValuePair<string, object>[0];
            var methodParameters = method.GetParameters();
            return supportedParameters
                .Where(supportedParam => methodParameters.Any(p => p.Name.Equals(supportedParam.Key, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }
    }
}