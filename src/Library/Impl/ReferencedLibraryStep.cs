using System.Reflection;

namespace Kekiri.Impl
{
    internal class ReferencedLibraryStep : IStep
    {
        private readonly IStep _libraryStep;
        
        public ReferencedLibraryStep(FieldInfo referringField)
        {
            Type = referringField.FieldType.AttributeOrDefault<IStepAttribute>().StepType;
            Name = referringField.Name;
            _libraryStep = LibraryStepIndex.ForAssembly(referringField.DeclaringType.Assembly).FindStep(Type, Name);
            SuppressOutput = referringField.SuppressOutputAttribute() != null || _libraryStep.SuppressOutput;
            SourceDescription = string.Format("{0}.{1}", referringField.DeclaringType.FullName, referringField.Name); 
        }

        public void Invoke(ScenarioTest test)
        {
            _libraryStep.Invoke(test);
        }

        public bool ExceptionExpected
        {
            get { return _libraryStep.ExceptionExpected; }
        }

        public bool SuppressOutput { get; private set; }

        public string Name { get; private set; }

        public StepType Type { get; private set; }

        public string SourceDescription { get; private set; }
    }
}
