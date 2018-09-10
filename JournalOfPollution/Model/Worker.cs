using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Работник</summary>
    public class Worker : MyTools.C_A_BaseFromAllDB
    {
        public Worker(uint ID, bool CanEdit = true) : base(G.Worker, ID, CanEdit)
        { }
        string Name => T.Worker.Rows.Get<string>(ID, C.Worker.Name);
        string Patronymic => T.Worker.Rows.Get<string>(ID, C.Worker.Patronymic);
        /// <summary>ФИО сокращённое</summary>
        public string FIO => $"{T.Worker.Rows.Get<string>(ID, C.Worker.SureName)} {(Name.Length > 0 ? $"{Name[0]}." : string.Empty)}{(Patronymic.Length > 0 ? $"{Patronymic[0]}." : string.Empty)}";
        /// <summary>ФИО инициалы впереди сокращённое</summary>
        public string rFIO => $"{(Name.Length > 0 ? $"{Name[0]}." : string.Empty)}{(Patronymic.Length > 0 ? $"{Patronymic[0]}." : string.Empty)} {T.Worker.Rows.Get<string>(ID, C.Worker.SureName)}";
        /// <summary>Должность</summary>
        public string Post => T.Worker.Rows.Get<string>(ID, C.Worker.Post, C.Post.FullName);
        public string Post_FIO => $"{Post} {FIO}";
    }
}
