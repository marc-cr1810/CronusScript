using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator
{
    internal enum RuleType
    {
        Named,
        Loop
    }

    internal class Rule
    {
        public readonly string Name;
        public readonly string Format;
        public readonly ResultType ResultType;
        public SubRule[] Rules;

        public Rule(string name, string format, ResultType resultType, bool saveToFile = true, RuleType type = RuleType.Named)
        {
            Name = name;
            Format = format;
            ResultType = resultType;

            bool added = type == RuleType.Named ? Program.AddRule(this, saveToFile) : false;

            if (added)
                 Rules = SubRule.GetSubRulesFromFormat(format);
            else
                Rules = new SubRule[0];
        }

        public string GetRuleFunctionName()
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name.ToLower()).Replace("_", "");
        }

        public override string ToString()
        {
            return Name + ": " + Format;
        }
    }
}
