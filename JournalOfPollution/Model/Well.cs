using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Колодец</summary>
    public class Well : MyTools.C_A_BaseFromAllDB
    {
        public Well(uint ID, bool CanEdit = true) : base(G.Well, ID, CanEdit)
        { }

        public Objecte Objecte
        { get; private set; }

        /// <summary>Номер колодца в представлении </summary>
        public string PresentNumber => T.Well.Rows.Get<string>(ID, C.Well.TypeWell, C.TypeWell.CurtName) + (Number > 0 ? "-" + Number : string.Empty);
        public string FullName => T.Well.Rows.Get<string>(ID, C.Well.TypeWell, C.TypeWell.FullName);

        /// <summary>Номер колодца</summary>
        public int Number => T.Well.Rows.Get<int>(ID, C.Well.Number);
        /// <summary>Дата создания в месяцах</summary>
        public int YMFrom => T.Well.Rows.Get<int>(ID, C.Well.YMFrom);
        /// <summary>Дата закрытия в месяцах</summary>
        public int YMTo => T.Well.Rows.Get<int>(ID, C.Well.YMTo);
        /// <summary>ID тип колодца</summary>
        public uint TypeWellID => T.Well.Rows.Get_UnShow<uint>(ID, C.Well.TypeWell);
        /// <summary>ID подразделения</summary>
        public uint UnitID => T.Well.Rows.Get_UnShow<uint>(ID, C.Well.Unit);

        public uint ObjectID => T.Well.Rows.Get_UnShow<uint>(ID, C.Well.Object);

        public bool Add(Declaration declaration)
        {
            if (declaration.WellID == this.ID)
            {
                Declaration = declaration;
                declaration.Add(this);

                return true;
            }
            else
            { return false; }
        }

        public bool Add(Objecte objecte)
        {
            if (objecte.ID == ObjectID)
            {
                Objecte = objecte;

                return true;
            }
            else
            { return false; }
        }

        public Declaration Declaration
        { get; private set; }
    }
}
