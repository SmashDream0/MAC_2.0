using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Адрес</summary>
    public class Adres : MyTools.C_A_BaseFromAllDB
    {
        public Adres(uint ID, bool CanEdit = true) : base(G.Unit, ID, CanEdit)
        { }
        /// <summary>Адрес</summary>
        public string Adr => T.AdresReference.Rows.Get<string>(ID, C.AdresReference.Adres);
    }
}
