using RuleGenerator.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator
{
    internal class SubRule
    {
        public readonly string Format;
        public RObject[] Objects;

        public SubRule(string format)
        {
            Format = format;
            Objects = RObject.GetObjectsFromFormat(format);
        }

        public static SubRule[] GetSubRulesFromFormat(string format)
        {
            List<SubRule> subRules = new List<SubRule>();
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
                    if (f.Length > 0)
                    {
                        ruleFormats.Add(f.Trim());
                        f = "";
                    }
                    continue;
                }

                f += c;
            }

            if (f.Length > 0)
                ruleFormats.Add(f.Trim());

            foreach (string ruleFormat in ruleFormats)
            {
                SubRule subRule = new SubRule(ruleFormat);
                subRules.Add(subRule);
            }

            return subRules.ToArray();
        }

        public override string ToString()
        {
            return $"sub_rule: {Format}";
        }
    }
}
