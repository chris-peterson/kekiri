namespace Kekiri.TestSupport.Scenarios.Fluent
{
    public class Parameterized_steps : ScenarioBase
    {
        public Parameterized_steps()
        {
            Given<Use_case_PARAMETER>("foo")
                .And(NULL_param, (string) null);
            When(Doing_task_TASKNUMBER, 1);
            Then(It_should_RESULT, "pass");
        }

        void NULL_param(string @null)
        {
        }

        void Doing_task_TASKNUMBER(int taskNumber)
        {
        }

        void It_should_RESULT(string result)
        {
        }
    }

    public class Use_case_PARAMETER : Step
    {
        public Use_case_PARAMETER(string parameter)
        {
        }

        public override void Execute()
        {
        }
    }
}