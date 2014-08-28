namespace Kekiri.Exceptions
{
    internal class FixtureShouldHaveThens : ScenarioTestException
    {
        public FixtureShouldHaveThens(object test)
            : base(test, GetMessage(test))
        {
        }

        private static string GetMessage(object test)
        {
            string messageDetail = string.Empty;

            if (test is Test)
                messageDetail = "; a then method should be a parameterless public method that uses the [Then] attribute";
            else if (test is FluentTest)
                messageDetail = "; a then should be specified by calling Then in the constructor";

            return "No thens found" + messageDetail;
        }
    }
}