using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal class RuleObject : RObject
    {
        public readonly string Name;
        public ResultType? ExpectedResult;

        public RuleObject(string name) : this(name, null)
        {
            Name = name;
        }

        public RuleObject(string name, ResultType? resultType) : base("rule", name)
        {
            Name = name;
            ExpectedResult = resultType;
        }
    }
}
