using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Kekiri.Exceptions;
using NUnit.Framework;

namespace Kekiri.Impl
{
    internal static class ScenarioMapper
    {
        private static ScenarioTest _test;

        public static ScenarioTestMetadata Map(ScenarioTest test)
        {
            _test = test;

            var type = _test.GetType();

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
                            value = backedField.GetValue(_test).ToString();
                        }
                        catch
                        {
                            value = "UNKNOWN!";
                        }
                        scenario.Parameters.Add(parameter.Name.ToUpper(), value);
                    }
                }
            }

            var methods = GetPotentialStepMethods(type);

            scenario.GivenMethods = ValidateAndBuildGivenMethods(methods);
            scenario.WhenMethods = ValidateAndBuildWhenMethod(methods);
            scenario.ThenMethods = ValidateAndBuildThenMethods(methods);

            return scenario;
        }

        private static IEnumerable<MethodBase> ValidateAndBuildGivenMethods(IEnumerable<MethodBase> methods)
        {
            var givenMethods = GetStepMethods<GivenAttribute>(methods);

            var nonPublicGiven = givenMethods.FirstOrDefault(g => g.IsPrivate);
            if (nonPublicGiven != null)
            {
                throw new GivensShouldBePublic(_test, nonPublicGiven);
            }

            return givenMethods; 
        }

        private static IEnumerable<MethodBase> ValidateAndBuildWhenMethod(IEnumerable<MethodBase> methods)
        {
            var whens = GetStepMethods<WhenAttribute>(methods);

            if (whens.Count == 0)
            {
                throw new FixtureShouldHaveWhens(_test);
            }

            var dictionary = new Dictionary<string, MethodBase>();

            foreach (var when in whens.Reverse())
            {
                if (!dictionary.ContainsKey(when.Name))
                {
                    dictionary.Add(when.Name, when);
                }
            }
            whens = dictionary.Values.ToList();

            if (whens.Count > 1)
            {
                throw new NotSupportedException(string.Format(
                    "Currently, only a single 'When' is supported, found: {0}", whens.Count)); 
            }

            var nonPublicWhen = whens.FirstOrDefault(g => g.IsPrivate);
            if (nonPublicWhen != null)
            {
                throw new WhensShouldBePublic(_test, nonPublicWhen);
            }

            return whens; 
        }

        private static IEnumerable<MethodBase> ValidateAndBuildThenMethods(IEnumerable<MethodBase> methods)
        {
            var methodBases = methods as MethodBase[] ?? methods.ToArray();
            
            var thenMethods = GetStepMethods<ThenAttribute>(methodBases);

            if ((thenMethods == null) || (thenMethods.Count == 0))
            {
                throw new FixtureShouldHaveThens(_test); 
            }

            var nonPublicThen = thenMethods.FirstOrDefault(g => g.IsPrivate);
            if (nonPublicThen != null)
            {
                throw new ThensShouldBePublic(_test, nonPublicThen);
            }

            var testMethods = GetStepMethods<TestAttribute>(methodBases);
            foreach (var testMethod in testMethods.Where(testMethod => !thenMethods.Contains(testMethod)))
            {
                throw new FixtureShouldNotUseTestAttribute(_test, testMethod);
            }

            return thenMethods; 
        }

        private static IList<MethodBase> GetStepMethods<TAttribute>(IEnumerable<MethodBase> methods)
        {
            var stepMethods = methods.Where(
                m => m.GetCustomAttributes(typeof(TAttribute), false).Any()).ToList();

            ValidateMethodSignatures(stepMethods);

            return stepMethods;
        }

        private static void ValidateMethodSignatures(IEnumerable<MethodBase> methods)
        {
            var methodsWithParameters = methods.Where(m => m.GetParameters().Length > 0).ToList();
            if (methodsWithParameters.Any())
            {
                var message = new StringBuilder();
                message.AppendLine("The following method(s) are steps in a ScenarioTest and may not contain any parameters.");
                foreach (MethodBase method in methodsWithParameters)
                {
                    message.AppendLine(method.Name);
                }

                throw new ScenarioStepMethodsShouldNotHaveParameters(_test, message.ToString());
            }
        }

        private static IList<MethodBase> GetPotentialStepMethods(Type type)
        {
            // Walk the type hierarchy from ScenarioTest downward so that base class givens are invoked before derived ones
            var derivedScenarioTestTypes = new Stack<Type>(new[] {type});
            while (type != null && type.BaseType != typeof(ScenarioTest))
            {
                type = type.BaseType;
                derivedScenarioTestTypes.Push(type);
            }

            var potentialStepMethods = new List<MethodBase>();
            foreach (var t in derivedScenarioTestTypes)
            {
                potentialStepMethods.AddRange(t.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Static | BindingFlags.Instance));
            }

            return potentialStepMethods;
        }
    }
}