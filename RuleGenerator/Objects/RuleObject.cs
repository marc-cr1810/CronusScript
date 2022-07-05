using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal class RuleObject : RObject
    {
        public string Name;
        public ResultType? ExpectedResult;

        public RuleObject(string name) : this(name, null)
        {

        }

        public RuleObject(string name, ResultType? resultType) : base("rule")
        {
            Name = name;
            ExpectedResult = resultType;
        }
    }
}
