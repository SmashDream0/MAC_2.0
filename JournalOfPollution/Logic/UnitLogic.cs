using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class UnitLogic
        : BaseLogicTyped<Unit>
    {
        public UnitLogic() : base(T.Unit)
        { }

        protected override Unit internalGetModel(uint id)
        { return new Unit(id); }
    }
}
