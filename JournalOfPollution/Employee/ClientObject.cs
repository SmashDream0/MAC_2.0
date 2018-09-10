using AutoTable;
using System.Collections.Generic;

namespace MAC_2.EmployeeWindow
{
    public partial class Employee_Window
    {
        public class ClientObject :MyTools.C_A_BaseFromAllDB
        {
            public ClientObject(uint IDClient, uint ObjectID = 0):base (G.Client,IDClient,false)
            {
                this.ObjectID = ObjectID;
                Values = new Dictionary<string, object>();
                Values.Add(column.INN, INN);
                Values.Add(column.Name, null);
                if (ObjectID > 0)
                {
                    Values.Add(column.Adres, T.Objecte.Rows.Get<string>(ObjectID, C.Objecte.AdresFact, C.AdresReference.Adres).StringDivision(20));
                    Values.Add(column.NumberFolder, NumberFolder);
                }
                Values.Add(column.DateClose, close);
            }
            public readonly uint ObjectID;
            public int NumberFolder => T.Objecte.Rows.Get<int>(ObjectID, C.Objecte.NumberFolder);

            public string close => _close>0?$"Закрыто от {MyTools.YearMonth_From_YM(_close)}":"";
            int _close => closeC > closeO ? closeO : closeC;

            private int closeO => T.Objecte.Rows.Get<int>(ObjectID, C.Objecte.YMTo);
            private int closeC => T.Client.Rows.Get<int>(ID, C.Client.YMTo);
            private string INN => T.Client.Rows.Get<string>(ID, C.Client.INN);

            uint _IDDetailClient;
            public uint IDDetailClient
            {
                get { return _IDDetailClient; }
                set
                {
                    _IDDetailClient = value;
                    Values[column.Name] = T.DetailsClient.Rows.Get<string>(_IDDetailClient, C.DetailsClient.FullName).StringDivision(20);
                }
            }
        }
    }
}