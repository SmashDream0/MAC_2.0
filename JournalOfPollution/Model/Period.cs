using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Период</summary>
    public class Period : MyTools.C_A_BaseFromAllDB
    {
        public Period(uint ID):base(G.Period, ID, true)
        { }
        /// <summary>Прайс</summary>
        public decimal Price => T.Period.Rows.Get<decimal>(ID, C.Period.Price);
        /// <summary>Месяц</summary>
        public int YM => T.Period.Rows.Get<int>(ID, C.Period.YM);
        /// <summary>НДС</summary>
        public decimal NDS => T.Period.Rows.Get<decimal>(ID, C.Period.NDS);
        /// <summary>Минимальный лимит</summary>
        public decimal MinLimits => T.Period.Rows.Get<decimal>(ID, C.Period.MinLimits);

        public override string ToString()
        {
            return $"{ATMisc.GetDateTimeFromYM(YM).ToString("yyyy.MM")}, НДС={NDS}, тариф={Price}";
        }
    }
}
