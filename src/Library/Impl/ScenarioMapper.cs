using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kekiri.Exceptions;
using NUnit.Framework;

namespace Kekiri.Impl
{
    internal static class ScenarioMapper
    {
        public static IEnumerable<KeyValuePair<string, object>> GetParameters(object test)
        {
            var type = test.GetType();
            var ctor = type.GetConstructors().SingleOrDefault();
            if (ctor != null)
            {
                foreach (var parameter in ctor.GetParameters())
                {
                    var backedField = type
                        .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .SingleOrDefault(
                            p => string.Compare(p.Name.TrimStart('_'), parameter.Name,
                                StringComparison.InvariantCultureIgnoreCase) == 0);
                    if (backedField != null)
                    {
                        object value;
                        try
                        {
                            value = backedField.GetValue(test);
                        }
                        catch
                        {
                            value = "UNKNOWN!";
                        }
                        yield return new KeyValuePair<string, object>(parameter.Name, value);
                    }
                }
            }
        }

        public static IList<IStepInvoker> GetStepInvokers(object test)
        {
            var parameters = GetParameters(test).ToArray();
            var type = test.GetType();
            // Walk the type hierarchy from ScenarioTest downward so that base class givens are invoked before derived ones
            var derivedScenarioTestTypes = new Stack<Type>(new[] {type});
            while (type != null && type.BaseType != typeof (ScenarioTest))
            {
                type = type.BaseType;
                derivedScenarioTestTypes.Push(type);
            }

            return derivedScenarioTestTypes
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance))
                .Where(IsStepMethod)
                .Select(m => GetStepFromMethod(m, parameters))
                .OrderBy(i => i, new StepNameOrderer())
                .Distinct(new StepNameComparer())
                .ToList();
        }

        private class StepNameComparer : IEqualityComparer<IStepInvoker>
        {
            public bool Equals(IStepInvoker x, IStepInvoker y)
            {
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(IStepInvoker obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        private class StepNameOrderer : IComparer<IStepInvoker>
        {
            public int Compare(IStepInvoker x, IStepInvoker y)
            {
                if (x.Type == StepType.When && y.Type == StepType.When)
                {
                    // favor derived class for Whens
                    return y.Order.CompareTo(x.Order);
                }

                return x.Order.CompareTo(y.Order);
            }
        }

        private static IStepInvoker GetStepFromMethod(MethodInfo method, KeyValuePair<string, object>[] parameters)
        {
            if (method.IsPrivate)
                throw new StepMethodShouldBePublic(method.DeclaringType, method);
            if (method.GetParameters().Length > 0)
                throw new ScenarioStepMethodsShouldNotHaveParameters(method.DeclaringType,
                    "The method '" + method.Name + "' is in a ScenarioTest and cannot have parameters");

            return new StepMethodInvoker(method, parameters);
        }

        private static bool IsStepMethod(MethodInfo method)
        {
            if (method.GetCustomAttributes(true).Any(a => a.GetType() == typeof (TestAttribute)))
                throw new FixtureShouldNotUseTestAttribute(method);

            return method.HasAttribute<IStepAttribute>();
        }
    }
}