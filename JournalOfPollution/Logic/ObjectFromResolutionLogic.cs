using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class ObjectFromResolutionLogic
        : BaseLogicTyped<ObjectFromResolution>
    {
        public ObjectFromResolutionLogic() : base(T.ObjectFromResolution)
        { }

        protected override ObjectFromResolution internalGetModel(uint id)
        { return new ObjectFromResolution(id); }

        public IEnumerable<ObjectFromResolution> Find(int ym, bool? canSelect = null)
        {
            return getQuerryResult($"int ym={ym}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = table.QUERRY()
                    .SHOW
                   .WHERE
                       .OB()
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.YMTo).More.BV(ym - 1)
                       .OR
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.YMTo).EQUI.BV(0)
                       .CB()
                   .AND
                       .OB()
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.YMTo).More.BV(ym - 1)
                       .OR
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.YMTo).EQUI.BV(0)
                       .CB();

                if (canSelect.HasValue)
                { query.AND.ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            });
        }

        public IEnumerable<ObjectFromResolution> Find(int ym, uint objectID, bool? canSelect)
        {
            return getQuerryResult($"int ym={ym}|objectID={objectID}|canSelect={(canSelect.HasValue ? canSelect.Value.ToString(): "null")}", (table) =>
            {
                var query = table.QUERRY()
                    .SHOW
                   .WHERE
                       .OB()
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.YMTo).More.BV(ym - 1)
                       .OR
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.YMTo).EQUI.BV(0)
                       .CB()
                   .AND
                       .OB()
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.YMTo).More.BV(ym - 1)
                       .OR
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.YMTo).EQUI.BV(0)
                       .CB()
                    .AND
                        .AC(C.ObjectFromResolution.Object).EQUI.BV(objectID);

                if (canSelect.HasValue)
                { query.AND.ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.CanSelect).EQUI.BV(canSelect.Value); }

                query.DO();
            });
        }

        public IEnumerable<ObjectFromResolution> Find(int ym, uint clientID)
        {
            return getQuerryResult($"int ym={ym}|clientID={clientID}", (table) =>
            {
                var query = table.QUERRY()
                    .SHOW
                   .WHERE
                       .OB()
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.YMTo).More.BV(ym - 1)
                       .OR
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.Client, C.Client.YMTo).EQUI.BV(0)
                       .CB()
                   .AND
                       .OB()
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.YMTo).More.BV(ym - 1)
                       .OR
                           .ARC(C.ObjectFromResolution.Object, C.Objecte.YMTo).EQUI.BV(0)
                       .CB()
                    .AND
                        .ARC(C.ObjectFromResolution.Object, C.Objecte.Client).EQUI.BV(clientID);

                query.DO();
            });
        }

        public IEnumerable<ObjectFromResolution> Find(uint objectID)
        {
            return getQuerryResult($"int objectID={objectID}", (table) =>
            {
                var query = G.ObjectFromResolution.QUERRY()
                      .SHOW
                      .WHERE
                      .C(C.ObjectFromResolution.Object, objectID);

                query.DO();
            });
        }
    }
}
