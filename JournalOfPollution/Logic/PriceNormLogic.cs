using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class PriceNormLogic
        : BaseLogicTyped<PriceNorm>
    {
        public PriceNormLogic() : base(T.PriceNorm)
        { }

        protected override PriceNorm internalGetModel(uint id)
        { return new PriceNorm(id); }

        public override IEnumerable<PriceNorm> Find()
        {
            return getQuerryResult($""
                , (table) =>
                {
                    table.QUERRY().SHOW.DO();
                }
                , (result) =>
                 {
                     {
                         var pollutions = LogicHelper.PollutionLogic.Find();
                         var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find();

                         var popullitionDictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);
                         var resolutionClarifyDictionary = LogicHelper.ResolutionClarifyLogic.GetDictionary(resolutionClarifies);

                         foreach (var priceNorm in result)
                         {
                             if (popullitionDictionary.ContainsKey(priceNorm.PollutionID))
                             {
                                 var pollution = popullitionDictionary[priceNorm.PollutionID];

                                 priceNorm.Add(pollution);
                             }
                             if (resolutionClarifyDictionary.ContainsKey(priceNorm.ResolutionClarifyID))
                             {
                                 var resolutionClarify = resolutionClarifyDictionary[priceNorm.ResolutionClarifyID];

                                 priceNorm.Add(resolutionClarify);
                             }
                         }
                     }
                 });
        }

        public IEnumerable<PriceNorm> Find(int ym)
        {
            return getQuerryResult($"ym={ym}"
                , (table) =>
                {
                    DataBase.IOrAndDo query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                                                C.PriceNorm.YMFrom,
                                                C.PriceNorm.YMTo);

                    query = makeRangePeriod(query, ym,
                                                C.PriceNorm.ResolutionClarify, new int[] { C.ResolutionClarify.YMFrom },
                                                C.PriceNorm.ResolutionClarify, new int[] { C.ResolutionClarify.YMTo });

                    query = makeRangePeriod(query, ym,
                                                C.PriceNorm.ResolutionClarify, new int[] { C.ResolutionClarify.Resolution, C.Resolution.YMFrom },
                                                C.PriceNorm.ResolutionClarify, new int[] { C.ResolutionClarify.Resolution, C.Resolution.YMTo });

                    query.DO();
                }
                , (result) =>
                {
                    {
                        var pollutions = LogicHelper.PollutionLogic.Find();
                        var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find(ym);

                        var pollutionDictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);
                        var resolutionClarifyDictionary = LogicHelper.ResolutionClarifyLogic.GetDictionary(resolutionClarifies);

                        foreach (var priceNorm in result)
                        {
                            if (pollutionDictionary.ContainsKey(priceNorm.PollutionID))
                            {
                                var pollution = pollutionDictionary[priceNorm.PollutionID];

                                priceNorm.Add(pollution);
                            }
                            if (resolutionClarifyDictionary.ContainsKey(priceNorm.ResolutionClarifyID))
                            {
                                var resolutionClarify = resolutionClarifyDictionary[priceNorm.ResolutionClarifyID];

                                priceNorm.Add(resolutionClarify);
                                resolutionClarify.Add(priceNorm);
                            }
                        }
                    }
                });
        }
    }
}