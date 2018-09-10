using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Реквизиты объекта</summary>
    public class DetailsObject : MyTools.C_A_BaseFromAllDB
    {
        public DetailsObject(uint ID, bool CanEdit = true) : base(G.DetailsObject, ID, CanEdit)
        { }
        /// <summary>Действует с в месяцах</summary>
        public int YM => T.DetailsObject.Rows.Get<int>(ID, C.DetailsObject.YM);
        /// <summary>Юридический адрес</summary>
        public string LegalAdres => T.DetailsObject.Rows.Get<string>(ID, C.DetailsObject.LegalAdresReference, C.AdresReference.Adres);
        /// <summary>Почтовый адрес</summary>
        public string MailAdres => T.DetailsObject.Rows.Get<string>(ID, C.DetailsObject.MailAdresReference, C.AdresReference.Adres);
        /// <summary>Добавочное имя</summary>
        public string AddName => T.DetailsObject.Rows.Get<string>(ID, C.DetailsObject.AddName);

        public uint ObjectID => T.DetailsObject.Rows.Get_UnShow<uint>(ID, C.DetailsObject.Object);

        public Objecte Object
        { get; private set; }

        public bool Add(Objecte objecte)
        {
            if (objecte.ID == ObjectID)
            {
                Object = objecte;
                return true;
            }
            else
            { return false; }
        }
    }
}
