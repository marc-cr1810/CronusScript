using System.Text.RegularExpressions;

namespace RuleGenerator
{
    public partial class Form1 : Form
    {
        private Rule Rule;

        public Form1()
        {
            InitializeComponent();

            foreach (string nodeType in Enum.GetNames(typeof(NodeType)))
            {
                ComboBoxType.Items.Add(nodeType);
            }
            ComboBoxType.SelectedIndex = 0;

            if (ComboBoxKind.Items.Count == 0)
                ComboBoxKind.Items.Add("No kind");
            ComboBoxKind.SelectedIndex = 0;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string name = textRuleName.Text;
            string format = textRuleFormat.Text;

            if (name == "")
            {
                int i = format.IndexOf(':');                
                if (Regex.IsMatch(format.Substring(0, i), @"^[a-zA-Z_]+$"))
                {
                    name = format.Substring(0, i);
                    format = format.Substring(i + 2);
                }
            }

            ResultType resultType = new ResultType();
            resultType.Type = (NodeType)Enum.Parse(typeof(NodeType), ComboBoxType.Text);

            Generate(name, format, resultType);

            // Output the result
            OutputBox.Text = Rule.ToString();
        }

        private void Generate(string name, string format, ResultType resultType)
        {
            string result = "";

            if (name == null || format == null ||
                name == "" || format == "")
                return;

            Rule = new Rule(name, format, resultType);

            string rules = GenerateSubRules(format);
        }

        private string GenerateSubRules(string format)
        {
            string result = "";

            List<string> ruleFormats = new List<string>();
            int parenLevel = 0;
            bool inQuote = false;

            string f = "";
            foreach (char c in format)
            {
                if (c == '"' || c == '\'')
                    inQuote = !inQuote;

                else if (c == '(' || c == '[')
                    parenLevel++;
                else if (c == ')' || c == ']')
                    parenLevel--;

                if (c == '|' && parenLevel == 0 && !inQuote)
                {
                    ruleFormats.Add(f.Trim());
                    f = "";
                    continue;
                }

                f += c;
            }

            if (f.Length > 0)
                ruleFormats.Add(f);

            foreach (string ruleFormat in ruleFormats)
            {
                SubRule subRule = new SubRule(ruleFormat);

            }

            return result;
        }

        private void ComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type? enm = null;
            NodeType type = (NodeType)ComboBoxType.SelectedIndex + 1;

            switch (type)
            {
                case NodeType.Mod:
                    enm = typeof(CronusScript.Parser.ModKind);
                    break;
                case NodeType.Stmt:
                    enm = typeof(CronusScript.Parser.StmtKind);
                    break;
                case NodeType.Expr:
                    enm = typeof(CronusScript.Parser.ExprKind);
                    break;
                case NodeType.ExceptHandler:
                    enm = typeof(CronusScript.Parser.ExceptHandlerKind);
                    break;
                case NodeType.TypeIgnore:
                    enm = typeof(CronusScript.Parser.TypeIgnoreKind);
                    break;
            }

            if (enm == null)
                return;

            ComboBoxKind.Items.Clear();
            ComboBoxKind.Items.Add("No kind");
            foreach (string nodeTypeKind in Enum.GetNames(enm))
            {
                ComboBoxKind.Items.Add(nodeTypeKind);
            }
            ComboBoxKind.SelectedIndex = 0;
        }
    }
}