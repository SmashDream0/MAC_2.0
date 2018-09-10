using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Среднегодовой объём</summary>
    public class MidMonthVolume : MyTools.C_A_BaseFromAllDB
    {
        public MidMonthVolume(uint ID, bool CanEdit = true) : base(G.MidMonthVolume, ID, CanEdit)
        { }
        /// <summary>Объём</summary>
        public double Volume => T.MidMonthVolume.Rows.Get<double>(ID, C.MidMonthVolume.Volume);
        /// <summary>Год</summary>
        public int Year => T.MidMonthVolume.Rows.Get<int>(ID, C.MidMonthVolume.Year);
    }
}
