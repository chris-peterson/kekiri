using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kekiri.Impl
{
    static class ReflectionExtensions
    {
        public static TAttribute AttributeOrDefault<TAttribute>(this Type type) where TAttribute : class
        {
            return type.GetCustomAttributes(typeof(TAttribute), true)
                .SingleOrDefault() as TAttribute;
        }

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