using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class UnitsLogic
        : BaseLogicTyped<Units>
    {
        public UnitsLogic() : base(T.Units)
        { }

        protected override Units internalGetModel(uint id)
        { return new Units(id); }
    }
}
