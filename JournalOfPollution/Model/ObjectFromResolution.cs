using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Отношение объекта к постановлению</summary>
    public class ObjectFromResolution : MyTools.C_A_BaseFromAllDB
    {
        public ObjectFromResolution(uint ID, bool CanEdit = true) : base(G.ObjectFromResolution, ID, CanEdit)
        { }

        public Objecte Objecte
        { get; private set; }

        /// <summary>Применение</summary>
        public bool Application => T.ObjectFromResolution.Rows.Get<bool>(ID, C.ObjectFromResolution.Application);
        /// <summary>Причина</summary>
        public string Reason => T.ObjectFromResolution.Rows.Get<string>(ID, C.ObjectFromResolution.Reason);
        /// <summary>ID Объекта</summary>
        public uint ObjectID => T.ObjectFromResolution.Rows.Get_UnShow<uint>(ID, C.ObjectFromResolution.Object);
        /// <summary>ID постановления</summary>
        public uint ResolutionID => T.ObjectFromResolution.Rows.Get_UnShow<uint>(ID, C.ObjectFromResolution.Resolution);

        public bool Add(Objecte objecte)
        {
            if (objecte.ID == this.ObjectID)
            {
                this.Objecte = objecte;
                return true;
            }
            else
            { return false; }
        }
    }
}
