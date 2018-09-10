using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class CoefficientValueLogic
        : BaseLogicTyped<CoefficientValue>
    {
        public CoefficientValueLogic() : base(T.CoefficientValue)
        { }

        protected override CoefficientValue internalGetModel(uint id)
        { return new CoefficientValue(id); }

        public override IEnumerable<CoefficientValue> Find()
        {
            return getQuerryResult($"all"
                , (table) =>
                 {
                     table.QUERRY().SHOW.DO();
                 });
        }
    }
}
