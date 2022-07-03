using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronusScript.Objects
{    
    public class CObjectType
    {
        public string? Name;
    }

    public class CObject
    {
        protected long RefCount;
        protected CObjectType? ObjectType;

        public CObject()
        {
            RefCount = 1;
        }

        public bool IsType(CObjectType type)
        {
            return ObjectType.Name == type.Name;
        }

        public void IncRef()
        {
            RefCount++;
        }

        public void DecRef()
        {
            RefCount--;
        }
    }

    /// Null object
    
    public class NullObject : CObject
    {
        public NullObject() : base()
        {
            ObjectType = new CObjectType() { Name = "null" };
        }
    }
}
