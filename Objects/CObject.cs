using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronusScript.Objects
{    
    public class CObjectType
    {
        public string Name;
    }

    public class CObject
    {
        protected long RefCount;
        protected CObjectType ObjectType;

        public CObject()
        {
            RefCount = 1;
        }
    }
}
