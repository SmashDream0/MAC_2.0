using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class ValueNormLogic
        : BaseLogicTyped<ValueNorm>
    {
        public ValueNormLogic() : base(T.ValueNorm)
        { }

        protected override ValueNorm internalGetModel(uint id)
        { return new ValueNorm(id); }

        public override IEnumerable<ValueNorm> Find()
        {
            return getQuerryResult($"all"
                , (table) =>
                {
                    table.QUERRY().SHOW.DO();
                }
                , (result) =>
                {
                    var units = LogicHelper.UnitLogic.Find();
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find();

                    var unitDictionary = LogicHelper.UnitLogic.GetDictionary(units);
                    var pollutionDictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);
                    var resolutionClarifyDictionary = LogicHelper.ResolutionClarifyLogic.GetDictionary(resolutionClarifies);

                    foreach (var valueNorm in result)
                    {
                        if (unitDictionary.ContainsKey(valueNorm.UnitID))
                        {
                            var unit = unitDictionary[valueNorm.UnitID];

                            valueNorm.Add(unit);
                        }
                        if (pollutionDictionary.ContainsKey(valueNorm.PollutionID))
                        {
                            var pollution = pollutionDictionary[valueNorm.PollutionID];

                            valueNorm.Add(pollution);
                        }
                        if (resolutionClarifyDictionary.ContainsKey(valueNorm.ResolutionClarifyID))
                        {
                            var resolutionClarify = resolutionClarifyDictionary[valueNorm.ResolutionClarifyID];

                            valueNorm.Add(resolutionClarify);
                        }
                    }
                 });
        }
        
        public IEnumerable<ValueNorm> Find(int ym, uint resolutionID, uint pollutionID, uint unitID)
        {
            return getQuerryResult($"ym={ym}|resolutionID={resolutionID}|pollutionID={pollutionID}|unitID={unitID}"
                , (table) =>
                {
                    var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                        C.ValueNorm.ResolutionClarify, new int[] { C.ResolutionClarify.YMFrom },
                        C.ValueNorm.ResolutionClarify, new int[] { C.ResolutionClarify.YMTo });

                    query = makeRangePeriod(query.AND, ym,
                        C.ValueNorm.YMFrom,
                        C.ValueNorm.YMTo);

                    query
                        .AND
                           .ARC(C.ValueNorm.ResolutionClarify, C.ResolutionClarify.Resolution).EQUI.BV(resolutionID)
                       .AND
                           .C(C.ValueNorm.Pollution, pollutionID);

                    if (unitID > 0)
                    { query.AND.C(C.ValueNorm.Unit, unitID); }

                    query.DO();
                }
                , (result) =>
                {
                    var units = LogicHelper.UnitLogic.Find();
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find();

                    var unitDictionary = LogicHelper.UnitLogic.GetDictionary(units);
                    var pollutionDictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);
                    var resolutionClarifyDictionary = LogicHelper.ResolutionClarifyLogic.GetDictionary(resolutionClarifies);

                    foreach (var valueNorm in result)
                    {
                        if (unitDictionary.ContainsKey(valueNorm.UnitID))
                        {
                            var unit = unitDictionary[valueNorm.UnitID];

                            valueNorm.Add(unit);
                        }
                        if (pollutionDictionary.ContainsKey(valueNorm.PollutionID))
                        {
                            var pollution = pollutionDictionary[valueNorm.PollutionID];

                            valueNorm.Add(pollution);
                        }
                        if (resolutionClarifyDictionary.ContainsKey(valueNorm.ResolutionClarifyID))
                        {
                            var resolutionClarify = resolutionClarifyDictionary[valueNorm.ResolutionClarifyID];

                            valueNorm.Add(resolutionClarify);
                        }
                    }
                });
        }

        public IEnumerable<ValueNorm> Find(int ym)
        {
            return getQuerryResult($"ym={ym}"
                , (table) =>
                {
                    var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                        C.ValueNorm.YMFrom,
                        C.ValueNorm.YMTo);

                    query = makeRangePeriod(query, ym,
                        C.ValueNorm.ResolutionClarify, new int[] { C.ResolutionClarify.YMFrom },
                        C.ValueNorm.ResolutionClarify, new int[] { C.ResolutionClarify.YMTo });

                    query = makeRangePeriod(query, ym,
                        C.ValueNorm.ResolutionClarify, new int[] { C.ResolutionClarify.Resolution, C.Resolution.YMFrom },
                        C.ValueNorm.ResolutionClarify, new int[] { C.ResolutionClarify.Resolution, C.Resolution.YMTo });

                    query.DO();
                }
                , (result) =>
                {
                    var units = LogicHelper.UnitLogic.Find();
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find(ym);

                    var unitDictionary = LogicHelper.UnitLogic.GetDictionary(units);
                    var pollutionDictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);
                    var resolutionClarifyDictionary = LogicHelper.ResolutionClarifyLogic.GetDictionary(resolutionClarifies);

                    foreach (var valueNorm in result)
                    {
                        if (unitDictionary.ContainsKey(valueNorm.UnitID))
                        {
                            var unit = unitDictionary[valueNorm.UnitID];

                            valueNorm.Add(unit);
                        }
                        if (pollutionDictionary.ContainsKey(valueNorm.PollutionID))
                        {
                            var pollution = pollutionDictionary[valueNorm.PollutionID];

                            valueNorm.Add(pollution);
                        }
                        if (resolutionClarifyDictionary.ContainsKey(valueNorm.ResolutionClarifyID))
                        {
                            var resolutionClarify = resolutionClarifyDictionary[valueNorm.ResolutionClarifyID];

                            valueNorm.Add(resolutionClarify);
                            resolutionClarify.Add(valueNorm);
                        }
                    }
                });
        }
    }
}
