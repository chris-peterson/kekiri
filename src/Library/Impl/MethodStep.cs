using System.Reflection;
using Kekiri.Exceptions;

namespace Kekiri.Impl
{
    internal abstract class MethodStep : IStep
    {
        protected MethodBase Method { get; private set; }

        public bool ExceptionExpected { get; private set; }

        public string Name
        {
            get { return Method.Name; }
        }

        public StepType Type { get; private set; }

        public string SourceDescription { get; private set; }

        public bool SuppressOutput { get; private set; }


        protected MethodStep(MethodBase method)
        {
            if(method.IsPrivate)
                throw new StepMethodShouldBePublic(method.DeclaringType, method);
            if(method.GetParameters().Length > 0)
                throw new ScenarioStepMethodsShouldNotHaveParameters(method.DeclaringType, "The method '" + method.Name + "' is in a ScenarioTest and cannot have parameters");

            Method = method;
            Type = method.AttributeOrDefault<IStepAttribute>().StepType;
            SuppressOutput = method.SuppressOutputAttribute() != null;
            ExceptionExpected = method.AttributeOrDefault<ThrowsAttribute>() != null;
            SourceDescription = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
        }

        public abstract void Invoke(ScenarioTest test);
    }
}