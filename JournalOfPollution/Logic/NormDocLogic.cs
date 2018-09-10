using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class NormDocLogic
        : BaseLogicTyped<NormDoc>
    {
        public NormDocLogic() : base(T.NormDoc)
        { }

        protected override NormDoc internalGetModel(uint id)
        { return new NormDoc(id); }

        public override IEnumerable<NormDoc> Find()
        {
            return getQuerryResult($"all", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW;

                //if (sampleStatus.HasValue)
                //{ query.AND.ARC(C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.DO();
            }
            , (result) =>
            {
                {
                    var resolutions = Helpers.LogicHelper.ResolutionLogic.Find();
                    var dictionary = Helpers.LogicHelper.ResolutionLogic.GetDictionary(resolutions);

                    foreach (var normDoc in result)
                    {
                        if (dictionary.ContainsKey(normDoc.ResolutionID))
                        {
                            var resolution = dictionary[normDoc.ResolutionID];

                            resolution.ListNormDoc.Add(normDoc);
                        }
                    }
                }
            });
        }

        public IEnumerable<NormDoc> Find(uint sampleID)
        {
            return getQuerryResult($"uint sampleID={sampleID}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.NormDoc.Volume, C.Volume.Sample).EQUI.BV(sampleID);

                //if (sampleStatus.HasValue)
                //{ query.AND.ARC(C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                query.DO();
            }
            , (result) =>
            {
                {
                    var resolutions = Helpers.LogicHelper.ResolutionLogic.Find();
                    var dictionary = Helpers.LogicHelper.ResolutionLogic.GetDictionary(resolutions);

                    foreach (var normDoc in result)
                    {
                        if (dictionary.ContainsKey(normDoc.ResolutionID))
                        {
                            var resolution = dictionary[normDoc.ResolutionID];

                            resolution.ListNormDoc.Add(normDoc);
                        }
                    }
                }
            });
        }
    }
}
