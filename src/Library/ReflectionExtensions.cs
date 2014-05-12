using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kekiri
{
    internal static class ReflectionExtensions
    {
        public static TAttribute AttributeOrDefault<TAttribute>(this MemberInfo member) where TAttribute : class
        {
            return member.GetCustomAttributes(typeof (TAttribute), false)
                .SingleOrDefault() as TAttribute;
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo member) where TAttribute : class
        {
            return member.GetCustomAttributes(typeof (TAttribute), false).Any();
        }

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

        /// <summary>
        /// Much more performant way of getting an assembly's short name compared to assembly.GetName().Name
        /// </summary>
        public static string ShortName(this Assembly assembly)
        {
            var commaIndex = assembly.FullName.IndexOf(",");
            if (commaIndex > 0)
                return assembly.FullName.Remove(commaIndex);

            return assembly.FullName;
        }
    }
}