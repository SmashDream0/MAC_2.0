using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Реквизиты клиента</summary>}
    public class DetailsClient : MyTools.C_A_BaseFromAllDB
    {
        public DetailsClient(uint ID, bool CanEdit = true) : base(G.DetailsClient, ID, CanEdit)
        { }

        public Client Client
        { get; private set; }

        public bool Add(Client client)
        {
            if (ClientID == client.ID)
            {
                Client = client;
                return true;
            }
            else
            { return false; }
        }

        /// <summary>ID почтового адреса</summary>
        public uint AdresPostID => T.DetailsClient.Rows.Get_UnShow<uint>(ID, C.DetailsClient.AdresP);
        /// <summary>ID юридического адреса</summary>
        public uint AdresLegalID => T.DetailsClient.Rows.Get_UnShow<uint>(ID, C.DetailsClient.AdresUr);
        /// <summary>Полное наименование</summary>
        public string FullName => T.DetailsClient.Rows.Get<string>(ID, C.DetailsClient.FullName);
        /// <summary>Дата действия с в месяцах</summary>
        public int YM => T.DetailsClient.Rows.Get<int>(ID, C.DetailsClient.YM);
        /// <summary>Наименование в дательном падеже</summary>
        public string NameInDative => T.DetailsClient.Rows.Get<string>(ID, C.DetailsClient.NameInDative);
        public uint ClientID => T.DetailsClient.Rows.Get_UnShow<uint>(ID, C.DetailsClient.Client);
    }
}
