using Kekiri.TestSupport.Scenarios.Reporting;
using NUnit.Framework;

namespace Kekiri.TestSupport.Scenarios.Examples
{
    // Inspired by https://github.com/cucumber/cucumber/wiki/Scenario-Outlines
    [ScenarioOutline("eating")]
    [Example(12, 5, 7)]
    [Example(20, 5, 15)]
    public class Eating_cucumbers : ReportingScenarioMetaTest
    {
        private readonly int _start;
        private readonly int _eat;
        private readonly int _left;
        private int _cucumbers;

        public Eating_cucumbers(int start, int eat, int left)
        {
            _start = start;
            _eat = eat;
            _left = left;
        }

        [Given]
        public void Given_there_are_START_cucumbers()
        {
            _cucumbers = _start;
        }

        [When]
        public void When_I_eat_EAT_cucumbers()
        {
            _cucumbers -= _eat;
        }

        [Then]
        public void I_should_have_LEFT_cucumbers()
        {
            Assert.AreEqual(_left, _cucumbers);
        }
    }
}