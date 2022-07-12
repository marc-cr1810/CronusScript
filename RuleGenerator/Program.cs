using CronusScript.Parser;
using RuleGenerator.Objects;
using System.Text.RegularExpressions;

namespace RuleGenerator
{
    internal static class Program
    {
        public static Dictionary<string, Rule> Rules = new Dictionary<string, Rule>();
        public static Dictionary<string, Rule> Loops = new Dictionary<string, Rule>();
        
        private const string OutputFile = "generated.rules";
        private const string LoopFile = "loop.rules";

        public static bool AddRule(Rule rule, bool saveToFile = true)
        {
            if (Rules.ContainsKey(rule.Name))
                return false;
            Rules.Add(rule.Name, rule);

            if (saveToFile)
            {
                string type = rule.ResultType.Type.ToString();
                string kind = rule.ResultType.GetKindString();

                string resultType = (type.Length > 0) ? ("::" + type + ((kind.Length > 0) ? ":" + kind : "")) : "";

                string line = $"{(resultType.Length > 0 ? resultType + " " : "")}{rule.Name}: {rule.Format}";
                File.AppendAllText(OutputFile, line + "\n");
            }

            return true;
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!File.Exists(OutputFile))
                File.Create(OutputFile);
            string[] rules = File.ReadAllText(OutputFile).Split('\n');
            foreach (string ruleLine in rules)
            {
                string rule = ruleLine;

                ResultType resultType = new ResultType();
                string name = "";
                string format = "";

                // Get result type
                if (rule.StartsWith("::"))
                {
                    string rt = rule.Split(' ')[0].Substring(2);
                    if (rt.Split(':').Length > 0)
                    {
                        string[] s = rt.Split(':');
                        string type = s[0];
                        string k = s.Length > 1 ? s[1] : "";


                        ResultType result = new ResultType();
                        result.Type = (NodeType)Enum.Parse(typeof(NodeType), type);
                        result.Kind = ResultType.GetKind(result.Type, k);
                        resultType = result;
                    }
                    else
                    {
                        ResultType result = new ResultType();
                        result.Type = (NodeType)Enum.Parse(typeof(NodeType), rt);
                        result.Kind = new NodeTypeKind();
                    }
                    rule = rule.Substring(rt.Length + 2);
                }

                rule = rule.Trim();
                if (rule.Length > 0)
                {
                    int i = rule.IndexOf(':');
                    if (Regex.IsMatch(rule.Substring(0, i), @"^[a-zA-Z_]+$"))
                    {
                        name = rule.Substring(0, i);
                        format = rule.Substring(i + 2);
                    }

                    Rule resultRule = new Rule(name, format, resultType, false);
                }
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}