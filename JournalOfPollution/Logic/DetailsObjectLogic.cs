using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class DetailsObjectLogic
        : BaseLogicTyped<DetailsObject>
    {
        public DetailsObjectLogic() : base(T.DetailsObject)
        { }

        protected override DetailsObject internalGetModel(uint id)
        { return new DetailsObject(id); }

        /// <summary>
        /// Вообще тут плохо должно работать
        /// </summary>
        /// <param name="ym"></param>
        /// <returns></returns>
        public IEnumerable<DetailsObject> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"long ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                {
                    var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                          C.DetailsObject.Object, new int[] { C.Objecte.Client, C.Client.YMFrom },
                          C.DetailsObject.Object, new int[] { C.Objecte.Client, C.Client.YMTo });

                    query = makeRangePeriod(query, ym,
                        C.DetailsObject.Object, new int[] { C.Objecte.YMFrom },
                        C.DetailsObject.Object, new int[] { C.Objecte.YMTo });

                    query = makeRangePeriodFrom(query, ym, C.DetailsObject.YM);

                    if (canSelect.HasValue)
                    { query.AND.ARC(C.DetailsObject.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                    query.DO();
                }

                var models = getModels(table);

                var dictionary = new Dictionary<uint, DetailsObject>();

                foreach (var model in models)
                {
                    if (dictionary.ContainsKey(model.ObjectID))
                    {
                        var findedModel = dictionary[model.ObjectID];

                        if (findedModel.YM < model.YM || (findedModel.YM == model.YM && findedModel.ID < model.ID))
                        { dictionary[model.ObjectID] = model; }
                    }
                    else
                    { dictionary.Add(model.ObjectID, model); }
                }

                return dictionary.Values.ToArray();
            }
            , (result)=>
            {

            });
        }

        /// <summary>
        /// Вообще тут плохо должно работать
        /// </summary>
        /// <param name="ym"></param>
        /// <returns></returns>
        public IEnumerable<DetailsObject> Find(int ym, uint objectID, bool? canSelect)
        {
            return getQuerryResult($"long ym={ym}|objectID={objectID}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                {
                    var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                          C.DetailsObject.Object, new int[] { C.Objecte.Client, C.Client.YMFrom },
                          C.DetailsObject.Object, new int[] { C.Objecte.Client, C.Client.YMTo });

                    query = makeRangePeriod(query, ym,
                        C.DetailsObject.Object, new int[] { C.Objecte.YMFrom },
                        C.DetailsObject.Object, new int[] { C.Objecte.YMTo });

                    query = makeRangePeriodFrom(query, ym, C.DetailsObject.YM);

                    if (canSelect.HasValue)
                    { query.AND.ARC(C.DetailsObject.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                    query.AND.AC(C.DetailsObject.Object).EQUI.BV(objectID);

                    query.DO();
                }

                var models = getModels(table);

                var dictionary = new Dictionary<uint, DetailsObject>();

                foreach (var model in models)
                {
                    if (dictionary.ContainsKey(model.ObjectID))
                    {
                        var findedModel = dictionary[model.ObjectID];

                        if (findedModel.YM < model.YM || (findedModel.YM == model.YM && findedModel.ID < model.ID))
                        { dictionary[model.ObjectID] = model; }
                    }
                    else
                    { dictionary.Add(model.ObjectID, model); }
                }

                return dictionary.Values.ToArray();
            }
            , (result) =>
            {

            });
        }

        public IEnumerable<DetailsObject> Find(int ym, uint clientID)
        {
            return getQuerryResult($"long ym={ym}|clientID={clientID}", (table) =>
            {
                {
                    var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                          C.DetailsObject.Object, new int[] { C.Objecte.Client, C.Client.YMFrom },
                          C.DetailsObject.Object, new int[] { C.Objecte.Client, C.Client.YMTo });

                    query = makeRangePeriod(query, ym,
                        C.DetailsObject.Object, new int[] { C.Objecte.YMFrom },
                        C.DetailsObject.Object, new int[] { C.Objecte.YMTo });

                    query = makeRangePeriodFrom(query, ym, C.DetailsObject.YM);

                    query.AND.ARC(C.DetailsObject.Object, C.Objecte.Client).EQUI.BV(clientID);

                    query.DO();
                }

                var models = getModels(table);

                var dictionary = new Dictionary<uint, DetailsObject>();

                foreach (var model in models)
                {
                    if (dictionary.ContainsKey(model.ObjectID))
                    {
                        var findedModel = dictionary[model.ObjectID];

                        if (findedModel.YM < model.YM || (findedModel.YM == model.YM && findedModel.ID < model.ID))
                        { dictionary[model.ObjectID] = model; }
                    }
                    else
                    { dictionary.Add(model.ObjectID, model); }
                }

                return dictionary.Values.ToArray();
            }
            , (result) =>
            {

            });
        }
    }
}
