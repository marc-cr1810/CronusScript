using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronusScript.Objects
{
    public class StringObjectType : CObjectType
    {
        public StringObjectType()
        {
            Name = "string";
        }
    }

    public class StringObject : CObject
    {
        public string StringValue;

        public StringObject(string value) : base()
        {
            StringValue = value;
            ObjectType = new StringObjectType();
        }
    }
}
