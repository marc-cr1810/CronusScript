using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal class GroupObject : RObject
    {
        public RObject[] Objects;
        public ResultType? ExpectedResult;

        public GroupObject(string format) : base("group", $"[{format}]")
        {
            Objects = GetObjectsFromFormat(format);
        }
    }
}
