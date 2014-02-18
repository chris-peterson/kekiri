using System;

namespace Kekiri
{

    [LibraryStepReference(StepType.Given)]
    public class Given { }

    [LibraryStepReference(StepType.When)]
    public class When { }

    public class LibraryStepReferenceAttribute : Attribute, IStepAttribute
    {
        public LibraryStepReferenceAttribute(StepType stepType)
        {
            StepType = stepType;
        }

        public StepType StepType { get; private set; }
    }
}