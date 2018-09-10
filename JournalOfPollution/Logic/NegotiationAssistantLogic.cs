using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class NegotiationAssistantLogic
        : BaseLogicTyped<NegotiationAssistant>
    {
        public NegotiationAssistantLogic() : base(T.NegotiationAssistant)
        { }

        protected override NegotiationAssistant internalGetModel(uint id)
        { return new NegotiationAssistant(id); }

        public IEnumerable<NegotiationAssistant> Find(int ym, uint? sampleStatus = null)
        {
            return this.getQuerryResult($"int ym{ym}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
             {
                 var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .C(C.NegotiationAssistant.YM, ym);

                 if (sampleStatus.HasValue)
                 { query.AND.ARC(C.NegotiationAssistant.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                 query.DO();
             },
            (result) =>
            {
                var samples = LogicHelper.SampleLogic.Find(ym, null);
                var sampleDictionary = LogicHelper.SampleLogic.GetDictionary(samples);

                var workers = LogicHelper.WorkerLogic.Find(ym);
                var workerDictionary = LogicHelper.WorkerLogic.GetDictionary(workers);

                var objects = LogicHelper.ObjecteLogic.Find(ym);
                var objecteDictionary = LogicHelper.ObjecteLogic.GetDictionary(objects);

                foreach (var negotiationAssistant in result)
                {
                    if (sampleDictionary.ContainsKey(negotiationAssistant.SampleID))
                    {
                        var sample = sampleDictionary[negotiationAssistant.SampleID];

                        negotiationAssistant.Add(sample);
                    }
                    if (workerDictionary.ContainsKey(negotiationAssistant.WorkerID))
                    {
                        var worker = workerDictionary[negotiationAssistant.WorkerID];

                        negotiationAssistant.Add(worker);
                    }
                    if (objecteDictionary.ContainsKey(negotiationAssistant.ObjectID))
                    {
                        var objecte = objecteDictionary[negotiationAssistant.ObjectID];

                        negotiationAssistant.Add(objecte);
                    }
                }
            });
        }

        public IEnumerable<NegotiationAssistant> Find(int ym, uint objectID, uint? sampleStatus)
        {
            return this.getQuerryResult($"int ym{ym}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}|objectID={objectID}", (table) =>
            {
                var query = table.QUERRY()
                      .SHOW
                      .WHERE
                      .C(C.NegotiationAssistant.YM, ym);

                if (sampleStatus.HasValue)
                { query.AND.ARC(C.NegotiationAssistant.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.AND.AC(C.NegotiationAssistant.Objecte).EQUI.BV(objectID);

                query.DO();
            },
            (result) =>
            {
                var sample = LogicHelper.SampleLogic.FirstOrDefault(ym, objectID, sampleStatus);

                var workers = LogicHelper.WorkerLogic.Find(ym);
                var workerDictionary = LogicHelper.WorkerLogic.GetDictionary(workers);

                var objecte = LogicHelper.ObjecteLogic.FirstOrDefault(ym, objectID, null);

                foreach (var negotiationAssistant in result)
                {
                    negotiationAssistant.Add(sample);

                    if (workerDictionary.ContainsKey(negotiationAssistant.WorkerID))
                    {
                        var worker = workerDictionary[negotiationAssistant.WorkerID];

                        negotiationAssistant.Add(worker);
                    }

                    negotiationAssistant.Add(objecte);
                }
            });
        }

        public IEnumerable<NegotiationAssistant> Find(int ym, uint clientID)
        {
            return this.getQuerryResult($"int ym{ym}|clientID={clientID}", (table) =>
            {
                var query = table.QUERRY()
                      .SHOW
                      .WHERE
                      .C(C.NegotiationAssistant.YM, ym);

                query.AND.ARC(C.NegotiationAssistant.Objecte, C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            },
            (result) =>
            {
                var sample = LogicHelper.SampleLogic.Find(ym, clientID, null);

                var workers = LogicHelper.WorkerLogic.Find(ym);

                var objecte = LogicHelper.ObjecteLogic.Find(ym, clientID);
            });
        }
    }
}
