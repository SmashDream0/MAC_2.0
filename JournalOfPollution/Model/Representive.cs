using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Представитель</summary>
    public class Representative : MyTools.C_A_BaseFromAllDB
    {
        public Representative(uint ID, bool CanEdit = true) : base(G.Representative, ID, CanEdit)
        { }
        /// <summary>ФИО сокращённое</summary>
        public string FIO => T.Representative.Rows.Get<string>(ID, C.Representative.FIO);
        /// <summary>Должность</summary>
        public string Post => T.Representative.Rows.Get<string>(ID, C.Representative.Post);
        public string Post_FIO => $"{Post} {FIO}";
    }
}
