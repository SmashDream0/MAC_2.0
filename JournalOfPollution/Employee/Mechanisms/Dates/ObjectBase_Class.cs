using AutoTable;
using MAC_2.EmployeeWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MAC_2.Model;
using MAC_2.Logic;
using MAC_2.Helpers;

namespace MAC_2.Employee.Mechanisms
{
    /// <summary>Все клиенты</summary>
    public static class AllClients
    {
        static AllClients()
        {
            Clients = new List<Client>();

        }
        #region Клиенты и его функции
        public static List<Client> Clients { get; internal set; }

        /// <summary>Получить клиента по id колодца</summary>
        public static Client ClientAtWell(uint WellID)
        {
            //LoadClients(new uint[] { WellID });
            var result = Clients.FirstOrDefault(x => x.Objects.FirstOrDefault(y => y.Wells.FirstOrDefault(i => i.ID == WellID) != null) != null);
            if (result == null)
            {
                //MessageBox.Show("Что-то тут не чисто!");
                Clients.Add(Helpers.LogicHelper.ClientsLogic.FirstModel(T.Well.Rows.Get_UnShow<uint>(WellID, C.Well.Object, C.Objecte.Client)));
                result = Clients.Last();
            }
            return result;
        }
        /// <summary>Грузить клиентов</summary>        
        public static void LoadClients()
        {
            int ym = DateControl_Class.SelectMonth;
            
            Clients = LogicHelper.ClientsLogic.Find(ym).ToList();
        }

        #endregion
    }
}