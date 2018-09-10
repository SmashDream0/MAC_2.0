using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class ResolutionLogic
        : BaseLogicTyped<Resolution>
    {
        public ResolutionLogic() : base(T.Resolution)
        { }

        protected override Resolution internalGetModel(uint id)
        { return new Resolution(id); }

        public IEnumerable<Resolution> Find(int ym)
        {
            return getQuerryResult($"int ym={ym}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym, 
                    C.Resolution.YMFrom,
                    C.Resolution.YMTo);

                query.DO();
            }
            , (result)=>
            {
                var resolutionDeclaration = this.GetDictionary(result);

                {
                    var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find(ym);

                    foreach (var resolutionClarify in resolutionClarifies)
                    {
                        if (resolutionDeclaration.ContainsKey(resolutionClarify.ResolutionID))
                        {
                            var resolution = resolutionDeclaration[resolutionClarify.ResolutionID];

                            resolution.ListResolutionClarify.Add(resolutionClarify);
                        }
                    }
                }

                {
                    var normDocs = Helpers.LogicHelper.NormDocLogic.Find();

                    foreach (var normDoc in normDocs)
                    {
                        if (resolutionDeclaration.ContainsKey(normDoc.ResolutionID))
                        {
                            var resolution = resolutionDeclaration[normDoc.ResolutionID];

                            resolution.ListNormDoc.Add(normDoc);
                        }
                    }
                }
            });
        }
    }
}
