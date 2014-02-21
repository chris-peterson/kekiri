using System.Reflection;
using Kekiri.Exceptions;

namespace Kekiri.Impl
{
    internal class StepMethodInvoker : IStepInvoker
    {
        protected MethodBase Method { get; private set; }

        public bool ExceptionExpected { get; private set; }

        public StepName Name { get; private set; }

        public StepType Type { get; private set; }

        public string SourceDescription { get; private set; }

        public bool SuppressOutput { get; private set; }

        public StepMethodInvoker(MethodBase method) : this(method.AttributeOrDefault<IStepAttribute>().StepType, method) { }

        public StepMethodInvoker(StepType stepType, MethodBase method)
        {
            Method = method;
            Type = stepType;
            Name = new StepName(Type, method.Name);
            SuppressOutput = method.SuppressOutputAttribute() != null;
            ExceptionExpected = method.AttributeOrDefault<ThrowsAttribute>() != null;
            SourceDescription = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
        }

        public virtual void Invoke(object test)
        {
            Method.Invoke(Method.IsStatic ? null : test, null);
        }
    }
}