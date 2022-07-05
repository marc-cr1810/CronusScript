using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal enum Conditions
    {
        Default,
        Loop,
        Loop_Opt,
        Not
    }

    internal class RObject
    {
        public readonly string Type;
        public Conditions Conditions;

        public RObject(string type)
        {
            Type = type;
            Conditions = Conditions.Default;
        }
    }
}
