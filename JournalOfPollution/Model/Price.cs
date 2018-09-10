using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Прайс</summary>
    public class Price : MyTools.C_A_BaseFromAllDB
    {
        public Price(uint ID) : base(G.Period, ID, false)
        { }
        public decimal _Price
        {
            get { return T.Period.Rows.Get<decimal>(ID, C.Period.Price); }
            set { SetOneValue(C.Period.Price, value); }
        }
        public decimal MinLimits
        {
            get { return T.Period.Rows.Get<decimal>(ID, C.Period.MinLimits); }
            set { SetOneValue(C.Period.MinLimits, value); }
        }
        public decimal NDS
        {
            get { return T.Period.Rows.Get<decimal>(ID, C.Period.NDS); }
            set { SetOneValue(C.Period.NDS, value); }
        }
        public int YM
        {
            get { return T.Period.Rows.Get<int>(ID, C.Period.YM); }
            set { SetOneValue(C.Period.YM, value); }
        }
    }
}
