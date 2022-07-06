using CronusScript.Parser;
using RuleGenerator.Templates;
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
            string format = textRuleFormat.Text.ReplaceLineEndings("");
            format = Regex.Replace(format, @"\s+", " ");

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

            if (ComboBoxKind.Text != "No kind")
            {
                NodeTypeKind kind = new NodeTypeKind();
                switch (resultType.Type)
                {
                    case NodeType.ModType:
                        {
                            kind.ModKind = (ModKind)Enum.Parse(typeof(ModKind), ComboBoxKind.Text);
                        }
                        break;
                    case NodeType.StmtType:
                        {
                            kind.StmtKind = (StmtKind)Enum.Parse(typeof(StmtKind), ComboBoxKind.Text);
                        }
                        break;
                    case NodeType.ExprType:
                        {
                            kind.ExprKind = (ExprKind)Enum.Parse(typeof(ExprKind), ComboBoxKind.Text);
                        }
                        break;
                    case NodeType.ExceptHandlerType:
                        {
                            kind.ExceptHandlerKind = (ExceptHandlerKind)Enum.Parse(typeof(ExceptHandlerKind), ComboBoxKind.Text);
                        }
                        break;
                    case NodeType.TypeIgnoreType:
                        {
                            kind.TypeIgnoreKind = (TypeIgnoreKind)Enum.Parse(typeof(TypeIgnoreKind), ComboBoxKind.Text);
                        }
                        break;
                }
                resultType.Kind = kind;
            }

            Generate(name, format, resultType);

            // Output the result
            OutputBox.Text = Template.GenerateNamedRule(Rule);
        }

        private void Generate(string name, string format, ResultType resultType)
        {
            string result = "";

            if (name == null || format == null ||
                name == "" || format == "")
                return;

            Rule = new Rule(name, format, resultType);
        }

        private void ComboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type? enm = null;
            NodeType type = (NodeType)ComboBoxType.SelectedIndex + 1;

            switch (type)
            {
                case NodeType.ModType:
                    enm = typeof(CronusScript.Parser.ModKind);
                    break;
                case NodeType.StmtType:
                    enm = typeof(CronusScript.Parser.StmtKind);
                    break;
                case NodeType.ExprType:
                    enm = typeof(CronusScript.Parser.ExprKind);
                    break;
                case NodeType.ExceptHandlerType:
                    enm = typeof(CronusScript.Parser.ExceptHandlerKind);
                    break;
                case NodeType.TypeIgnoreType:
                    enm = typeof(CronusScript.Parser.TypeIgnoreKind);
                    break;
            }

            ComboBoxKind.Items.Clear();
            ComboBoxKind.Items.Add("No kind");

            if (enm == null)
            {
                ComboBoxKind.SelectedIndex = 0;
                return;
            }

            foreach (string nodeTypeKind in Enum.GetNames(enm))
            {
                ComboBoxKind.Items.Add(nodeTypeKind);
            }
            ComboBoxKind.SelectedIndex = 0;
        }
    }
}