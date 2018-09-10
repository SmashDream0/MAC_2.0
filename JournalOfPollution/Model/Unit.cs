using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Подразделение</summary>
    public class Unit : MyTools.C_A_BaseFromAllDB
    {
        public Unit(uint ID) : base(G.Unit, ID, false)
        { }
        /// <summary>Наименование</summary>
        public string Name => T.Unit.Rows.Get<string>(ID, C.Unit.Name);
        /// <summary>ID Тип сооружения</summary>
        public uint TypeConstructionID => T.Unit.Rows.Get_UnShow<uint>(ID, C.Unit.TypeConstruction);
    }
}
