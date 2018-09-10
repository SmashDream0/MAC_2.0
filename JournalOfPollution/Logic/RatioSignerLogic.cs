using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class RatioSignerLogic
        : BaseLogicTyped<RatioSigner>
    {
        public RatioSignerLogic() : base(T.RatioSigner)
        { }

        protected override RatioSigner internalGetModel(uint id)
        { return new RatioSigner(id); }

        public IEnumerable<RatioSigner> Find(int ym, data.ETypeTemplate type, string list = null)
        {
            return getQuerryResult($"int ym={ym}|type={type}|List={(list ?? "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.RatioSigner.Worker, new int[] { C.Worker.YMFrom },
                    C.RatioSigner.Worker, new int[] { C.Worker.YMTo });

                query.AND.C(C.RatioSigner.TypeTemplate, (uint)type);

                if (list != null)
                { query.AND.C(C.RatioSigner.List, list); }

                query.DO();

                table.Sort(C.RatioSigner.Position);

                var result = getModels(table);

                result = result.OrderBy(x => x.Position).ToArray();

                return result;
            },
            (result) =>
            {
                var workers = LogicHelper.WorkerLogic.Find(ym);
                var dictionary = LogicHelper.WorkerLogic.GetDictionary(workers);

                foreach (var ratioSigner in result)
                {
                    if (dictionary.ContainsKey(ratioSigner.WorkerID))
                    {
                        var worker = dictionary[ratioSigner.WorkerID];

                        ratioSigner.Add(worker);
                    }
                }
            });
        }
    }
}