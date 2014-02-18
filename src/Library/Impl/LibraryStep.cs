using System.Reflection;

namespace Kekiri.Impl
{
    internal class LibraryStep : MethodStep
    {
        
        public LibraryStep(MethodBase method) : base(method)
        {
        }

        public override void Invoke(ScenarioTest test)
        {
            var stepLibrary = StepLibrary.InstanceFor(test, Method.DeclaringType);
            Method.Invoke(stepLibrary, null);
        }
    }
}