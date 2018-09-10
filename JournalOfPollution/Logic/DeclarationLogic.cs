using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class DeclarationLogic
        : BaseLogicTyped<Declaration>
    {
        public DeclarationLogic() : base(T.Declaration)
        { }

        protected override Declaration internalGetModel(uint id)
        { return new Declaration(id); }

        public IEnumerable<Declaration> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"int ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Declaration.Well, new int[] { C.Well.YMFrom },
                    C.Declaration.Well, new int[] { C.Well.YMTo });

                query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.YMFrom },
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.YMTo });

                query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMFrom },
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMTo });

                if (canSelect.HasValue)
                { query = query.AND.ARC(C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            }
            ,(result)=>
            {
                var dictionaryDeclaration = GetDictionary(result);

                var declarationValues = LogicHelper.DeclarationValueLogic.Find(ym, canSelect);

                foreach (var declarationValue in declarationValues)
                {
                    if (dictionaryDeclaration.ContainsKey(declarationValue.DeclarationID))
                    {
                        var declaration = dictionaryDeclaration[declarationValue.DeclarationID];

                        declaration.DeclarationValues.Add(declarationValue);
                    }
                }
            });
        }
        
        public IEnumerable<Declaration> Find(int ym, uint clientID)
        {
            return getQuerryResult($"int ym={ym}|clientID={clientID}", (table) =>
            {
                var query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Declaration.Well, new int[] { C.Well.YMFrom },
                    C.Declaration.Well, new int[] { C.Well.YMTo });

                query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.YMFrom },
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.YMTo });

                query = makeRangePeriod(table.QUERRY().SHOW.WHERE, ym,
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMFrom },
                    C.Declaration.Well, new int[] { C.Well.Object, C.Objecte.Client, C.Client.YMTo });

                query = query.AND.ARC(C.Declaration.Well, C.Well.Object, C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            }
            , (result) =>
            {
                var dictionaryDeclaration = GetDictionary(result);

                var declarationValues = LogicHelper.DeclarationValueLogic.Find(ym, clientID);

                foreach (var declarationValue in declarationValues)
                {
                    if (dictionaryDeclaration.ContainsKey(declarationValue.DeclarationID))
                    {
                        var declaration = dictionaryDeclaration[declarationValue.DeclarationID];

                        declaration.DeclarationValues.Add(declarationValue);
                    }
                }
            });
        }

        public IEnumerable<Declaration> Find(uint wellID)
        {
            return getQuerryResult($"uint wellID={wellID}",
            (table) =>
            {
                var query = table.QUERRY()
                .SHOW
                .WHERE
                    .ARC(C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(true)
                .AND
                    .AC(C.Declaration.Well).EQUI.BV(wellID);

                query.DO();
            }
            , (result) =>
            {
                var dictionaryDeclaration = GetDictionary(result);

                var declarationValues = LogicHelper.DeclarationValueLogic.Find(wellID);

                foreach (var declarationValue in declarationValues)
                {
                    if (dictionaryDeclaration.ContainsKey(declarationValue.DeclarationID))
                    {
                        var declaration = dictionaryDeclaration[declarationValue.DeclarationID];

                        declaration.DeclarationValues.Add(declarationValue);
                    }
                }
            });
        }
    }
}
