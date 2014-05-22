using Kekiri.Impl;

namespace Kekiri.Exceptions
{
    internal class FixtureShouldHaveWhens : ScenarioTestException
    {
        private static string GetMessage(ScenarioTestMetadata metaData)
        {
            string messageDetail = string.Empty;

            if (metaData.ScenarioTestType.BaseType == typeof(Test))
                messageDetail = "; whens should be parameterless public methods that use the [When] attribute";
            else if (metaData.ScenarioTestType.BaseType == typeof(FluentTest))
                messageDetail = "; whens should be specified with the When() method";

            return "No whens found" + messageDetail;
        }

        public FixtureShouldHaveWhens(ScenarioTestMetadata metaData, object test)
            : base(test, GetMessage(metaData))
        {
        }
    }
}