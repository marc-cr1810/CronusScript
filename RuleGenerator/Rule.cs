using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator
{
    internal class Rule
    {
        public readonly string Name;
        public readonly string Format;
        public readonly ResultType ResultType;
        public SubRule[] Rules;

        public Rule(string name, string format, ResultType resultType)
        {
            Name = name;
            Format = format;
            ResultType = resultType;
            Rules = SubRule.GetSubRulesFromFormat(format);

            Program.AddRule(this);
        }
    }
}
