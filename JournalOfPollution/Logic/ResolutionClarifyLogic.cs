using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class ResolutionClarifyLogic
        : BaseLogicTyped<ResolutionClarify>
    {
        public ResolutionClarifyLogic() : base(T.ResolutionClarify)
        { }

        protected override ResolutionClarify internalGetModel(uint id)
        { return new ResolutionClarify(id); }

        public IEnumerable<ResolutionClarify> Find(int ym)
        {
            return getQuerryResult($"int ym{ym}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym, C.ResolutionClarify.YMFrom, C.ResolutionClarify.YMTo);

                query.DO();
            }
            , (result) =>
            {
                var dictionary = this.GetDictionary(result);

                {
                    var resolutions = Helpers.LogicHelper.ResolutionLogic.Find();
                    var resolutionDictionary = Helpers.LogicHelper.ResolutionLogic.GetDictionary(resolutions);

                    foreach (var resolution in result)
                    {
                        if (resolutionDictionary.ContainsKey(resolution.ResolutionID))
                        {
                            var resolutionClarify = resolutionDictionary[resolution.ResolutionID];

                            resolution.Add(resolutionClarify);
                        }
                    }
                }

                Helpers.LogicHelper.ValueNormLogic.Find(ym);
                Helpers.LogicHelper.PriceNormLogic.Find(ym);
            });
        }

        public override IEnumerable<ResolutionClarify> Find()
        {
            return getQuerryResult($"all"
            , (table) => table.QUERRY().SHOW.DO()
            , (result) =>
            {
                var dictionary = this.GetDictionary(result);

                {
                    var resolutions = Helpers.LogicHelper.ResolutionLogic.Find();
                    var resolutionDictionary = Helpers.LogicHelper.ResolutionLogic.GetDictionary(resolutions);

                    foreach (var resolution in result)
                    {
                        if (resolutionDictionary.ContainsKey(resolution.ResolutionID))
                        {
                            var resolutionClarify = resolutionDictionary[resolution.ResolutionID];

                            resolution.Add(resolutionClarify);
                        }
                    }
                }

                {
                    var resolutionValues = Helpers.LogicHelper.ValueNormLogic.Find();

                    foreach (var resolutionValue in resolutionValues)
                    {
                        if (dictionary.ContainsKey(resolutionValue.ResolutionClarifyID))
                        {
                            var resolution = dictionary[resolutionValue.ResolutionClarifyID];

                            resolution.Add(resolutionValue);
                        }
                    }
                }
            });
        }
    }
}