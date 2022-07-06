using CronusScript.Parser;
using RuleGenerator.Objects;
using System;
using System.Collections.Generic;
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

            string name = rule.GetRuleFunctionName();

            ReplaceValue(ref result, RULE_NAME, name);
            ReplaceValue(ref result, RULE_RESULT_TYPE, rule.ResultType.Type.ToString());
            ReplaceValue(ref result, RULE_SUB_RULES, GenerateSubRules(rule.Name, rule.ResultType, rule.Rules, 1));

            return result;
        }

        private static string GenerateSubRules(string ruleName, ResultType? expected, SubRule[] subRules, int tabs = 0)
        {
            string result = "";

            for (int i = 0; i < subRules.Length; i++)
            {
                SubRule subRule = subRules[i];
                string template = File.ReadAllText($"{FolderPath}SubRule.template");

                Dictionary<string, RObject> objects = GetObjectNames(subRule.Objects);

                ReplaceValue(ref template, FORMAT, subRule.Format);
                ReplaceValue(ref template, RULE_NAME, ruleName);
                ReplaceValue(ref template, OBJECTS, GenerateObjects(objects, tabs));
                ReplaceValue(ref template, OBJECT_CONDITIONALS, GenerateObjectConditionals(objects, tabs + 1));
                ReplaceValue(ref template, RESULT, GenerateResult(expected, subRule.Objects));

                result += template + (i < subRules.Length - 1 ? "\n" : "");
            }

            return InsertTabs(result, tabs);
        }

        private static string GenerateObjects(Dictionary<string, RObject> objects, int tabs = 0)
        {
            int literals = 0;
            int vars = 0;

            string result = "";

            for (int i = 0; i < objects.Keys.Count; i++)
            {
                string name = objects.Keys.ElementAt(i);
                RObject obj = objects[name];
                string str = "";

                switch (obj.Type)
                {
                    case "token":
                        {
                            TokenObject tokenObj = obj as TokenObject;
                            str = $"Token? {name}";
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

                            str += name;
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

                            str += name;
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

                            str += name;
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

                            str += name;
                            vars++;
                        }
                        break;
                }

                if (obj.Conditions.HasFlag(Conditions.Not))
                    continue;

                result += str + (i < objects.Keys.Count - 1 ? ";\n" : ";");
            }

            return InsertTabs(result, tabs);
        }

        private static string GenerateObjectConditionals(Dictionary<string, RObject> objects, int tabs = 0)
        {
            string result = "";
            int literals = 0;
            int vars = 0;

            for (int i = 0; i < objects.Keys.Count; i++)
            {
                string name = objects.Keys.ElementAt(i);
                RObject obj = objects[name];
                string str = "";
                string comment = "";
                bool handledOpt = false;

                switch (obj.Type)
                {
                    case "token":
                        {
                            TokenObject tokenObj = obj as TokenObject;
                            str = $"({name} = Generator.ExpectToken(ref p, TokenType.{tokenObj.TokenType})) != null";
                            comment = $"token='{tokenObj.Value}'";
                        }
                        break;
                    case "rule":
                        {
                            RuleObject ruleObj = obj as RuleObject;
                            str = $"({name} = {ruleObj.GetRuleFunctionName()}(ref p)) != null";
                        }
                        break;
                }

                if (comment == "")
                    comment = $"{obj.Format}";
                if (obj.Conditions.HasFlag(Conditions.Optional) && !handledOpt)
                    str = $"{str} || !p.ErrorIndicator";

                str = $"({str}) // {comment}";
                result += str + (i < objects.Keys.Count - 1 ? "\n&&\n" : "");
            }

            return InsertTabs(result, tabs);
        }

        private static string GenerateResult(ResultType? expected, RObject[] objects)
        {
            string result = "";

            if (expected == null)
                return "null";

            NodeType? type = expected.Value.Type;
            NodeTypeKind? kind = expected.Value.Kind;

            if (type == null)
                return "null";

            switch (type)
            {
                case NodeType.ModType:
                    {
                        if (kind != null)
                        {
                            switch (kind.Value.ModKind)
                            {
                                case ModKind.Module:
                                    {
                                        result = "Generator.MakeModule(ref p, )";
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }

            return result;
        }

        private static string GetParamObject(Dictionary<string, RObject> objects, NodeType type)
        {
            string result = "null";



            return result;
        }

        private static Dictionary<string, RObject> GetObjectNames(RObject[] objects)
        {
            Dictionary<string, RObject> result = new Dictionary<string, RObject>();

            int literals = 0;
            int vars = 0;

            foreach (RObject obj in objects)
            {
                string name = "";
                switch (obj.Type)
                {
                    case "token":
                        {
                            TokenObject tokenObj = obj as TokenObject;

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                name += $"opt_";
                            name += "literal" + (literals > 0 ? $"_{literals}" : "") + $"_{tokenObj.TokenType}";
                            literals++;
                        }
                        break;
                    case "rule":
                        {
                            RuleObject ruleObj = obj as RuleObject;

                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                name += $"opt_";
                            name += "var" + (vars > 0 ? $"_{vars}" : "") + $"_{ruleObj.Name}";
                            vars++;
                        }
                        break;
                    case "gather":
                        {
                            GatherObject gatherObject = obj as GatherObject;
                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                name += $"opt_";
                            name += "var" + (vars > 0 ? $"_{vars}" : "");
                            vars++;
                        }
                        break;
                    case "group":
                        {
                            GroupObject groupObject = obj as GroupObject;
                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                name += $"opt_";
                            name += "var" + (vars > 0 ? $"_{vars}" : "");
                            vars++;
                        }
                        break;
                    case "rule_group":
                        {
                            RuleGroupObject ruleGroupObject = obj as RuleGroupObject;
                            if (obj.Conditions.HasFlag(Conditions.Optional))
                                name += $"opt_";
                            name += "var" + (vars > 0 ? $"_{vars}" : "");
                            vars++;
                        }
                        break;
                }
                result.Add(name, obj);
            }
            return result;
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
