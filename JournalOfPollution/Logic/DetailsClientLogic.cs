using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class DetailsClientLogic
        : BaseLogicTyped<DetailsClient>
    {
        public DetailsClientLogic() : base(T.DetailsClient)
        { }

        protected override DetailsClient internalGetModel(uint id)
        { return new DetailsClient(id); }

        public IEnumerable<DetailsClient> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"int ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriodFrom(table.QUERRY().SHOWL(C.DetailsClient.YM).WHERE, ym, C.DetailsClient.YM);

                query = makeRangePeriod(query, ym,
                    C.DetailsClient.Client, new int[] { C.Client.YMFrom },
                    C.DetailsClient.Client, new int[] { C.Client.YMTo });

                if (canSelect.HasValue)
                { query.AND.ARC(C.DetailsClient.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();

                var result = getModels(table);
                var dictionary = new Dictionary<uint, DetailsClient>();

                foreach (var detailsClient in result)
                {
                    if (dictionary.ContainsKey(detailsClient.ClientID))
                    {
                        var findedDetailsClient = dictionary[detailsClient.ClientID];

                        if (findedDetailsClient.YM < detailsClient.YM)
                        { dictionary[detailsClient.ClientID] = detailsClient; }
                    }
                    else
                    { dictionary.Add(detailsClient.ClientID, detailsClient); }
                }

                return dictionary.Values.ToArray();
            });
        }

        public IEnumerable<DetailsClient> Find(int ym, uint clientID, bool? canSelect = null)
        {
            return getQuerryResult($"int ym={ym}|clientID={clientID}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriodFrom(table.QUERRY().SHOWL(C.DetailsClient.YM).WHERE, ym, C.DetailsClient.YM);

                query = makeRangePeriod(query, ym,
                    C.DetailsClient.Client, new int[] { C.Client.YMFrom },
                    C.DetailsClient.Client, new int[] { C.Client.YMTo });

                if (canSelect.HasValue)
                { query.AND.ARC(C.DetailsClient.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.AND.AC(C.DetailsClient.Client).EQUI.BV(clientID);

                query.DO();

                var result = getModels(table);
                var dictionary = new Dictionary<uint, DetailsClient>();

                foreach (var detailsClient in result)
                {
                    if (dictionary.ContainsKey(detailsClient.ClientID))
                    {
                        var findedDetailsClient = dictionary[detailsClient.ClientID];

                        if (findedDetailsClient.YM < detailsClient.YM)
                        { dictionary[detailsClient.ClientID] = detailsClient; }
                    }
                    else
                    { dictionary.Add(detailsClient.ClientID, detailsClient); }
                }

                return dictionary.Values.ToArray();
            });
        }

        public IEnumerable<DetailsClient> Find(uint clientID)
        {
            return getQuerryResult($"uint clientID={clientID}", (table) =>
            {
                var query = table.QUERRY()
                      .SHOWL(C.DetailsClient.YM)
                     .WHERE
                        .AC(C.DetailsClient.Client).EQUI.BV(clientID);

                query.DO();
            });
        }
    }
}
