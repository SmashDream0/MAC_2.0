using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Тип клиента</summary>
    public class TypeClient
    {
        public TypeClient(uint ID)
        { this.ID = ID; }
        public readonly uint ID;
        /// <summary>Наименование</summary>
        public string Name => T.TypeClient.Rows.Get<string>(ID, C.TypeClient.Name);
        /// <summary>В падежах</summary>
        public MyTools.TextAsCase[] InCase => MyTools.DivideCases(T.TypeClient.Rows.Get<string>(ID, C.TypeClient.InCase));
        /// <summary>Тип клиента</summary>
        public data.ETypeClient typeClient => (data.ETypeClient)ID;
    }
}
