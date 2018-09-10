using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Тип колодца</summary>
    public class TypeWell
    {
        public TypeWell(uint ID)
        { this.ID = ID; }
        uint ID;
        /// <summary>Полное наименование</summary>
        public string FullName => T.TypeWell.Rows.Get<string>(ID, C.TypeWell.FullName);
        /// <summary>Краткое наименование</summary>
        public string CurtName => T.TypeWell.Rows.Get<string>(ID, C.TypeWell.CurtName);
    }
}
