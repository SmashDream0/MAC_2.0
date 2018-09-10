using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using System.Windows;
using System.Data;
using MAC_2.Model;
using MAC_2.Logic;
using MAC_2.Helpers;

namespace MAC_2.Employee.Mechanisms
{
    public static class AdditionnTable
    {
        #region Период
        /// <summary>Отсортирован по возрастанию!</summary>
        public static List<Period> ListPeriod;

        public static void LoadPeriod()
        {
            ListPeriod = LogicHelper.PeiodLogic.Find().ToList();
        }
        /// <summary>Получить запись период по тукущему выбранному периоду!</summary>
        public static Period GetPeriod => ListPeriod.Last(x => x.YM < DateControl_Class.SelectMonth);

        #endregion

        #region Единицы измерений

        /// <summary>Единицы измерений</summary>
        public static Units[] AllUnits;

        /// <summary>Грузить все подразделения</summary>
        public static void LoadUnits()
        {
            if (AllUnits == null)
            { AllUnits = LogicHelper.UnitsLogic.Find().ToArray(); }
        }

        #endregion

        #region басейны сброса/подразделения

        /// <summary>Подразделения</summary>
        public static Unit[] AllUnit;

        /// <summary>Грузить все подразделения</summary>
        public static void LoadUnit()
        {
            if (AllUnit == null)
            { AllUnit = LogicHelper.UnitLogic.Find().ToArray(); }
        }

        #endregion

        #region отношение объекта к постановлениям
        
        /// <summary>Загрузка отношений по постановлениям к объекту</summary>
        public static ObjectFromResolution[] LoadOFRAtObjecte(uint IDObj)
        {
            return LogicHelper.ObjectFromResolutionLogic.Find(IDObj).ToArray();
        }

        #endregion

        /// <summary>Получить актуальную акредитацию</summary>
        public static Accredit GetAccredit()
        {
            long ymd = MyTools.GetNowDate(MyTools.EInputDate.YMD);

            var accreds = LogicHelper.AccreditLogic.Find(ymd);

            /*
             * Вот это ветвление мутит нам в протоколы аккредитацию, прикол в чем? А в том, что он явно не указывает в View_Class какую имеено аккредитацию брать
             * по этому это исключение вылетает в одном из случаев - Когда в базе нет вообще аккредитаций. И когда там более двух аккредитаций. Понять, как он
             * определяет что берет вторую аккредитацию, не смог пока что, кода много. 
             * 
             */
            if (!accreds.Any())
            { throw new Exception("Не найдёно ни одной акредитаций действующей на данный период!"); }
            else if (accreds.Count() > 1)
            { throw new Exception("Найдёно несколько акредитаций имеющих одинаковый период действия!"); }
            else
            { return accreds.First(); }
            //___________________________________________________________________________________________________________________________________________________//
        }

        /// <summary>Получить подписывающего соглассно таблице подписантов</summary>
        public static Worker GetSigner(data.ETypeTemplate type, string list)
        {
            if (list == null)
            { list = ""; }

            var ratioSigner = LogicHelper.RatioSignerLogic.Find(DateControl_Class.SelectMonth, type, list);

            if (!ratioSigner.Any())
            { throw new Exception($"Не выбран подписывающий для данного документа! {G.TypeTemplate.Rows.Get<string>((uint)type, C.TypeTemplate.Name)}"); }
            else if (ratioSigner.Count() > 1)
            { throw new Exception($"Подписывающих не может быть больше одного! {G.TypeTemplate.Rows.Get<string>((uint)type, C.TypeTemplate.Name)}"); }
            return ratioSigner.First().Worker;
        }

        /// <summary>Получить работников согласно таблице подписантов</summary>
        public static Worker[] GetWorkers(data.ETypeTemplate type)
        {
            //var workers = LogicHelper.RatioSignerLogic.Find(DateControl_Class.SelectMonth, type);

            //return workers.Select(x => x.Worker).ToArray();

            return LogicHelper.WorkerLogic.Find().ToArray();
        }

        /// <summary>Список работников</summary>
        public static Worker[] GetWorkers(data.ETypeTemplate type, bool Positions, string list = null)
        {
            var ratios = LogicHelper.RatioSignerLogic.Find(DateControl_Class.SelectMonth, type, list);

            var workers = ratios.Select(x => new Worker(x.WorkerID)).ToArray();

            return workers;
        }

        /// <summary>Получить таблицу работников соглассно таблице подписантов</summary>
        public static DataTable GetWorkerDT(data.ETypeTemplate type)
        {
            DataTable result = new DataTable();
            result.Columns.Add("ID");
            result.Columns.Add("ФИО");
            result.Columns.Add("Должность");
            var worker = GetWorkers(type);
            foreach (var one in worker)
            { result.Rows.Add(one.ID, one.FIO, one.Post); }
            return result;
        }
    }
}