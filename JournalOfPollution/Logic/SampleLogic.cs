using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class SampleLogic
        : BaseLogicTyped<Sample>
    {
        public SampleLogic() : base(T.Sample)
        { }

        protected override Sample internalGetModel(uint id)
        { return new Sample(id); }

        public IEnumerable<Sample> Find(int ym, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
             {
                 var query = table.QUERRY()
                        .SHOW
                        .WHERE
                        .AC(C.Sample.YM).EQUI.BV(ym);

                 if (sampleStatus.HasValue)
                 { query.AND.AC(C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                 query.DO();
             }
             , (result)=>
             {
                 var dictionary = this.GetDictionary(result);

                 {
                     var values = LogicHelper.SelectionWellLogic.Find(ym, sampleStatus);

                     foreach (var value in values)
                     {
                         if (dictionary.ContainsKey(value.SampleID))
                         {
                             var sample = dictionary[value.SampleID];

                             sample.Add(value);
                         }
                     }
                 }

                 {
                     var volumes = LogicHelper.VolumeLogic.Find(ym, sampleStatus);

                     foreach (var volume in volumes)
                     {
                         if (dictionary.ContainsKey(volume.SampleID))
                         {
                             var sample = dictionary[volume.SampleID];

                             sample.Add(volume);
                         }
                     }
                 }

                 {
                     var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym, sampleStatus);

                     foreach (var negotiationAssistant in negotiationAssistants)
                     {
                         if (dictionary.ContainsKey(negotiationAssistant.SampleID))
                         {
                             var sample = dictionary[negotiationAssistant.SampleID];

                             sample.Add(negotiationAssistant);
                         }
                     }
                 }
             });
        }

        public IEnumerable<Sample> Find(int ym, uint clientID, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|clientID={clientID}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .AC(C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                { query.AND.AC(C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.AND.ARC(C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query.DO();
            }
             , (result) =>
             {
                 var dictionary = this.GetDictionary(result);

                 {
                     var values = LogicHelper.SelectionWellLogic.Find(ym, clientID, 1, true);

                     foreach (var value in values)
                     {
                         if (dictionary.ContainsKey(value.SampleID))
                         {
                             var sample = dictionary[value.SampleID];

                             sample.Add(value);
                         }
                     }
                 }

                 {
                     var volumes = LogicHelper.VolumeLogic.Find(ym, clientID);

                     foreach (var volume in volumes)
                     {
                         if (dictionary.ContainsKey(volume.SampleID))
                         {
                             var sample = dictionary[volume.SampleID];

                             sample.Add(volume);
                         }
                     }
                 }

                 {
                     var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym, sampleStatus);

                     foreach (var negotiationAssistant in negotiationAssistants)
                     {
                         if (dictionary.ContainsKey(negotiationAssistant.SampleID))
                         {
                             var sample = dictionary[negotiationAssistant.SampleID];

                             sample.Add(negotiationAssistant);
                         }
                     }
                 }
             });
        }

        public Sample FirstOrDefault(int ym, uint objectID, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|objectID={objectID}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var clientID = T.Objecte.Rows.Get_UnShow<uint>(objectID, C.Objecte.Client);

                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .AC(C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                { query.AND.AC(C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.AND.ARC(C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query.DO();
            }
             , (result) =>
             {
                 var dictionary = this.GetDictionary(result);

                 {
                     var selectionWell = LogicHelper.SelectionWellLogic.FirstOrDefault(ym, objectID, sampleStatus);

                     if (selectionWell != null && dictionary.ContainsKey(selectionWell.SampleID))
                     {
                         var sample = dictionary[selectionWell.SampleID];

                         sample.Add(selectionWell);
                     }
                 }

                 {
                     var volumes = LogicHelper.VolumeLogic.Find(ym, objectID, sampleStatus);

                     foreach (var volume in volumes)
                     {
                         if (dictionary.ContainsKey(volume.SampleID))
                         {
                             var sample = dictionary[volume.SampleID];

                             sample.Add(volume);
                         }
                     }
                 }

                 {
                     var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym, objectID, sampleStatus);

                     foreach (var negotiationAssistant in negotiationAssistants)
                     {
                         if (dictionary.ContainsKey(negotiationAssistant.SampleID))
                         {
                             var sample = dictionary[negotiationAssistant.SampleID];

                             sample.Add(negotiationAssistant);
                         }
                     }
                 }
             }).FirstOrDefault();
        }

        /// <summary>
        /// Метод возвращает объект sample с поломанными связанными сущьностями!
        /// </summary>
        /// <param name="maxYM"></param>
        /// <returns></returns>
        public Sample FirstOrDefault(int maxYM, uint objectID)
        {
            return getQuerryResult($"maxYM={maxYM}|objectID={objectID}", (table) =>
            {
                var clientID = T.Objecte.Rows.Get_UnShow<uint>(objectID, C.Objecte.Client);

                var query = table.QUERRY()
                       .SHOW
                       .Max(C.Sample.YM).By()
                       .WHERE
                       .AC(C.Sample.YM).Less.BV(maxYM + 1);

                query.AND.ARC(C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query.DO();
            }).FirstOrDefault();
        }

        //public IEnumerable<Sample> Find(int ymTo, uint clientID, uint wellID)
        //{
        //    return getQuerryResult($"ymTo={ymTo}|clientID={clientID}", (table) =>
        //    {
        //        var clientID = T.Objecte.Rows.Get_UnShow<uint>(objectID, C.Objecte.Client);

        //        var query = table.QUERRY()
        //               .SHOW
        //               .WHERE
        //               .AC(C.Sample.YM).EQUI.BV(ym);

        //        if (sampleStatus.HasValue)
        //        { query.AND.AC(C.Sample.Status).EQUI.BV(sampleStatus.Value); }

        //        query.AND.ARC(C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

        //        query.DO();
        //    }
        //     , (result) =>
        //     {
        //         var dictionary = this.GetDictionary(result);

        //         {
        //             var selectionWell = LogicHelper.SelectionWellLogic.FirstOrDefault(ym, objectID, sampleStatus);

        //             if (selectionWell != null && dictionary.ContainsKey(selectionWell.SampleID))
        //             {
        //                 var sample = dictionary[selectionWell.SampleID];

        //                 sample.Add(selectionWell);
        //             }
        //         }

        //         {
        //             var volumes = LogicHelper.VolumeLogic.Find(ym, objectID, sampleStatus);

        //             foreach (var volume in volumes)
        //             {
        //                 if (dictionary.ContainsKey(volume.SampleID))
        //                 {
        //                     var sample = dictionary[volume.SampleID];

        //                     sample.Add(volume);
        //                 }
        //             }
        //         }

        //         {
        //             var negotiationAssistants = LogicHelper.NegotiationAssistantLogic.Find(ym, objectID, sampleStatus);

        //             foreach (var negotiationAssistant in negotiationAssistants)
        //             {
        //                 if (dictionary.ContainsKey(negotiationAssistant.SampleID))
        //                 {
        //                     var sample = dictionary[negotiationAssistant.SampleID];

        //                     sample.Add(negotiationAssistant);
        //                 }
        //             }
        //         }
        //     }).FirstOrDefault();
        //}

        //public IEnumerable<Sample> Get
        //отбор двух последних замеров
        //SELECT s.* FROM sample s
        //join selectionwell sw on s.id= sw.sample
        //join well w on w.id= sw.well
        //where w.object = 600
        //group by s.id order by s.ym limit 2
    }
}
