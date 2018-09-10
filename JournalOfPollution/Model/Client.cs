using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using System.Windows;
using MAC_2.Employee.Mechanisms;

namespace MAC_2.Model
{
    /// <summary>Клиент</summary>
    public class Client : MyTools.C_A_BaseFromAllDB
    {
        /// <param name="LoadAll">Прогрузить все внутренности</param>
        public Client(uint ID, bool CanEdit = true) : base(G.Client, ID, CanEdit)
        { }

        public string Name => (Detail == null ? String.Empty : Detail.FullName);
        public readonly bool LoadAll;
        /// <summary>ИНН</summary>
        public string INN => T.Client.Rows.Get<string>(ID, C.Client.INN);
        /// <summary>Можно отбирать</summary>
        public bool CanSelect => T.Client.Rows.Get<bool>(ID, C.Client.CanSelect);
        /// <summary>Обязан подать декларацию</summary>
        public bool MustDeclair => T.Client.Rows.Get<bool>(ID, C.Client.MustDeclair);
        /// <summary>Дата создания в месяцах</summary>
        public int YMFrom => T.Client.Rows.Get<int>(ID, C.Client.YMFrom);
        /// <summary>Дата закрытия в месяцах</summary>
        public int YMTo => T.Client.Rows.Get<int>(ID, C.Client.YMTo);
        /// <summary>Тип клиента</summary>
        public TypeClient TypeClient
        {
            get
            {
                if (T.Client.Rows.Get_UnShow<uint>(ID, C.Client.TypeClient) == 0)
                {
                    if (MessageBox.Show("Данный клиент не имеет типизации.\nХотите задать тип?", "Типизация", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        MyTools.C_A_TypeColumn[] columns = new MyTools.C_A_TypeColumn[]
                        {
                            new  MyTools.C_DefColumn(C.Client.INN,false),
                            new  MyTools.C_DefColumn(C.Client.TypeClient)
                        };
                        FreeControl_Window FR = new FreeControl_Window(G.Client, new MyTools.C_A_BaseFromAllDB[] { Helpers.LogicHelper.ClientsLogic.FirstModel(ID) }, new MyTools.C_MinMaxWidthHeight(), columns);
                        FR.GridTop.SetFromGrid(Detail.GetEditor(new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Horisontal), new MyTools.C_MinMaxWidthHeight(), new MyTools.C_DefColumn(C.DetailsClient.FullName, false)));
                        FR.ShowDialog();
                    }
                    else
                    { throw new Exception("Без типа нельзя получить нужное значение."); }
                }
                return new TypeClient(T.Client.Rows.Get_UnShow<uint>(ID, C.Client.TypeClient));
            }
        }

        private Dictionary<uint, Objecte> _objects = new Dictionary<uint, Objecte>();

        public bool Add(Objecte objecte)
        {
            if (objecte.ClientID == this.ID)
            {
                if (_objects.ContainsKey(objecte.ID))
                { _objects[objecte.ID] = objecte; }
                else
                { _objects.Add(objecte.ID, objecte); }

                objecte.Add(this);

                return true;
            }
            else
            { return false; }
        }

        public bool Add(DetailsClient detailsClient)
        {
            if (detailsClient.ClientID == this.ID)
            {
                Detail = detailsClient;

                detailsClient.Add(this);

                return true;
            }
            else
            { return false; }
        }

        public IEnumerable<Objecte> Objects => _objects.Values;
        /// <summary>Получить реквизиты</summary>
        public DetailsClient Detail
        { get; private set; }

        public IEnumerable<DetailsClient> GetDetailsAll()
        {
            return Helpers.LogicHelper.DetailsClientLogic.Find(this.ID);
        }

        public Objecte ObjAtWell(uint ID)
        { return Objects.First(x => x.Wells.FirstOrDefault(y => y.ID == ID) != null); }
    }
}
