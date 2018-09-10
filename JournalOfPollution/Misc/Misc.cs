
using AutoTable;
using MAC_2.EmployeeWindow;
using MAC_2.EmployeeWindow.Admin;
using System;
using System.Windows;

namespace MAC_2
{
    public static partial class Misc
    {
        public const int Number = 0;

        delegate void AddCols_delegate(DataBase.ITable Table);

        /// <summary>Добавить кеширующую таблицу</summary>
        static bool AddSynch(StartupLogo_Window.Loading_class Loading, string Name, string AlterName, ref DataBase.ITable Table, ref DataBase.ISTable SubTable, AddCols_delegate AddCols, bool Dedicate)
        {
            if (Loading != null)
            { Loading.LoadingComment = Name; }

            if (Table != null)
            { throw new Exception("Таблица уже существует!"); }

            Table = data.T1.Tables.Add(Name, AlterName);
            AddCols(Table);
            Table.AutoSave(Dedicate, DataBase.TypeOfTable.Combine);

            if (Table.Parent.DataSourceEnabled)
            {
                SubTable = Table.CreateSubTable();
                SubTable.QUERRY().SHOW.DO();
                return true;
            }
            else
            {
                if (Loading != null)
                { Loading.LoadingComment = "Ошибка"; }
                return false;
            }
        }

        /// <summary>Добавить не кеширующую таблицу</summary>
        static bool AddRemote(StartupLogo_Window.Loading_class Loading, string Name, string AlterName, ref DataBase.ITable Table, ref DataBase.ISTable SubTable, AddCols_delegate AddCols, bool Dedicate)
        {
            if (Loading != null)
            { Loading.LoadingComment = Name; }

            if (Table != null)
            { throw new Exception("Таблица уже существует!"); }

            Table = data.T1.Tables.Add(Name, AlterName);
            AddCols(Table);
            Table.AutoSave(Dedicate, DataBase.TypeOfTable.Remote);

            if (data.T1.type == DataBase.RemoteType.Local || Table.Parent.DataSourceEnabled)
            {
                SubTable = Table.CreateSubTable();
                return true;
            }
            else
            {
                if (Loading != null)
                { Loading.LoadingComment = "Ошибка"; }
                return false;
            }
        }

        public static Window SelectForm()
        {
            switch ((data.UType)data.User<uint>(C.User.UType))
            {
                case data.UType.Admin: //админ
                    return new AdminPanel();
                case data.UType.MainWork:
                    return new Employee_Window();
                default:
                    throw new Exception("Не извесный тип пользователя");
            }
        }
    }
}