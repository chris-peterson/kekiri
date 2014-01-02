using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace Kekiri.TestGen
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            var builder = new StringBuilder();

            var stepType = StepType.Given;

            foreach (var line in _textBox.Text.Split(Environment.NewLine.ToCharArray())
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                ProcessScenarioLine(line, ref stepType, builder);
            }

            // remove the extra whitespace line after methods
            builder.Remove(builder.Length - 2, 2);
            
            builder.AppendLine("   }");

            Clipboard.SetText(builder.ToString());

        }

        private void ProcessScenarioLine(string line, ref StepType stepType, StringBuilder builder)
        {
            if (stepType == StepType.When)
            {
                stepType = StepType.Then;
            }

            if (line.StartsWith("When"))
            {
                stepType = StepType.When;
            }

            const string tagToken = "@";
            if (line.StartsWith(tagToken))
            {
                builder.AppendLine(string.Format("   [Tag(\"{0}\")]", line.TrimStart('@')));
                return;
            }
            const string scenarioToken = "Scenario:";
            if (line.StartsWith(scenarioToken))
            {
                builder.AppendLine("   [Scenario]");
                builder.AppendLine(string.Format("   class {0} : ScenarioTest", Sanitize(line.Substring(scenarioToken.Length))));
                builder.AppendLine("   {");
                return;
            }

            if (line.StartsWith("-> done: "))
            {
                return;
            }

            var methodName = ProcessStepLine(line, stepType);

            builder.AppendLine(string.Format("      [{0}]", stepType));
            builder.AppendLine(string.Format("      public void {0}()", methodName));
            builder.AppendLine("      {");
            builder.AppendLine("      }");
            builder.AppendLine();
        }

        private string ProcessStepLine(string stepLine, StepType stepType)
        {
            string str = stepLine;

            var stepToken = stepType.ToString();
            
            const string andToken = "And";
            if (str.StartsWith(andToken, StringComparison.InvariantCultureIgnoreCase))
            {
                str = str.Substring(andToken.Length);
                stepToken = andToken;
            }

            const string butToken = "But";
            if (str.StartsWith(butToken, StringComparison.InvariantCultureIgnoreCase))
            {
                str = str.Substring(butToken.Length);
                stepToken = butToken;
            }

            if (str.StartsWith(stepToken, StringComparison.InvariantCultureIgnoreCase))
            {
                str = str.Substring(stepToken.Length);
            }

            str = Sanitize(str);

            return string.Format("{0}_{1}", stepToken, str);
        }

        private string Sanitize(string str)
        {
            return str
                .Trim()
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("\"", "")
                .Replace("’", "")
                .Replace("'", "")
                .Replace("“", "")
                .Replace("”", "");
        }
    }
}
