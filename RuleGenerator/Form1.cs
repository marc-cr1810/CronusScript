using System.Text.RegularExpressions;

namespace RuleGenerator
{
    public partial class Form1 : Form
    {
        private Rule Rule;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string name = textRuleName.Text;
            string format = textRuleFormat.Text;
            string resultType = ComboBoxResultType.Text;

            if (name == "")
            {
                int i = format.IndexOf(':');                
                if (Regex.IsMatch(format.Substring(0, i), @"^[a-zA-Z_]+$"))
                {
                    name = format.Substring(0, i);
                    format = format.Substring(i + 2);
                }
            }
            Generate(name, format, resultType);

            // Output the result
            OutputBox.Text = Rule.ToString();
        }

        private void Generate(string name, string format, string resultType)
        {
            string result = "";

            if (name == null || format == null || resultType == null ||
                name == "" || format == "" || resultType == "")
                return;

            Rule = new Rule(name, format, resultType);

            string rules = GenerateSubRules(format);
        }

        private string GenerateSubRules(string format)
        {
            string result = "";

            string[] ruleFormats = format.Split(" | ");

            foreach (string ruleFormat in ruleFormats)
            {
                SubRule subRule = new SubRule(ruleFormat);

            }

            return result;
        }
    }
}