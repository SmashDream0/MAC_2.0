using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class DeclarationValueLogic
        : BaseLogicTyped<DeclarationValue>
    {
        public DeclarationValueLogic() : base(T.DeclarationValue)
        { }

        protected override DeclarationValue internalGetModel(uint id)
        { return new DeclarationValue(id); }

        public IEnumerable<DeclarationValue> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"int ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.YMFrom },
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.YMTo });

                query = makeRangePeriod(query, ym,
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.YMFrom },
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.YMFrom },
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.YMTo });

                if (canSelect.HasValue)
                { query.AND.ARC(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            }
            , (result)=>
            {
                {
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var dictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);

                    foreach (var declarationValue in result)
                    {
                        if (dictionary.ContainsKey(declarationValue.PollutionID))
                        {
                            var pollution = dictionary[declarationValue.PollutionID];

                            declarationValue.Add(pollution);
                        }
                    }
                }
            });
        }

        public IEnumerable<DeclarationValue> Find(int ym, uint clientID)
        {
            return getQuerryResult($"int ym={ym}|clientID={clientID}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.YMFrom },
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.YMTo });

                query = makeRangePeriod(query, ym,
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.YMFrom },
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.YMFrom },
                    C.DeclarationValue.Declaration, new int[] { C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.YMTo });

                query.AND.ARC(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.Object, C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            }
            , (result) =>
            {
                {
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var dictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);

                    foreach (var declarationValue in result)
                    {
                        if (dictionary.ContainsKey(declarationValue.PollutionID))
                        {
                            var pollution = dictionary[declarationValue.PollutionID];

                            declarationValue.Add(pollution);
                        }
                    }
                }
            });
        }

        public IEnumerable<DeclarationValue> Find(uint wellID)
        {
            return getQuerryResult($"uint wellID={wellID}", (table) =>
            {
                table.QUERRY()
                  .SHOW
                  .WHERE
                      .ARC(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(true)
                  .AND
                      .ARC(C.DeclarationValue.Declaration, C.Declaration.Well).EQUI.BV(wellID)
                  .DO();
            }
            , (result) =>
            {
                {
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var dictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);

                    foreach (var declarationValue in result)
                    {
                        if (dictionary.ContainsKey(declarationValue.PollutionID))
                        {
                            var pollution = dictionary[declarationValue.PollutionID];

                            declarationValue.Add(pollution);
                        }
                    }
                }
            });
        }
    }
}