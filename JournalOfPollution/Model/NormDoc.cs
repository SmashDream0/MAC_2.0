using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Нормативные документы</summary>
    public class NormDoc : MyTools.C_A_BaseFromAllDB
    {
        public NormDoc(uint ID, bool CanEdit = true) : base(G.NormDoc, ID, CanEdit)
        { }

        /// <summary>Акты</summary>
        public string Act => T.NormDoc.Rows.Get<string>(ID, C.NormDoc.Act);
        /// <summary>Счёт</summary>
        public string Score => T.NormDoc.Rows.Get<string>(ID, C.NormDoc.Score);
        /// <summary>Счёт фактура</summary>
        public string Invoces => T.NormDoc.Rows.Get<string>(ID, C.NormDoc.Invoces);
        /// <summary>Сумма</summary>
        public decimal Summ => T.NormDoc.Rows.Get<decimal>(ID, C.NormDoc.Summ);
        /// <summary>Дата от</summary>
        public int YMD => T.NormDoc.Rows.Get<int>(ID, C.NormDoc.Date);
        /// <summary>Дата от строкой</summary>
        public string sYMD => MyTools.YearMonthDay_From_YMD(YMD);

        public uint ResolutionID => T.NormDoc.Rows.Get_UnShow<uint>(ID, C.NormDoc.Resolution);

        public Resolution Resolution
        { get; private set; }

        public bool Add(Resolution resolution)
        {
            if (resolution.ID == this.ResolutionID)
            {
                Resolution = resolution;
                return true;
            }
            else
                return false;
        }

        /// <summary>В представлении текста</summary>
        public string Text => $"расчёт платы - в размере {Math.Round(Summ, 2)} руб., 1 экз. на 1 л." +
            $"\nсчёт - № {Score} от {sYMD}, 1 экз. на 1 л." +
            $"\nсчёт-фактура - № {Invoces} от {sYMD}, 1 экз. на 1 л." +
            $"\nакт - № {Act} от {sYMD}, 2 экз. на 1 л.";
    }
}
