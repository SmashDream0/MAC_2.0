using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MAC_2.Model;
using MAC_2.Logic;
using MAC_2.Helpers;

namespace MAC_2.Employee.Mechanisms
{
    /// <summary>Загрязнения и базовый набор функционала</summary>
    public static class PollutionBase_Class
    {
        static PollutionBase_Class()
        {
            ListSelectionWell = new SelectionWell[0];
        }
        
        /// <summary>выбранные колодцы</summary>
        public static SelectionWell[] ListSelectionWell { get; internal set; }

        /// <summary>загрязнения</summary>
        public static Pollution[] AllPolutions { get; internal set; }
        /// <summary>точность измерений</summary>
        public static AccurateMeasurement[] AccurateMeasurements { get; internal set; }
        /// <summary>постановления</summary>
        public static Resolution[] AllResolution { get; internal set; }
        /// <summary>все отборы</summary>
        public static Sample[] AllSample { get; internal set; }
        /// <summary>формулы</summary>
        public static CalculationFormula[] AllCalculationFormul { get; internal set; }
        /// <summary>значения нормы</summary>
        public static ValueNorm[] AllValueNorm { get; internal set; }
        /// <summary>стоимости нормы</summary>
        public static PriceNorm[] AllPriceNorm { get; internal set; }
        /// <summary>коэффициенты</summary>
        public static Coefficient[] AllCoefficient { get; internal set; }
        /// <summary>стоимость</summary>
        public static Period[] AllPeriod { get; internal set; }

        public static void LoadSelectedWells(int ym)
        {
            ListSelectionWell = LogicHelper.SelectionWellLogic.Find(ym, 1).ToArray();
        }
        /// <summary>Грузить все загрязнения, нормативы, прайс</summary>        
        public static void LoadAllPolutions()
        {
            AllPolutions = LogicHelper.PollutionLogic.Find().ToArray();
            AllResolution = LogicHelper.ResolutionLogic.Find(DateControl_Class.SelectMonth).ToArray();
            AllPeriod = LogicHelper.PeiodLogic.Find().OrderBy(x => x.YM).ToArray();
        }
        /// <summary>Грузить отборы</summary>
        public static void LoadSample()
        {
            AllSample = LogicHelper.SampleLogic.Find(DateControl_Class.SelectMonth, 1).ToArray();
        }

        /// <summary>Загрузить все возможные расчёты</summary>
        public static void LoadCalculationFormuls()
        {
            LoadValueNorms();
            LoadPriceNorm();
            LoadCoefficient();

            AllCalculationFormul = LogicHelper.CalculationFormulaLogic.Find().ToArray();
        }
        /// <summary>Загрузить значения нормы</summary>
        public static void LoadValueNorms()
        {
            AllValueNorm = LogicHelper.ValueNormLogic.Find(DateControl_Class.SelectMonth).ToArray();
        }
        /// <summary>Загрузить стоимость нормы</summary>
        public static void LoadPriceNorm()
        {
            AllPriceNorm = LogicHelper.PriceNormLogic.Find(DateControl_Class.SelectMonth).ToArray();
        }
        /// <summary>Загрузить коэффициенты</summary>
        public static void LoadCoefficient(bool CanEdit = false)
        {
            AllCoefficient = LogicHelper.CoefficientLogic.Find().ToArray();
        }
        /// <summary>Загрузить точность измерений</summary>
        public static void LoadAccurateMeasurement(bool CanEdit = false)
        {
            AccurateMeasurements = LogicHelper.AccurateMeasurementLogic.Find().ToArray();
        }

        public static ValueNorm GetValueNorm(uint resolutionID, uint pollutionID, uint unitID)
        {
            return LogicHelper.ValueNormLogic.Find(DateControl_Class.SelectMonth, resolutionID, pollutionID, unitID).FirstOrDefault();
        }

        /// <summary>Значения по указанному месяцу</summary>
        public static ValueSelection[] GetValuesFromYM(int ym)
        {
            return LogicHelper.ValuesSelectionLogic.Find(ym, (Nullable<uint>)1).ToArray();
        }

        /// <summary>Значения по указанному месяцу</summary>
        public static SelectionWell[] GetSelectionWellFromYM(int ym)
        {
            return LogicHelper.SelectionWellLogic.Find(ym, 1).ToArray();
        }
    }
}