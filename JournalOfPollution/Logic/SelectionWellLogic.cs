using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class SelectionWellLogic
        : BaseLogicTyped<SelectionWell>
    {
        public SelectionWellLogic() : base(T.SelectionWell)
        { }

        protected override SelectionWell internalGetModel(uint id)
        { return new SelectionWell(id); }

        /// <summary>
        /// Поулчить список отборов
        /// </summary>
        /// <param name="ym">Период</param>
        /// <param name="sampleStatus">статус отбора</param>
        /// <param name="actualNumber">актуальный(>0) или нет</param>
        /// <returns></returns>
        public IEnumerable<SelectionWell> Find(int ym, uint? sampleStatus, bool actualNumber = true)
        {
            return getQuerryResult($"ym={ym}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                DataBase.IOrAndDo query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                { query.AND.ARC(C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                if (actualNumber)
                { query.AND.AC(C.SelectionWell.Number).More.BV<int>(0); }

                query = makeRangePeriod(query, ym,
                    C.SelectionWell.Well, new int[] { C.Well.YMFrom },
                    C.SelectionWell.Well, new int[] { C.Well.YMTo });

                query = makeRangePeriod(query, ym,
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.YMFrom },
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMFrom },
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMTo });

                query.DO();

                var models = getModels(table);

                models = models.OrderBy(x => x.Number).ToArray();

                return models;
            }
             , (result) =>
             {
                 var dictionary = this.GetDictionary(result);

                 {
                     var values = LogicHelper.ValuesSelectionLogic.Find(ym, sampleStatus);

                     foreach (var value in values)
                     {
                         if (dictionary.ContainsKey(value.SelectionWellID))
                         {
                             var selectionWell = dictionary[value.SelectionWellID];

                             selectionWell.Add(value);
                         }
                     }
                 }

                 {
                     LogicHelper.SampleLogic.Find(ym, sampleStatus); //достаточно загрузить записи, т.к. внутри производится соотнение с SelectionWell
                 }

                 {
                     var wells = LogicHelper.WellLogic.Find(ym);

                     var wellDictionary = LogicHelper.WellLogic.GetDictionary(wells);

                     foreach (var selectionWell in result)
                     {
                         if (wellDictionary.ContainsKey(selectionWell.WellID))
                         {
                             var well = wellDictionary[selectionWell.WellID];

                             selectionWell.Add(well);
                         }
                     }
                 }

                 {
                     var objects = LogicHelper.ObjecteLogic.Find(ym);

                     var ObjecteLogicDictionary = LogicHelper.ObjecteLogic.GetDictionary(objects);

                     foreach (var selectionWell in result)
                     {
                         if (ObjecteLogicDictionary.ContainsKey(selectionWell.ObjectID))
                         {
                             var objecte = ObjecteLogicDictionary[selectionWell.ObjectID];

                             selectionWell.Add(objecte);
                         }
                     }
                 }
             });
        }

        public IEnumerable<SelectionWell> Find(int ym, uint clientID, uint? sampleStatus, bool actualNumber)
        {
            return getQuerryResult($"ym={ym}|clientID={clientID}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString() : "null")}", (table) =>
            {
                DataBase.IOrAndDo query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                { query.AND.ARC(C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value); }

                if (actualNumber)
                { query.AND.AC(C.SelectionWell.Number).More.BV<int>(0); }

                query.AND.ARC(C.SelectionWell.Sample, C.Sample.Representative, C.Representative.Client).EQUI.BV(clientID);

                query = makeRangePeriod(query, ym,
                    C.SelectionWell.Well, new int[] { C.Well.YMFrom },
                    C.SelectionWell.Well, new int[] { C.Well.YMTo });

                query = makeRangePeriod(query, ym,
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.YMFrom },
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMFrom },
                    C.SelectionWell.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMTo });

                query.DO();

                var models = getModels(table);

                models = models.OrderBy(x => x.Number).ToArray();

                return models;
            }
             , (result) =>
             {
                 var dictionary = this.GetDictionary(result);

                 {
                     var values = LogicHelper.ValuesSelectionLogic.Find(ym, clientID);

                     foreach (var value in values)
                     {
                         if (dictionary.ContainsKey(value.SelectionWellID))
                         {
                             var selectionWell = dictionary[value.SelectionWellID];

                             selectionWell.Add(value);
                         }
                     }
                 }

                 {
                     LogicHelper.SampleLogic.Find(ym, clientID, sampleStatus); //достаточно загрузить записи, т.к. внутри производится соотнение с SelectionWell
                 }

                 {
                     var wells = LogicHelper.WellLogic.Find(ym);

                     var wellDictionary = LogicHelper.WellLogic.GetDictionary(wells);

                     foreach (var selectionWell in result)
                     {
                         if (wellDictionary.ContainsKey(selectionWell.WellID))
                         {
                             var well = wellDictionary[selectionWell.WellID];

                             selectionWell.Add(well);
                         }
                     }
                 }

                 {
                     var objects = LogicHelper.ObjecteLogic.Find(ym, clientID);

                     var ObjecteLogicDictionary = LogicHelper.ObjecteLogic.GetDictionary(objects);

                     foreach (var selectionWell in result)
                     {
                         if (ObjecteLogicDictionary.ContainsKey(selectionWell.ObjectID))
                         {
                             var objecte = ObjecteLogicDictionary[selectionWell.ObjectID];

                             selectionWell.Add(objecte);
                         }
                     }
                 }
             });
        }

        public SelectionWell FirstOrDefault(int ym, uint objectID, uint? sampleStatus)
        {
            return getQuerryResult($"ym={ym}|objectID={objectID}|sampleStatus={(sampleStatus.HasValue ? sampleStatus.Value.ToString(): "null")}", (table) =>
            {
                var query = table.QUERRY()
                       .SHOW
                       .WHERE
                       .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(ym);

                if (sampleStatus.HasValue)
                {
                    query.AND.ARC(C.SelectionWell.Sample, C.Sample.Status).EQUI.BV(sampleStatus.Value);
                }

                query.AND.ARC(C.SelectionWell.Well, C.Well.Object).EQUI.BV(objectID);

                query.DO();
            }
             , (result) =>
             {
                 var dictionary = this.GetDictionary(result);

                 {
                     var values = LogicHelper.ValuesSelectionLogic.Find(ym, objectID, sampleStatus);

                     foreach (var value in values)
                     {
                         if (dictionary.ContainsKey(value.SelectionWellID))
                         {
                             var selectionWell = dictionary[value.SelectionWellID];

                             selectionWell.Add(value);
                         }
                     }
                 }

                 {
                     LogicHelper.SampleLogic.FirstOrDefault(ym, objectID, sampleStatus); //достаточно загрузить записи, т.к. внутри производится соотнение с SelectionWell
                 }

                 {
                     var wells = LogicHelper.WellLogic.Find(ym);

                     var wellDictionary = LogicHelper.WellLogic.GetDictionary(wells);

                     foreach (var selectionWell in result)
                     {
                         if (wellDictionary.ContainsKey(selectionWell.WellID))
                         {
                             var well = wellDictionary[selectionWell.WellID];

                             selectionWell.Add(well);
                         }
                     }
                 }

                 {
                     var objects = LogicHelper.ObjecteLogic.Find(ym);

                     var ObjecteLogicDictionary = LogicHelper.ObjecteLogic.GetDictionary(objects);

                     foreach (var selectionWell in result)
                     {
                         if (ObjecteLogicDictionary.ContainsKey(selectionWell.ObjectID))
                         {
                             var objecte = ObjecteLogicDictionary[selectionWell.ObjectID];

                             selectionWell.Add(objecte);
                         }
                     }
                 }
             }).FirstOrDefault();
        }
    }
}