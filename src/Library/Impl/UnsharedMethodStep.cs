using System.Reflection;

namespace Kekiri.Impl
{
    internal class UnsharedMethodStep : MethodStep
    {
        public UnsharedMethodStep(MethodBase method) : base(method)
        {
        }

        public override void Invoke(ScenarioTest test)
        {
            Method.Invoke(Method.IsStatic ? null : test, null);
        }
    }
}