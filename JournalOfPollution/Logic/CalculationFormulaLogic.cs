using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class CalculationFormulaLogic
        : BaseLogicTyped<CalculationFormula>
    {
        public CalculationFormulaLogic() : base(T.CalculationFormula)
        { }

        protected override CalculationFormula internalGetModel(uint id)
        { return new CalculationFormula(id); }

        public IEnumerable<CalculationFormula> Find(int ym)
        {
            return getQuerryResult($"ym={ym}"
                , (table) =>
                {
                    table.QUERRY()
                    .SHOW
                    .WHERE
                        .AC(C.CalculationFormula.YM).Less.BV(ym + 1)
                    .DO();

                    var result = getModels(table);

                    var dictionary = new Dictionary<uint, CalculationFormula>();    //группировка по норме

                    foreach (var сalculationFormula in result)
                    {
                        if (dictionary.ContainsKey(сalculationFormula.ResolutionClarifyID))
                        {
                            var cfFinded = dictionary[сalculationFormula.ResolutionClarifyID];

                            if (cfFinded.YM < сalculationFormula.YM)
                            { dictionary[сalculationFormula.ResolutionClarifyID] = сalculationFormula; }
                        }
                        else
                        { dictionary.Add(сalculationFormula.ResolutionClarifyID, сalculationFormula); }
                    }

                    return dictionary.Values.ToArray();
                }
                , (result) =>
                {
                    {
                        var pollutions = LogicHelper.PollutionLogic.Find();
                        var dictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);

                        foreach (var formula in result)
                        {
                            if (dictionary.ContainsKey(formula.PollutionID))
                            {
                                var pollution = dictionary[formula.PollutionID];

                                formula.Add(pollution);
                            }
                        }
                    }
                    {
                        var resolutionClarifies = LogicHelper.ResolutionClarifyLogic.Find(ym);
                        var dictionary = LogicHelper.ResolutionClarifyLogic.GetDictionary(resolutionClarifies);

                        foreach (var formula in result)
                        {
                            if (dictionary.ContainsKey(formula.PollutionID))
                            {
                                var resolutionClarify = dictionary[formula.PollutionID];

                                formula.Add(resolutionClarify);
                            }
                        }
                    }
                });
        }
    }
}