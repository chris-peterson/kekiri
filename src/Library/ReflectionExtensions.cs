using System;
using System.Linq;
using System.Reflection;
using Kekiri.Reporting;

namespace Kekiri
{
    internal static class ReflectionExtensions
    {
        public static SuppressOutputAttribute SuppressOutputAttribute(this MemberInfo method)
        {
            var attributeOnMethod = method.AttributeOrDefault<SuppressOutputAttribute>();

            if (attributeOnMethod == null)
            {
                return method.DeclaringType.AttributeOrDefault<SuppressOutputAttribute>();
            }

            return attributeOnMethod;
        }

        public static TAttribute AttributeOrDefault<TAttribute>(this MemberInfo method) where TAttribute : class
        {
            return method.GetCustomAttributes(typeof (TAttribute), false)
                .SingleOrDefault() as TAttribute;
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo member) where TAttribute : class
        {
            return member.AttributeOrDefault<TAttribute>() != null;
        }

        public static TAttribute AttributeOrDefault<TAttribute>(this Type type) where TAttribute : class
        {
            return type.GetCustomAttributes(typeof(TAttribute), true)
                .SingleOrDefault() as TAttribute;
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