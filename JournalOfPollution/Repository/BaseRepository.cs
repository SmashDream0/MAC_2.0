using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2
{
    public abstract class BaseCache
    {
        public void Clear()
        {
            ClearModel();
            ClearQuerry();
        }

        public abstract void ClearModel();
        public abstract void ClearQuerry();
    }
}
