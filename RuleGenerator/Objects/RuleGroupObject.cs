using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal class RuleGroupObject : RObject
    {
        public SubRule[] Rules;
        public ResultType? ExpectedResult;

        public RuleGroupObject(string format) : base("rule_group", $"({format})")
        {
            Rules = SubRule.GetSubRulesFromFormat(format);
        }
    }
}
