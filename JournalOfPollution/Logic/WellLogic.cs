using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class WellLogic
        : BaseLogicTyped<Well>
    {
        public WellLogic() : base(T.Well)
        { }

        protected override Well internalGetModel(uint id)
        { return new Well(id); }

        public IEnumerable<Well> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"ym={ym}|canSelect={true}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Well.YMFrom,
                    C.Well.YMTo);

                query = makeRangePeriod(query, ym,
                    C.Well.Object, new int[] { C.Objecte.YMFrom },
                    C.Well.Object, new int[]{C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.Well.Object, new int[] { C.Objecte.Client, C.Client.YMFrom },
                    C.Well.Object, new int[] { C.Objecte.Client, C.Client.YMTo });

                if (canSelect.HasValue)
                { query = query.AND.ARC(C.Well.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            }
            , (result)=>
            {
                var wellDeclaration = GetDictionary(result);

                {
                    var declarations = LogicHelper.DeclarationLogic.Find(ym, canSelect);

                    foreach (var declaration in declarations)
                    {
                        if (wellDeclaration.ContainsKey(declaration.WellID))
                        {
                            var well = wellDeclaration[declaration.WellID];

                            well.Add(declaration);
                        }
                    }
                }

                {
                    var objects = LogicHelper.ObjecteLogic.Find(ym, canSelect);
                    var objectDictionary = LogicHelper.ObjecteLogic.GetDictionary(objects);

                    foreach (var well in result)
                    {
                        if (objectDictionary.ContainsKey(well.ObjectID))
                        {
                            var objecte = objectDictionary[well.ObjectID];

                            well.Add(objecte);
                        }
                    }
                }
            });
        }

        public IEnumerable<Well> Find(int ym, uint objectID, bool? canSelect)
        {
            return getQuerryResult($"ym={ym}|objectID={objectID}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString() : "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Well.YMFrom,
                    C.Well.YMTo);

                query = makeRangePeriod(query, ym,
                    C.Well.Object, new int[] { C.Objecte.YMFrom },
                    C.Well.Object, new int[] { C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.Well.Object, new int[] { C.Objecte.Client, C.Client.YMFrom },
                    C.Well.Object, new int[] { C.Objecte.Client, C.Client.YMTo });

                if (canSelect.HasValue)
                { query = query.AND.ARC(C.Well.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query = query.AND.AC(C.Well.Object).EQUI.BV(objectID);

                query.DO();
            }
            , (result) =>
            {
                var wellDeclaration = GetDictionary(result);

                {
                    var declarations = LogicHelper.DeclarationLogic.Find(ym, canSelect);

                    foreach (var declaration in declarations)
                    {
                        if (wellDeclaration.ContainsKey(declaration.WellID))
                        {
                            var well = wellDeclaration[declaration.WellID];

                            well.Add(declaration);
                        }
                    }
                }

                {
                    var objecte = LogicHelper.ObjecteLogic.FirstOrDefault(ym, objectID, canSelect);

                    foreach (var well in result)
                    {
                        well.Add(objecte);
                    }
                }
            });
        }

        public IEnumerable<Well> Find(int ym, uint clientID)
        {
            return getQuerryResult($"ym={ym}|clientID={clientID}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Well.YMFrom,
                    C.Well.YMTo);

                query = makeRangePeriod(query, ym,
                    C.Well.Object, new int[] { C.Objecte.YMFrom },
                    C.Well.Object, new int[] { C.Objecte.YMTo });

                query = makeRangePeriod(query, ym,
                    C.Well.Object, new int[] { C.Objecte.Client, C.Client.YMFrom },
                    C.Well.Object, new int[] { C.Objecte.Client, C.Client.YMTo });

                query = query.AND.ARC(C.Well.Object, C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            }
            , (result) =>
            {
                var wellDeclaration = GetDictionary(result);

                {
                    var declarations = LogicHelper.DeclarationLogic.Find(ym, clientID);

                    foreach (var declaration in declarations)
                    {
                        if (wellDeclaration.ContainsKey(declaration.WellID))
                        {
                            var well = wellDeclaration[declaration.WellID];

                            well.Add(declaration);
                        }
                    }
                }

                {
                    var objecte = LogicHelper.ObjecteLogic.FirstOrDefault(ym, clientID);

                    foreach (var well in result)
                    {
                        well.Add(objecte);
                    }
                }
            });
        }
    }
}