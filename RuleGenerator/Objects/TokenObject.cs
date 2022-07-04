using CronusScript.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal class TokenObject : RuleObject
    {
        public readonly string Value;
        public readonly TokenType TokenType;

        public TokenObject(string value, TokenType tokenType) : base("token")
        {
            Value = value;
            TokenType = tokenType;
        }
    }
}
