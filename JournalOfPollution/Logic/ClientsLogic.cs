using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class ClientsLogic
        : BaseLogicTyped<Client>
    {
        public ClientsLogic() : base(T.Client)
        { }

        protected override Client internalGetModel(uint id)
        { return new Client(id); }
    
        public IEnumerable<Client> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"int ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Client.YMFrom,
                    C.Client.YMTo);

                if (canSelect.HasValue)
                { query.AND.AC(C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            }
            , (result) =>
            {
                var clientsDictionary = this.GetDictionary(result);

                {
                    var objects = LogicHelper.ObjecteLogic.Find(ym, canSelect);

                    foreach (var objecte in objects)
                    {
                        if (clientsDictionary.ContainsKey(objecte.ClientID))
                        {
                            var client = clientsDictionary[objecte.ClientID];

                            client.Add(objecte);
                        }
                    }
                }

                {
                    var detailsClients = LogicHelper.DetailsClientLogic.Find(ym, canSelect);

                    foreach (var detailsClient in detailsClients)
                    {
                        if (clientsDictionary.ContainsKey(detailsClient.ClientID))
                        {
                            var client = clientsDictionary[detailsClient.ClientID];

                            client.Add(detailsClient);
                        }
                    }
                }
            });
        }

        public Client FirstOrDefault(int ym, uint objectID, bool? canSelect)
        {
            return getQuerryResult($"int ym={ym}|objectID={objectID}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var clientID = T.Objecte.Rows.Get_UnShow<uint>(objectID, C.Objecte.Client);

                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Client.YMFrom,
                    C.Client.YMTo);

                if (canSelect.HasValue)
                { query.AND.AC(C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.AND.ID(clientID);

                query.DO();
            }
            , (result) =>
            {
                var clientsDictionary = this.GetDictionary(result);

                {
                    var objecte = LogicHelper.ObjecteLogic.FirstOrDefault(ym, objectID, canSelect);

                    if (clientsDictionary.ContainsKey(objecte.ClientID))
                    {
                        var client = clientsDictionary[objecte.ClientID];

                        client.Add(objecte);
                    }
                }

                {
                    var clientID = T.Objecte.Rows.Get_UnShow<uint>(objectID, C.Objecte.Client);

                    var detailsClients = LogicHelper.DetailsClientLogic.Find(ym, clientID, canSelect);

                    foreach (var detailsClient in detailsClients)
                    {
                        if (clientsDictionary.ContainsKey(detailsClient.ClientID))
                        {
                            var client = clientsDictionary[detailsClient.ClientID];

                            client.Add(detailsClient);
                        }
                    }
                }
            }).FirstOrDefault();
        }
        
        public Client FirstOrDefault(int ym, uint id)
        {
            bool cacheUsed;
            var fod = getQuerryResult($"id={id}|ym={ym}", (table) =>
            {
                table.QUERRY().SHOW.WHERE.ID(id).DO();
            }
            , out cacheUsed
            , (result) =>
            {
                var clientsDictionary = this.GetDictionary(result);

                {
                    var objects = LogicHelper.ObjecteLogic.Find(ym, id);
                }

                {
                    var detailsClients = LogicHelper.DetailsClientLogic.Find(ym, id, null);

                    foreach (var detailsClient in detailsClients)
                    {
                        if (clientsDictionary.ContainsKey(detailsClient.ClientID))
                        {
                            var client = clientsDictionary[detailsClient.ClientID];

                            client.Add(detailsClient);
                        }
                    }
                }
            }).FirstOrDefault();

            if (!cacheUsed)
            { this.Cache.ClearQuerry(); }

            return fod;
        }
    }
}
