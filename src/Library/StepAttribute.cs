namespace Kekiri
{
    /// <remarks>
    /// Typically would use a base class here, but ThenAttribute has to extend TestAttribute.
    /// </remarks>
    interface IStepAttribute
    {
        StepType StepType { get; }
    }
}