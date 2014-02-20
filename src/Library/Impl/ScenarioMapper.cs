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
        public static ScenarioTestMetadata Map(ScenarioTest test)
        {
            var type = test.GetType();

            var scenario = new ScenarioTestMetadata(type);

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
                        string value;
                        try
                        {
                            value = backedField.GetValue(test).ToString();
                        }
                        catch
                        {
                            value = "UNKNOWN!";
                        }
                        scenario.Parameters.Add(parameter.Name.ToUpper(), value);
                    }
                }
            }

            var steps = GetStepInvokers(type);

            scenario.GivenMethods = steps.Where(s => s.Type == StepType.Given).ToList();
            scenario.WhenMethods = ValidateAndBuildWhenSteps(test, steps);
            scenario.ThenMethods = ValidateAndBuildThenSteps(test, steps);

          
            return scenario;
        }
        private static IEnumerable<IStepInvoker> ValidateAndBuildWhenSteps(ScenarioTest test, IEnumerable<IStepInvoker> steps)
        {
            var whens = steps.Where(s => s.Type == StepType.When)
                .Reverse()
                .Distinct(new StepNameComparer())
                .ToList();

            if (whens.Count == 0)
            {
                throw new FixtureShouldHaveWhens(test);
            }
            if (whens.Count > 1)
            {
                throw new NotSupportedException(string.Format(
                    "Currently, only a single 'When' is supported, found: {0}", whens.Count)); 
            }

            return whens;
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

        private static IEnumerable<IStepInvoker> ValidateAndBuildThenSteps(ScenarioTest test, IEnumerable<IStepInvoker> steps)
        {
            var thenMethods = steps.Where(s => s.Type == StepType.Then).ToList();

            if (thenMethods.Count == 0)
            {
                throw new FixtureShouldHaveThens(test); 
            }

            return thenMethods;
        }

        private static IStepInvoker GetStepFromMember(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return new StepClassInvoker(((FieldInfo)member).FieldType);
            }
            if (member is MethodInfo)
            {
                return new StepMethodInvoker((MethodInfo)member);
            }
            throw new Exception("Unexpected member type");
        }

        private static IList<IStepInvoker> GetStepInvokers(Type type)
        {
            // Walk the type hierarchy from ScenarioTest downward so that base class givens are invoked before derived ones
            var derivedScenarioTestTypes = new Stack<Type>(new[] {type});
            while (type != null && type.BaseType != typeof(ScenarioTest))
            {
                type = type.BaseType;
                derivedScenarioTestTypes.Push(type);
            }

            return derivedScenarioTestTypes
                .SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic |
                                          BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance))
                .Where(m => IsStepMethod(m) || IsStepClass(m))
                .Select(GetStepFromMember)
                .ToList();
        }

        private static bool IsStepClass(MemberInfo member)
        {
            return member is FieldInfo &&
                   typeof (Step).IsAssignableFrom(((FieldInfo) member).FieldType);
        }

        private static bool IsStepMethod(MemberInfo member)
        {
            if(member.GetCustomAttributes(true).Any(a => a.GetType() == typeof(TestAttribute)))
                throw new FixtureShouldNotUseTestAttribute(member);
            
            return member is MethodInfo &&
                   member.HasAttribute<IStepAttribute>();
        }
    }
}