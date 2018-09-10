using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Загрязнения</summary>
    public class Pollution : MyTools.C_A_BaseFromAllDB
    {
        public Pollution(uint ID, bool CanEdit = true) : base(G.Pollution, ID, CanEdit)
        { }

        /// <summary>
        /// Индекс
        /// </summary>
        public int Index
        { get; set; }

        /// <summary>Краткое наименование</summary>
        public string CurtName
        {
            get { return T.Pollution.Rows.Get<string>(ID, C.Pollution.CurtName); }
            set { SetOneValue(C.Pollution.CurtName, value); }
        }
        /// <summary>Полное наименование</summary>
        public string FullName
        {
            get { return T.Pollution.Rows.Get<string>(ID, C.Pollution.FullName); }
            set { SetOneValue(C.Pollution.FullName, value); }
        }
        /// <summary>Диапозонное значение</summary>
        public bool HasRange
        {
            get { return T.Pollution.Rows.Get<bool>(ID, C.Pollution.HasRange); }
            set { SetOneValue(C.Pollution.FullName, value); }
        }
        /// <summary>Код</summary>
        public int Key
        {
            get { return T.Pollution.Rows.Get<int>(ID, C.Pollution.Key); }
            set { SetOneValue(C.Pollution.FullName, value); }
        }
        /// <summary>Номер по порядку</summary>
        public int Number
        {
            get { return T.Pollution.Rows.Get<int>(ID, C.Pollution.Number); }
            set { SetOneValue(C.Pollution.Number, value); }
        }
        /// <summary>Округление</summary>
        public int Round
        {
            get
            {
                int round = T.Pollution.Rows.Get<int>(ID, C.Pollution.Round);
                return round;
            }
            set { SetOneValue(C.Pollution.Round, value); }
        }
        /// <summary>ID единицы измерения</summary>
        public uint UnitsID
        {
            get { return T.Pollution.Rows.Get_UnShow<uint>(ID, C.Pollution.Units); }
            set { SetOneValue(C.Pollution.Units, value); }
        }
        /// <summary>Месяц с</summary>
        public int YMFrom
        {
            get { return T.Pollution.Rows.Get<int>(ID, C.Pollution.YMFrom); }
            set { SetOneValue(C.Pollution.YMFrom, value); }
        }
        /// <summary>Месяц до</summary>
        public int YMTo
        {
            get { return T.Pollution.Rows.Get<int>(ID, C.Pollution.YMTo); }
            set { SetOneValue(C.Pollution.YMTo, value); }
        }
        /// <summary>Binding имя</summary>
        public string BindName { get; set; }
        /// <summary>Методика</summary>
        public string Method => T.Pollution.Rows.Get<string>(ID, C.Pollution.Method);
        /// <summary>Показывать в основном окне</summary>
        public bool Show
        {
            get { return T.Pollution.Rows.Get<bool>(ID, C.Pollution.Show); }
            set { SetOneValue(C.Pollution.Show, value); }
        }
        /// <summary>Уникальный номер</summary>
        public string UniqueKey => T.Pollution.Rows.Get<string>(ID, C.Pollution.UniqueKey);

        public IEnumerable<AccurateMeasurement> Accurates => _accurates.Values;

        private Dictionary<uint, AccurateMeasurement> _accurates = new Dictionary<uint, AccurateMeasurement>();

        public bool Add(AccurateMeasurement accurateMeasurement)
        {
            if (accurateMeasurement.PollutionID == this.ID)
            {
                if (_accurates.ContainsKey(accurateMeasurement.ID))
                { _accurates[accurateMeasurement.ID] = accurateMeasurement; }
                else
                { _accurates.Add(accurateMeasurement.ID, accurateMeasurement); }

                return true;
            }
            else
            { return false; }
        }

        public override string ToString()
        {
            return CurtName;
        }
    }
}
