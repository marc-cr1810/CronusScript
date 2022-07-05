using RuleGenerator.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Templates
{
    internal class Template
    {
        private const string FolderPath = "../../../Templates/";

        private const string FORMAT = "FORMAT";
        private const string OBJECTS = "OBJECTS";
        private const string OBJECT_CONDITIONALS = "OBJECT_CONDITIONALS";
        private const string RESULT = "RESULT";

        private const string RULE_NAME = "RULE_NAME";
        private const string RULE_RESULT_TYPE = "RULE_RESULT_TYPE";
        private const string RULE_SUB_RULES = "RULE_SUB_RULES";

        private const string LOOP_RULE_NUM = "LOOP_RULE_NUM";
        private const string LOOP_RULE_RESULT_TYPE = "LOOP_RULE_RESULT_TYPE";
        private const string LOOP_RULE_ELEM_RESULT_TYPE = "LOOP_RULE_ELEM_RESULT_TYPE";

        public static string GenerateNamedRule(Rule rule)
        {
            string result = File.ReadAllText($"{FolderPath}NamedRule.template");

            string name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rule.Name.ToLower()).Replace("_", "");

            ReplaceValue(ref result, RULE_NAME, name);
            ReplaceValue(ref result, RULE_RESULT_TYPE, rule.ResultType.Type.ToString());
            ReplaceValue(ref result, RULE_SUB_RULES, GenerateSubRules(rule.Name, rule.Rules, 1));

            return result;
        }

        private static string GenerateSubRules(string ruleName, SubRule[] subRules, int tabs = 0)
        {
            string result = "";

            for (int i = 0; i < subRules.Length; i++)
            {
                SubRule subRule = subRules[i];
                string template = File.ReadAllText($"{FolderPath}SubRule.template");
                ReplaceValue(ref template, FORMAT, subRule.Format);
                ReplaceValue(ref template, RULE_NAME, ruleName);
                ReplaceValue(ref template, OBJECTS, GenerateObjects(subRule.Objects, tabs));

                result += template + (i < subRules.Length - 1 ? "\n" : "");
            }

            return InsertTabs(result, tabs);
        }

        private static string GenerateObjects(RObject[] objects, int tabs = 0)
        {
            int literals = 0;
            int vars = 0;

            string result = "";

            for (int i = 0; i < objects.Length; i++)
            {
                string str = "";
                RObject obj = objects[i];

                if (obj.Conditions.HasFlag(Conditions.Not))
                    continue;

                switch (obj.Type)
                {
                    case "token":
                        {
                            TokenObject tokenObj = obj as TokenObject;
                            str = "Token? ";

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                str += $"opt_";
                            str += "literal" + (literals > 0 ? $"_{literals}" : "") + $"_{tokenObj.TokenType};";
                            literals++;
                        }
                        break;
                    case "rule":
                        {
                            RuleObject ruleObj = obj as RuleObject;
                            /// TODO: Get rule type or create a new rule
                            str = "object? ";
                            if (ruleObj.ExpectedResult != null)
                                str = $"{ruleObj.ExpectedResult.Value.Type}? ";

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                str += $"opt_";
                            str += "var" + (vars > 0 ? $"_{vars}" : "") + $"_{ruleObj.Name};";
                            vars++;
                        }
                        break;
                    case "gather":
                        {
                            GatherObject gatherObject = obj as GatherObject;
                            /// TODO: Get gather rule type or create a new gather rule
                            str = "object? ";
                            if (gatherObject.ExpectedResult != null)
                                str = $"{gatherObject.ExpectedResult.Value.Type}? ";

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                str += $"opt_";
                            str += "var" + (vars > 0 ? $"_{vars}" : "") + ";";
                            vars++;
                        }
                        break;
                    case "group":
                        {
                            GroupObject groupObject = obj as GroupObject;
                            /// TODO: Get rule group type or create a new rule group
                            str = "object? ";
                            if (groupObject.ExpectedResult != null)
                                str = $"{groupObject.ExpectedResult.Value.Type}? ";

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                str += $"opt_";
                            str += "var" + (vars > 0 ? $"_{vars}" : "") + ";";
                            vars++;
                        }
                        break;
                    case "rule_group":
                        {
                            RuleGroupObject ruleGroupObject = obj as RuleGroupObject;
                            /// TODO: Get rule group type or create a new rule group
                            str = "object? ";
                            if (ruleGroupObject.ExpectedResult != null)
                                str = $"{ruleGroupObject.ExpectedResult.Value.Type}? ";

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                str += $"opt_";
                            str += "var" + (vars > 0 ? $"_{vars}" : "") + ";";
                            vars++;
                        }
                        break;
                }

                result += str + (i < objects.Length - 1 ? "\n" : "");
            }

            return InsertTabs(result, tabs);
        }

        private static void ReplaceValue(ref string template, string name, string value)
        {
            template = template.Replace(name, value);
        }

        private static string InsertTabs(string str, int tabs, bool ignoreFirst = true)
        {
            string result = "";
            string[] lines = str.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (ignoreFirst)
                {
                    result += line + '\n';
                    ignoreFirst = false;
                    continue;
                }
                string s = string.Concat(Enumerable.Repeat("    ", tabs)) + line + (i < lines.Length - 1 ? '\n' : "");
                result += s;
            }
            return result;
        }
    }
}
