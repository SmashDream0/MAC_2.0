using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class WorkerLogic
        : BaseLogicTyped<Worker>
    {
        public WorkerLogic() : base(T.Worker)
        { }

        protected override Worker internalGetModel(uint id)
        { return new Worker(id); }

        public IEnumerable<Worker> Find(int ym)
        {
            return getQuerryResult($"ym={ym}", (table) =>
            {
                makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Worker.YMFrom,
                    C.Worker.YMTo)
                .DO();
            },
            (result)=>
            {

            });
        }
    }
}
