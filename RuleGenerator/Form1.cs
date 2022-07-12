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
            LoadRuleTree();

            foreach (string nodeType in Enum.GetNames(typeof(NodeType)))
            {
                ComboBoxType.Items.Add(nodeType);
            }
            ComboBoxType.SelectedIndex = 0;

            if (ComboBoxKind.Items.Count == 0)
                ComboBoxKind.Items.Add("No kind");
            ComboBoxKind.SelectedIndex = 0;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
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
            resultType.Kind = ResultType.GetKind(resultType.Type, ComboBoxKind.Text);

            Generate(name, format, resultType);

            LoadRuleTree();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            string output = "";

            // Output the result
            foreach (string r in Program.Rules.Keys)
            {
                Rule rule = Program.Rules[r];
                output += Template.GenerateNamedRule(rule) + "\n\n";
            }

            SetOutput(output);
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

        private void LoadRuleTree()
        {
            RulesTree.Nodes.Clear();

            foreach (string r in Program.Rules.Keys)
            {
                Rule rule = Program.Rules[r];

                TreeNode treeNode = new TreeNode(rule.Name);
                treeNode.Nodes.Add($"Result Type: {rule.ResultType.Type}");
                treeNode.Nodes.Add($"Result Kind: {rule.ResultType.GetKindString()}");
                TreeNode subRulesNode = new TreeNode("Subrules");
                foreach (SubRule subRule in rule.Rules)
                {
                    subRulesNode.Nodes.Add(subRule.Format);
                }
                treeNode.Nodes.Add(subRulesNode);

                RulesTree.Nodes.Add(treeNode);
            }
        }

        private void SetOutput(string output)
        {
            OutputBox.Text = "";

            Regex r = new Regex("\n");
            String[] lines = r.Split(output.Replace("\r", ""));

            foreach (string l in lines)
                ParseLine(l);
        }

        private void ParseLine(string line)
        {
            Regex r = new Regex("([ \\t{}():;?.])");
            String[] tokens = r.Split(line);

            bool comment = false;
            bool nextIsVarName = false;
            for (int t = 0; t < tokens.Length; t++)
            {
                string token = tokens[t];

                // Set the tokens default color and font
                OutputBox.SelectionColor = SystemColors.ControlLightLight;
                OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Regular);

                // Check whether the token is the start of a comment
                if (token == "//" || comment)
                {
                    // Apply alternative color and font to highlight keyword
                    OutputBox.SelectionColor = Color.FromArgb(87, 166, 74);
                    OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                    comment = true;
                }
                else if (nextIsVarName)
                {
                    // Apply alternative color and font to highlight keyword
                    OutputBox.SelectionColor = Color.FromArgb(150, 210, 215);
                    OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                    nextIsVarName = false;
                }
                else
                {
                    // Check whether the token is a keyword
                    string[] keywords = new string[0];

                    // Dark blue colored keywords
                    // Variable declarations
                    keywords = new string[] {
                        "int", "string", "bool", "float", "double", "object"
                    };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            nextIsVarName = true;
                            break;
                        }
                    }
                    keywords = new string[] {
                        "public", "private", "void", "using", "static", "class", "ref", "out", "null",
                        "int", "string", "bool", "float", "double", "object"
                    };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword
                            OutputBox.SelectionColor = Color.FromArgb(71, 156, 213);
                            OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                            goto color;
                        }
                    }

                    // Purple colored keywords
                    keywords = new string[] {
                        "if", "for", "else", "while", "do", "return", "break", "goto"
                    };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword
                            OutputBox.SelectionColor = Color.FromArgb(216, 160, 223);
                            OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                            goto color;
                        }
                    }

                    // Light green colored keywords
                    keywords = Enum.GetNames(typeof(NodeType));
                    keywords = keywords.Append("Token").ToArray();
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword
                            OutputBox.SelectionColor = Color.FromArgb(134, 198, 145);
                            OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                            goto color;
                        }
                    }

                    // Green colored keywords
                    keywords = new string[] {
                        "Parser", "Generator", "Console"
                    };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword
                            OutputBox.SelectionColor = Color.FromArgb(73, 194, 145);
                            OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                            goto color;
                        }
                    }

                    // Yellow colored keywords
                    keywords = new string[] {
                        "WriteLine", "PrintTest", "PrintFail", "PrintSuccess",
                        "ExpectToken",
                        "MakeModule",

                    };
                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (keywords[i] == token)
                        {
                            // Apply alternative color and font to highlight keyword
                            OutputBox.SelectionColor = Color.FromArgb(220, 220, 160);
                            OutputBox.SelectionFont = new Font("Courier New", 10, FontStyle.Bold);
                            goto color;
                        }
                    }
                }
            color:
                OutputBox.SelectedText = token;
            }
            OutputBox.SelectedText = "\n";
        }
    }
}