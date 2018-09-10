using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class VolumeLogic
        : BaseLogicTyped<Volume>
    {
        public VolumeLogic() : base(T.Volume)
        { }

        protected override Volume internalGetModel(uint id)
        { return new Volume(id); }

        public IEnumerable<Volume> Find(int ym, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.Volume.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                { query.AND.ARC(C.Volume.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.DO();
            },
            (result) =>
            {
                var periods = Helpers.LogicHelper.PeiodLogic.Find(ym, sampleStatus);
                var dictionary = Helpers.LogicHelper.PeiodLogic.GetDictionary(periods);

                foreach (var volume in result)
                {
                    if (dictionary.ContainsKey(volume.PeriodID))
                    {
                        var period = dictionary[volume.PeriodID];
                        volume.Add(period);
                    }
                }
            });
        }

        public IEnumerable<Volume> Find(int ym, uint objectID, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|objectID={objectID}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var clientID = T.Objecte.Rows.Get_UnShow<uint>(objectID, C.Objecte.Client);
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.Volume.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                { query.AND.ARC(C.Volume.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.AND.ARC(C.Volume.Sample, C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query.DO();

                var result = getModels(table);

                result = result.OrderBy(x => x.Period.YM);

                return result;
            },
            (result) =>
            {
                var periods = Helpers.LogicHelper.PeiodLogic.Find(ym, objectID, sampleStatus);
                var dictionary = Helpers.LogicHelper.PeiodLogic.GetDictionary(periods);

                foreach (var volume in result)
                {
                    if (dictionary.ContainsKey(volume.PeriodID))
                    {
                        var period = dictionary[volume.PeriodID];
                        volume.Add(period);
                    }
                }
            });
        }


        public IEnumerable<Volume> Find(int ym, uint clientID)
        {
            return getQuerryResult($"ym={ym}|clientID={clientID}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.Volume.Sample, C.Sample.YM).EQUI.BV(ym);
                
                query.AND.ARC(C.Volume.Sample, C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query.DO();

                var result = getModels(table);

                result = result.OrderBy(x => x.Period.YM);

                return result;
            },
            (result) =>
            {
                var periods = Helpers.LogicHelper.PeiodLogic.Find(ym, clientID);
                var dictionary = Helpers.LogicHelper.PeiodLogic.GetDictionary(periods);

                foreach (var volume in result)
                {
                    if (dictionary.ContainsKey(volume.PeriodID))
                    {
                        var period = dictionary[volume.PeriodID];
                        volume.Add(period);
                    }
                }
            });
        }

        public IEnumerable<Volume> Find(uint sampleID)
        {
            return getQuerryResult($"sampleID={sampleID}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .AC(C.Volume.Sample).EQUI.BV(sampleID);

                query.DO();
            },
            (result) =>
            {
                var periods = Helpers.LogicHelper.PeiodLogic.Find(sampleID);
                var dictionary = Helpers.LogicHelper.PeiodLogic.GetDictionary(periods);

                foreach (var volume in result)
                {
                    if (dictionary.ContainsKey(volume.PeriodID))
                    {
                        var period = dictionary[volume.PeriodID];
                        volume.Add(period);
                    }
                }
            });
        }
    }
}
