using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class AccreditLogic
        : BaseLogicTyped<Accredit>
    {
        public AccreditLogic():base(T.Accredit)
        { }
        protected override Accredit internalGetModel(uint id)
        { return new Accredit(id); }

        public IEnumerable<Accredit> Find(long ymd)
        {
            return getQuerryResult($"long ymd={ymd}", (table) =>
            {
                table.QUERRY()
                     .SHOW
                     .WHERE
                     .OB()
                         .AC(C.Accredit.YMDFrom).Less.BV(ymd + 1)
                     .OR
                         .C(C.Accredit.YMDFrom, 0)
                     .CB()
                     .AND
                     .OB()
                         .AC(C.Accredit.YMDTo).More.BV(ymd - 1)
                     .OR
                         .C(C.Accredit.YMDTo, 0)
                     .CB()
                     .DO();
            });
        }
    }
}