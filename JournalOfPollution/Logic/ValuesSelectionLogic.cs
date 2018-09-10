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
    public class ValuesSelectionLogic
        : BaseLogicTyped<ValueSelection>
    {
        public ValuesSelectionLogic():base(T.ValueSelection)
        { }

        protected override ValueSelection internalGetModel(uint id)
        { return new ValueSelection(id); }

        public IEnumerable<ValueSelection> Find(int ym, uint? sampleStatus = null)
        {
            return getQuerryResult($"ym={ym}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                {
                    query.AND.ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value);
                }

                query.DO();
            }
            , (result) =>
             {
                 var pollitions = Helpers.LogicHelper.PollutionLogic.Find();

                 var dictionary = Helpers.LogicHelper.PollutionLogic.GetDictionary(pollitions);

                 foreach (var valueSelection in result)
                 {
                     if (dictionary.ContainsKey(valueSelection.PollutionID))
                     {
                         var pollution = dictionary[valueSelection.PollutionID];

                         valueSelection.Add(pollution);
                     }
                 }
             });
        }

        public IEnumerable<ValueSelection> Find(int ym, uint objectID, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|objectID={objectID}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                {
                    query.AND.ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value);
                }

                query.AND.ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Well, C.Well.Object).EQUI.BV(objectID);

                query.DO();
            }
            , (result) =>
            {
                var pollitions = Helpers.LogicHelper.PollutionLogic.Find();

                var dictionary = Helpers.LogicHelper.PollutionLogic.GetDictionary(pollitions);

                foreach (var valueSelection in result)
                {
                    if (dictionary.ContainsKey(valueSelection.PollutionID))
                    {
                        var pollution = dictionary[valueSelection.PollutionID];

                        valueSelection.Add(pollution);
                    }
                }
            });
        }

        public IEnumerable<ValueSelection> Find(int ym, uint clientID)
        {
            return getQuerryResult($"ym={ym}|clientID={clientID}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(ym);

                query.AND.ARC(C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query.DO();
            }
            , (result) =>
            {
                var pollitions = Helpers.LogicHelper.PollutionLogic.Find();

                var dictionary = Helpers.LogicHelper.PollutionLogic.GetDictionary(pollitions);

                foreach (var valueSelection in result)
                {
                    if (dictionary.ContainsKey(valueSelection.PollutionID))
                    {
                        var pollution = dictionary[valueSelection.PollutionID];

                        valueSelection.Add(pollution);
                    }
                }
            });
        }

        private static void load(IEnumerable<ValueSelection> result)
        {
            {
                var pollutions = LogicHelper.PollutionLogic.Find();
                var dictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);

                foreach (var valueSelector in result)
                {
                    if (dictionary.ContainsKey(valueSelector.PollutionID))
                    {
                        var pollution = dictionary[valueSelector.PollutionID];

                        valueSelector.Add(pollution);
                    }
                }
            }
            {
                var selectionWells = LogicHelper.SelectionWellLogic.Find();
                var dictionary = LogicHelper.SelectionWellLogic.GetDictionary(selectionWells);

                foreach (var valueSelector in result)
                {
                    if (dictionary.ContainsKey(valueSelector.PollutionID))
                    {
                        var selectionWell = dictionary[valueSelector.PollutionID];

                        valueSelector.Add(selectionWell);
                    }
                }
            }
        }
    }
}