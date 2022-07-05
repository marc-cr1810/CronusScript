using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal class GatherObject : RObject
    {
        public RObject Seperator;
        public RObject Element;
        public ResultType? ExpectedResult;

        public GatherObject(string format) : base("gather", format)
        {
            Seperator = new RObject("null");
            Element = new RObject("null");
        }

        public GatherObject(RObject sep, RObject elem) : base("gather")
        {
            Format = $"{sep.Format}.{elem.Format}";
            Seperator = sep;
            Element = elem;
        }
    }
}
