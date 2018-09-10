using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{/// <summary>Акредитация</summary>
    public class Accredit : MyTools.C_A_BaseFromAllDB
    {
        public Accredit(uint ID, bool CanEdit = true) : base(G.Accredit, ID, CanEdit)
        { }
        /// <summary>Текст</summary>
        public string Text => T.Accredit.Rows.Get<string>(ID, C.Accredit.Text);
        /// <summary>Дата от в днях</summary>
        public string YMDFrom => MyTools.YearMonthDay_From_YMD(T.Accredit.Rows.Get<int>(ID, C.Accredit.YMDFrom), MyTools.EDateTimeTypes.BeautifulWords);
    }
}
