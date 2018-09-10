using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Еденицы измерения</summary>
    public class Units : MyTools.C_A_BaseFromAllDB
    {
        public Units(uint ID) : base(G.Units, ID, false)
        { }
        /// <summary>Наименование</summary>
        public string Name => T.Units.Rows.Get<string>(ID, C.Units.Name);
    }
}
