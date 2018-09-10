using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class ObjecteLogic
        : BaseLogicTyped<Objecte>
    {
        public ObjecteLogic() : base(T.Objecte)
        { }

        protected override Objecte internalGetModel(uint id)
        { return new Objecte(id); }

        public IEnumerable<Objecte> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"long ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Objecte.Client, new int[] { C.Client.YMFrom },
                    C.Objecte.Client, new int[] { C.Client.YMTo });

                if (canSelect.HasValue)
                { query.AND.ARC(C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            },
            (result)=>
            {
                var objectsDeclaration = this.GetDictionary(result);

                {
                    var objectsFromResolution = LogicHelper.ObjectFromResolutionLogic.Find(ym, canSelect);

                    foreach (var objectFromDefinition in objectsFromResolution)
                    {
                        if (objectsDeclaration.ContainsKey(objectFromDefinition.ObjectID))
                        {
                            var objecte = objectsDeclaration[objectFromDefinition.ObjectID];

                            objecte.Add(objectFromDefinition);
                        }
                    }
                }

                {
                    var objectDetails = LogicHelper.DetailsObjectLogic.Find(ym, canSelect);

                    foreach (var detailsObject in objectDetails)
                    {
                        if (objectsDeclaration.ContainsKey(detailsObject.ObjectID))
                        {
                            var objecte = objectsDeclaration[detailsObject.ObjectID];

                            objecte.Add(detailsObject);
                        }
                    }
                }

                {
                    var wells = LogicHelper.WellLogic.Find(ym, canSelect);

                    foreach (var well in wells)
                    {
                        if (objectsDeclaration.ContainsKey(well.ObjectID))
                        {
                            var objecte = objectsDeclaration[well.ObjectID];

                            objecte.Add(well);
                        }
                    }
                }

                {
                    var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym);

                    foreach (var negotiationAssistant in negotiationAssistants)
                    {
                        if (objectsDeclaration.ContainsKey(negotiationAssistant.ObjectID))
                        {
                            var objecte = objectsDeclaration[negotiationAssistant.ObjectID];

                            objecte.Add(negotiationAssistant);
                        }
                    }
                }

                {
                    var clients = LogicHelper.ClientsLogic.Find(ym, canSelect);
                    var clientDictionary = LogicHelper.ClientsLogic.GetDictionary(clients);

                    foreach (var objecte in result)
                    {
                        if (clientDictionary.ContainsKey(objecte.ClientID))
                        {
                            var client = clientDictionary[objecte.ClientID];

                            objecte.Add(client);
                        }
                    }
                }
            });
        }

        public IEnumerable<Objecte> Find(int ym, uint clientID)
        {
            return getQuerryResult($"clientID={clientID}|ym={ym}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Objecte.Client, new int[] { C.Client.YMFrom },
                    C.Objecte.Client, new int[] { C.Client.YMTo });

                query.AND.AC(C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            },
            (result) =>
            {
                var objectsDeclaration = this.GetDictionary(result);

                {
                    var objectsFromResolution = LogicHelper.ObjectFromResolutionLogic.Find(ym, clientID);

                    foreach (var objectFromDefinition in objectsFromResolution)
                    {
                        if (objectsDeclaration.ContainsKey(objectFromDefinition.ObjectID))
                        {
                            var objecte = objectsDeclaration[objectFromDefinition.ObjectID];

                            objecte.Add(objectFromDefinition);
                        }
                    }
                }

                {
                    var objectDetails = LogicHelper.DetailsObjectLogic.Find(ym, clientID);

                    foreach (var detailsObject in objectDetails)
                    {
                        if (objectsDeclaration.ContainsKey(detailsObject.ObjectID))
                        {
                            var objecte = objectsDeclaration[detailsObject.ObjectID];

                            objecte.Add(detailsObject);
                        }
                    }
                }

                {
                    var wells = LogicHelper.WellLogic.Find(ym, clientID);

                    foreach (var well in wells)
                    {
                        if (objectsDeclaration.ContainsKey(well.ObjectID))
                        {
                            var objecte = objectsDeclaration[well.ObjectID];

                            objecte.Add(well);
                        }
                    }
                }

                {
                    var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym);

                    foreach (var negotiationAssistant in negotiationAssistants)
                    {
                        if (objectsDeclaration.ContainsKey(negotiationAssistant.ObjectID))
                        {
                            var objecte = objectsDeclaration[negotiationAssistant.ObjectID];

                            objecte.Add(negotiationAssistant);
                        }
                    }
                }

                {
                    var client = LogicHelper.ClientsLogic.FirstOrDefault(ym, clientID);

                    foreach (var objecte in result)
                    { objecte.Add(client); }
                }
            });
        }

        public Objecte FirstOrDefault(int ym, uint objectID, bool? canSelect)
        {
            return getQuerryResult($"long ym={ym}|objectID={objectID}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Objecte.Client, new int[] { C.Client.YMFrom },
                    C.Objecte.Client, new int[] { C.Client.YMTo });

                if (canSelect.HasValue)
                { query.AND.ARC(C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.AND.ID(objectID);

                query.DO();
            },
            (result) =>
            {
                var objectsDeclaration = this.GetDictionary(result);

                {
                    var objectsFromResolution = LogicHelper.ObjectFromResolutionLogic.Find(ym, objectID, canSelect);

                    foreach (var objectFromDefinition in objectsFromResolution)
                    {
                        if (objectsDeclaration.ContainsKey(objectFromDefinition.ObjectID))
                        {
                            var objecte = objectsDeclaration[objectFromDefinition.ObjectID];

                            objecte.Add(objectFromDefinition);
                        }
                    }
                }

                {
                    var objectDetails = LogicHelper.DetailsObjectLogic.Find(ym, objectID, canSelect);

                    foreach (var detailsObject in objectDetails)
                    {
                        if (objectsDeclaration.ContainsKey(detailsObject.ObjectID))
                        {
                            var objecte = objectsDeclaration[detailsObject.ObjectID];

                            objecte.Add(detailsObject);
                        }
                    }
                }

                {
                    var wells = LogicHelper.WellLogic.Find(ym, objectID, canSelect);

                    foreach (var well in wells)
                    {
                        if (objectsDeclaration.ContainsKey(well.ObjectID))
                        {
                            var objecte = objectsDeclaration[well.ObjectID];

                            objecte.Add(well);
                        }
                    }
                }

                {
                    var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym, objectID, 1);

                    foreach (var negotiationAssistant in negotiationAssistants)
                    {
                        if (objectsDeclaration.ContainsKey(negotiationAssistant.ObjectID))
                        {
                            var objecte = objectsDeclaration[negotiationAssistant.ObjectID];

                            objecte.Add(negotiationAssistant);
                        }
                    }
                }

                {
                    var client = LogicHelper.ClientsLogic.FirstOrDefault(ym, objectID, canSelect);

                    foreach (var objecte in result)
                    {
                        objecte.Add(client);
                    }
                }
            }).FirstOrDefault();
        }

        public Objecte FirstOrDefault(int ym, uint clientID)
        {
            return getQuerryResult($"long ym={ym}|clientID={clientID}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Objecte.Client, new int[] { C.Client.YMFrom },
                    C.Objecte.Client, new int[] { C.Client.YMTo });

                query.AND.AC(C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            },
            (result) =>
            {
                var objectsDeclaration = this.GetDictionary(result);

                {
                    var objectsFromResolution = LogicHelper.ObjectFromResolutionLogic.Find(ym, clientID);

                    foreach (var objectFromDefinition in objectsFromResolution)
                    {
                        if (objectsDeclaration.ContainsKey(objectFromDefinition.ObjectID))
                        {
                            var objecte = objectsDeclaration[objectFromDefinition.ObjectID];

                            objecte.Add(objectFromDefinition);
                        }
                    }
                }

                {
                    var objectDetails = LogicHelper.DetailsObjectLogic.Find(ym, clientID);

                    foreach (var detailsObject in objectDetails)
                    {
                        if (objectsDeclaration.ContainsKey(detailsObject.ObjectID))
                        {
                            var objecte = objectsDeclaration[detailsObject.ObjectID];

                            objecte.Add(detailsObject);
                        }
                    }
                }

                {
                    var wells = LogicHelper.WellLogic.Find(ym, clientID);

                    foreach (var well in wells)
                    {
                        if (objectsDeclaration.ContainsKey(well.ObjectID))
                        {
                            var objecte = objectsDeclaration[well.ObjectID];

                            objecte.Add(well);
                        }
                    }
                }

                {
                    var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym, clientID);

                    foreach (var negotiationAssistant in negotiationAssistants)
                    {
                        if (objectsDeclaration.ContainsKey(negotiationAssistant.ObjectID))
                        {
                            var objecte = objectsDeclaration[negotiationAssistant.ObjectID];

                            objecte.Add(negotiationAssistant);
                        }
                    }
                }

                {
                    var client = LogicHelper.ClientsLogic.FirstOrDefault(ym, clientID);

                    foreach (var objecte in result)
                    {
                        objecte.Add(client);
                    }
                }
            }).FirstOrDefault();
        }
    }
}
