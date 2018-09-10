using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class PeiodLogic
        : BaseLogicTyped<Period>
    {
        public PeiodLogic() : base(T.Period)
        { }

        protected override Period internalGetModel(uint id)
        { return new Period(id); }

        public new IEnumerable<Period> Find()
        {
            return getQuerryResult($"all", (table) =>
            {
                table.QUERRY().SHOW.DO();

                var result = getModels(table);

                result.OrderBy(x => x.YM).ToArray();

                return result;
            });
        }

        /// <summary>
        /// Получить период по номеру месяца
        /// </summary>
        /// <param name="ym">Номер месяца</param>
        /// <returns></returns>
        public Period FirstOrDefault(int ym)
        {
            return getQuerryResult($"first ym={ym}", (table) =>
            {
                table.QUERRY()
                    .SHOW
                    .Max(C.Period.YM).By()
                    .WHERE
                    .AC(C.Period.YM).Less.BV(ym + 1)
                    .DO();

                var result = getModels(table);

                result.OrderBy(x => x.YM).ToArray();

                return result;
            }).FirstOrDefault();
        }

        public IEnumerable<Period> Find(int ym, uint objectID, uint? sampleStatus)
        {
            return Find(ym, sampleStatus);
        }

        public IEnumerable<Period> Find(int ym, uint clientID)
        {
            return Find(ym, null);
        }

        public IEnumerable<Period> Find(int ym, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym},sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .AC(C.Period.YM).Less.BV(ym + 2);

                query.DO();

                var result = getModels(table);

                result.OrderBy(x => x.YM).ToArray();

                return result;
            });
        }

        public IEnumerable<Period> Find(uint sampleID)
        {
            return getQuerryResult($"sampleID={sampleID}", (table) =>
            {
                uint[] periodIDs;
                {
                    var tableVolume = T.Volume.CreateSubTable(false);

                    tableVolume.QUERRY().SHOW.WHERE.C(C.Volume.Sample, sampleID).DO();

                    periodIDs = new uint[tableVolume.Rows.Count];

                    for (int i = 0; i < tableVolume.Rows.Count; i++)
                    {
                        periodIDs[i] = tableVolume.Rows.Get_UnShow<uint>(i, C.Volume.Period);
                    }
                }

                var query = (DataBase.IAOperations)table.QUERRY()
                       .SHOW
                       .WHERE;

                foreach (var periodID in periodIDs)
                { var temp = query.ID(periodID).OR; }

                ((DataBase.IDo)query).DO();

                var result = getModels(table);

                result.OrderBy(x => x.YM).ToArray();

                return result;
            });
        }
    }
}