using AutoTable;
using MAC_2.Employee.Mechanisms;
using MAC_2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MAC_2.Calc
{
    public abstract class BaseCalc_Class
    {
        #region Конструкторы

        public BaseCalc_Class(Sample sample, Objecte objecte, Resolution Norm)
        {
            this._sample = sample;
            this.Objecte = objecte;
            this.Norm = Norm;

            init();
        }

        public BaseCalc_Class(ValueSelection[] Values, Objecte _Object, Resolution Norm)
        {
            this._values = Values;
            this.Objecte = _Object;
            this.Norm = Norm;

            init();
        }

        private void init()
        {
            _resolutionClarify = Norm.ListResolutionClarify
                          .First(y =>
                          (y.TypeResolution == data.ETypeResolution.Norm || y.TypeResolution == data.ETypeResolution.CostNorm)
                          && y.YMFrom <= DateControl_Class.SelectMonth
                          && (y.YMTo >= DateControl_Class.SelectMonth || y.YMTo == 0));
        }

        #endregion

        #region Элементы

        /// <summary>Массив элементов</summary>
        public IEnumerable<clValue> Value;
        protected Sample _sample;
        protected ValueSelection[] _values;
        public readonly Objecte Objecte;
        protected CalculationFormula _calcFormula;
        public readonly Resolution Norm;
        private ResolutionClarify _resolutionClarify;

        #endregion

        #region Месные КЛАССЫ

        public class Summs
        {
            //public Summs(decimal SummVal,uint Pollution, Price price)
            //{
            //    this.Summ = new Dictionary<uint, decimal>();
            //    Summ.Add(Pollution, Math.Round(SummVal *  price._Price, 2, MidpointRounding.AwayFromZero));
            //    this._Price = price;
            //}
            public Summs(List<C_KeyPol_Summ> Summ, Period price, bool MultPrice = false)
            {
                this.Summ = Summ.Select(x => new C_KeyPol_Summ(x.PollutionID, Math.Round((x.Summ * (MultPrice ? price.Price : 1)), 2, MidpointRounding.AwayFromZero),x.IsTarif)).ToList();
                this._Price = price;
            }
            /// <summary>Прайс</summary>
            public Period _Price;
            /// <summary>Сумма без НДС</summary>
            public List<C_KeyPol_Summ> Summ;
            /// <summary>Сумма с НДС</summary>
            public decimal SummNDS =>
                Math.Round(Summ.Where(x => !x.IsTarif).Sum(x => x.Summ) * (1 + _Price.NDS / 100), 2, MidpointRounding.AwayFromZero) +
                Math.Round(Summ.Where(x => x.IsTarif).Sum(x => x.Summ) * (1 + _Price.NDS / 100), 2, MidpointRounding.AwayFromZero);
        }

        public class clValue
        {
            public clValue(decimal value, Well well, Pollution pollution,
                ValueNorm valueNorm = null,
                CalculationFormula calculationFormula = null,
                CoefficientValue coefficientValue = null,
                PriceNorm priceNorm = null)
            {
                this.Value = Math.Round(value, pollution.Round, MidpointRounding.AwayFromZero);
                this.Pollution = pollution;
                this.Well = well;
                this.ValueNorm = valueNorm;
                this.CoefficientValue = coefficientValue;
                this.CalculationFormula = calculationFormula;
                this.PriceNorm = priceNorm;
            }
            public readonly decimal Value;
            public readonly Pollution Pollution;
            public readonly Well Well;
            public readonly ValueNorm ValueNorm;
            public readonly CoefficientValue CoefficientValue;
            public readonly CalculationFormula CalculationFormula;
            public readonly PriceNorm PriceNorm;
        }

        #endregion

        #region Механизмы для предков

        /// <summary>Расчёт</summary>
        /// <returns>Сохраняется в результат</returns>
        public virtual KeyValuePair<uint, Summs>[] Calc()
        { return OUT; }

        /// <summary>Сохранённые посчитанные значения</summary>
        protected KeyValuePair<uint, Summs>[] OUT;

        /// <summary>Рузультат</summary>
        public KeyValuePair<uint, Summs>[] Answer
        {
            get
            {
                if (OUT == null)
                { Calc(); }
                return OUT;
            }
        }

        /// <summary>
        /// Общая сумма с НДС
        /// </summary>
        public decimal TotalSumNDS
        {
            get
            {
                if (Answer == null)
                { return 0; }
                else
                { return Answer.Sum(x => x.Value.SummNDS); }
            }
        }

        #endregion

        #region Механизмы

        protected void LoadValues()
        {
            _values = _sample.ValueSelection.Where(x => x.Value > 0).ToArray();
        }

        /// <summary>Получить значение</summary>
        /// <param name="GettingValue">Получаемое значение</param>
        protected virtual decimal GettingValue(data.EGettingValue GettingValue, decimal[] Values)
        {
            switch (GettingValue)
            {
                case data.EGettingValue.Max:
                    { return Values.Max(); }
                case data.EGettingValue.Medium:
                    { return Values.Sum() / Values.Length; }
                case data.EGettingValue.Summ:
                    { return Values.Sum(); }
            }
            throw new Exception("Не выбран метод определения значения");
        }

        /// <summary>Группирование значений</summary>
        protected IEnumerable<clValue> Compl(IEnumerable<clValue> Values, data.EGettingValue GettingVal)
        {
            List<clValue> result = new List<clValue>();
            foreach (var one in Values.GroupBy(x => x.Pollution))
            {
                result.Add(new clValue( Math.Round(GettingValue(GettingVal, one.Select(x => x.Value).ToArray()), one.First().Pollution.Round,MidpointRounding.AwayFromZero), one.First().Well, one.Key,
                    one.First().ValueNorm,
                    one.First().CalculationFormula,
                    one.First().CoefficientValue,
                    one.First().PriceNorm));
            }
            return result.ToArray();
        }

        /// <summary>Сравнение с декларациями</summary>
        protected IEnumerable<clValue> CompareWithDeclaration(IEnumerable<ValueSelection> values)
        {
            List<clValue> result = new List<clValue>();
            foreach (var value in values)
            {
                var well = value.SelectionWell.Well;
                var declaration = well.Declaration;
                    //.Declarations.FirstOrDefault(x => x.YM <= DateControl_Class.SelectMonth && (x.YM - 1) / 12 == (DateControl_Class.SelectMonth - 1) / 12);
                var declarationValue = declaration == null ? null : declaration.DeclarationValues.FirstOrDefault(x => x.PollutionID == value.Pollution.ID);

                if (value.Pollution.HasRange)
                {
                    if (declarationValue != null && declarationValue.From > value.Value && declarationValue.From / value.Value < 1.5m)
                    { result.Add(new clValue(declarationValue.From, well, value.Pollution)); }
                    else if (declarationValue != null && declarationValue.To < value.Value && value.Value / declarationValue.To < 1.5m)
                    { result.Add(new clValue(declarationValue.To, well, value.Pollution)); }
                    else
                    { result.Add(new clValue(value.Value, well, value.Pollution)); }
                }
                else
                {
                    if (value.Value > 0)
                    {
                        if (declarationValue == null)
                        { result.Add(new clValue(value.Value, well, value.Pollution)); }
                        else if (value.Value / declarationValue.To >= 1.5m || declarationValue.To / value.Value >= 1.5m)
                        { result.Add(new clValue(value.Value, well, value.Pollution)); }
                        else
                        { result.Add(new clValue(declarationValue.To, well, value.Pollution)); }
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>Получение значения нормамы</summary>
        protected IEnumerable<clValue> CompareWithNorm(IEnumerable<clValue> values)
        {
            List<clValue> result = new List<clValue>();
            foreach (var value in values)
            {                
                ValueNorm Vnorm = this._resolutionClarify.ValueNorms.FirstOrDefault
                    (x => x.PollutionID == value.Pollution.ID
                    && (x.Unit != null ? x.Unit.ID == value.Well.UnitID : true)
                    );

                if (Vnorm == null || Vnorm.From == 0 && Vnorm.To == 0)
                { continue; }

                if (value.Pollution.HasRange
                    && value.Value < Vnorm.From)
                { result.Add(new clValue(value.Value, value.Well, value.Pollution, Vnorm)); }

                if (value.Value > Vnorm.To)
                { result.Add(new clValue(value.Value, value.Well, value.Pollution, Vnorm)); }
            }
            return result.ToArray();
        }

        /// <summary>Получение стоимости нормамы</summary>
        protected IEnumerable<clValue> CompareWithPriceNorm(IEnumerable<clValue> Values)
        {
            List<clValue> result = new List<clValue>();
            foreach (var one in Values)
            {
                PriceNorm PNorm = PollutionBase_Class.AllPriceNorm.FirstOrDefault
                    (x =>
                    x.ResolutionClarify.ID == _resolutionClarify.ID
                    && x.Pollution.ID == one.Pollution.ID
                    );
                if (PNorm == null)
                { result.Add(one); }
                else
                { result.Add(new clValue(one.Value, one.Well, one.Pollution, one.ValueNorm, one.CalculationFormula, one.CoefficientValue, PNorm)); }
            }
            return result.ToArray();
        }

        /// <summary>Сбор значений == значения собираются в одно значение "ФК"</summary>
        protected IEnumerable<clValue> Compilation(IEnumerable<clValue> Values)
        {
            List<clValue> result = new List<clValue>();
            var group = Values.GroupBy(x => x.Pollution.ID);

            foreach (var one in group)
            {
                CalculationFormula CalcFormula = PollutionBase_Class.AllCalculationFormul.FirstOrDefault
                    (x =>
                    x.ResolutionClarifyID == one.First().ValueNorm.ResolutionClarifyID
                    && x.PollutionID == one.First().Pollution.ID
                    && x.YM <= DateControl_Class.SelectMonth
                    );

                if (CalcFormula != null)
                {
                    decimal val = Math.Round(GettingValue(CalcFormula.GettingValueFormula, one.Select(x => x.Value).ToArray()), one.First().Pollution.Round, MidpointRounding.AwayFromZero);
                    result.Add(new clValue(val,
                        one.First().Well, one.First().Pollution, one.First().ValueNorm, CalcFormula));
                }
            }
            return result.ToArray();
        }

        /// <summary>Сбор значений == значения собираются в одно значение "ФК"</summary>
        protected IEnumerable<clValue> CompilationFormuls(IEnumerable<clValue> Values)
        {
            List<clValue> result = new List<clValue>();
            var group = Values.GroupBy(x => x.CalculationFormula.Number);
            foreach (var one in group)
            {
                decimal val = Math.Round( GettingValue(one.First().CalculationFormula.GettingValueLink, one.Select(x => x.Value).ToArray()), one.First().Pollution.Round,MidpointRounding.AwayFromZero);
                result.Add(new clValue(val,
                    one.First().Well, one.First().Pollution, one.First().ValueNorm, one.First().CalculationFormula));
            }
            return result.ToArray();
        }

        /// <summary>Пририсовка коэффициентов</summary>
        protected IEnumerable<clValue> CompareWithCoefficient(IEnumerable<clValue> Values)
        {
            List<clValue> result = new List<clValue>();
            foreach (var value in Values)
            {
                Coefficient coeff = PollutionBase_Class.AllCoefficient.FirstOrDefault
                    (x =>
                    x.PollutionID == value.Pollution.ID
                    && x.ResolutionID == value.ValueNorm.ResolutionClarify.ID
                    && x.YMFrom <= DateControl_Class.SelectMonth && (x.YMTo >= DateControl_Class.SelectMonth || x.YMTo == 0)
                    );
                if (coeff == null)
                { result.Add(value); }
                else if (coeff.Compare)
                {
                    foreach (var coef in coeff.CoefficientValues)
                    {
                        if (value.Value >= coef.From
                            && value.Value <= coef.To)
                        {
                            result.Add(new clValue(value.Value, value.Well, value.Pollution, value.ValueNorm, value.CalculationFormula, coef));
                            break;
                        }
                    }
                }
                else
                {
                    if (coeff.CoefficientValues.Count() > 1)
                    { throw new Exception("Значений коэффициента ID:" + coeff.ID + " не может быть больше одного на период времени"); }

                    result.Add(new clValue(value.Value, value.Well, value.Pollution, value.ValueNorm, value.CalculationFormula, coeff.CoefficientValues.First()));
                }
            }
            return result.ToArray();
        }

        #region ФОРМУЛЫ

        /// <summary>Выполнение формул БЕЗ объёмов</summary>
        protected List<C_KeyPol_Summ> RunFormulaNotVolume(IEnumerable<clValue> values)
        {
            List<C_KeyPol_Summ> result = new List<C_KeyPol_Summ>();
            foreach (var value in values)
            {
                var valueResult = RunFormula(value);
                result.Add(new C_KeyPol_Summ(valueResult.Key, valueResult.Value, value.PriceNorm == null || value.PriceNorm.Price == 0));
            }
            return result;
        }

        /// <summary>Выполнение формул С объёмом</summary>
        public Dictionary<uint, List<C_KeyPol_Summ>> RunFormulaVolume(IEnumerable<clValue> values)
        {
            if (_sample == null)
            {
                MessageBox.Show("Не выбран отбор!");
                return null;
            }

            Dictionary<uint, List<C_KeyPol_Summ>> result = new Dictionary<uint, List<C_KeyPol_Summ>>();

            foreach (var value in values)
            {
                string formulaText = value.CalculationFormula.Formula;

                var formula = Helpers.CalcHelper.GetFormula(formulaText);

                if (SetConst(formula, value))
                {/// <summary>Объём</summary>
                    foreach (var volume in _sample.Volumes)
                    {
                        if (value.PriceNorm == null || value.PriceNorm.Price == 0)
                        { formula.TrySetValue(Const_Class.Tariff, Helpers.LogicHelper.PeiodLogic.FirstModel(volume.PeriodID).Price); }

                        formula.TrySetValue(Const_Class.Volume, (decimal)volume.Value);

                        if (!result.ContainsKey(volume.ID))
                        { result.Add(volume.ID, new List<C_KeyPol_Summ>()); }

                        if (formula.CanCalculate)
                        {
                            result[volume.ID].Add(new C_KeyPol_Summ(value.Pollution.ID, Math.Round(GetRoundedFormulaSumm(formula), 2, MidpointRounding.AwayFromZero), value.PriceNorm == null || value.PriceNorm.Price == 0));
                        }
                    }
                }
            }

            return result;
        }

        static decimal GetRoundedFormulaSumm(Formulator.Formula formula)
        {
            return Math.Round(formula.Summ, 7);
        }

        /// <summary>Выполнение формул С объёмом</summary>
        public KeyValuePair<uint, decimal> RunFormula(clValue value, uint volumeID = 0)
        {
            decimal result = 0;
            string formulaText = value.CalculationFormula.Formula;

            var formula = Helpers.CalcHelper.GetFormula(formulaText);

            if (SetConst(formula, value))
            {
                if (volumeID > 0)
                {
                    Volume volume = _sample.Volumes.First(x => x.ID == volumeID);

                    var period = Helpers.LogicHelper.PeiodLogic.FirstModel(volume.PeriodID);

                    if (value.PriceNorm == null)
                    { formula.TrySetValue(Const_Class.Tariff, period.Price); }

                    formula.TrySetValue(Const_Class.Volume, (decimal)volume.Value);

                    result += GetRoundedFormulaSumm(formula);
                }
                else if (Norm.CurtName.Contains("621"))
                {
                    foreach (var volume in _sample.Volumes)
                    {
                        var period = Helpers.LogicHelper.PeiodLogic.FirstModel(volume.PeriodID);

                        if (value.PriceNorm == null)
                        { formula.TrySetValue(Const_Class.Tariff, period.Price); }

                        formula.TrySetValue(Const_Class.Volume, (decimal)volume.Value);

                        result += Math_Class.CalculateStroku(formulaText, 7);
                    }
                }
                else
                { result = GetRoundedFormulaSumm(formula); }
            }

            return new KeyValuePair<uint, decimal>(value.Pollution.ID, result);
        }

        /// <summary>Получить значение без учёта тарифов!</summary>
        public decimal FromulaNotTrKf(clValue value)
        {
            if (_sample == null)
            {
                MessageBox.Show("Не выбран отбор!");
                return 0;
            }
            decimal result = 0;
            string formulaText = value.CalculationFormula.Formula;

            var formula = Helpers.CalcHelper.GetFormula(formulaText);

            SetConst(formula, value);

            /// <summary>Коэффициент</summary>            
            formula.TrySetValue(Const_Class.Coeff, 1);
            /// <summary>Тариф загрязнения</summary>            
            formula.TrySetValue(Const_Class.TariffPollution, 1);
            formula.TrySetValue(Const_Class.Tariff, 1);

            foreach (var vol in _sample.Volumes)
            {
                formula.TrySetValue(Const_Class.Volume, (decimal)vol.Value);
                result += GetRoundedFormulaSumm(formula);
            }
            return result;
        }

        #endregion

        /// <summary>Заполнение констант</summary>
        public static bool SetConst(Formulator.Formula formula, clValue value)
        {
            /// <summary>Фактическая концентрация</summary>
            formula.TrySetValue(Const_Class.FaktDensities, value.Value);
            /// <summary>Допустимая концентрация(НОРМА)</summary>
            if (formula.GetVarriable(Const_Class.PermissibleDensities) != null)
            {
                if (value.ValueNorm != null && value.ValueNorm.To > 0)
                { formula.TrySetValue(Const_Class.PermissibleDensities, value.ValueNorm.To); }
                else
                { return false; }
            }

            /// <summary>Коэффициент</summary>
            if (formula.GetVarriable(Const_Class.Coeff) != null)
            {
                if (value.CoefficientValue != null && value.CoefficientValue.Value > 0)
                { formula.TrySetValue(Const_Class.Coeff, value.CoefficientValue.Value); }
                else
                { return false; }
            }

            /// <summary>Тариф загрязнения</summary>
            if (formula.GetVarriable(Const_Class.TariffPollution) != null)
            {
                if (value.PriceNorm != null && value.PriceNorm.Price > 0)
                { formula.TrySetValue(Const_Class.TariffPollution, value.PriceNorm.Price); }
                else
                { return false; }
            }

            return true;
        }

        /// <summary>Заполнение констант</summary>
        public static void SetConst(ref string formula, clValue value)
        {
            /// <summary>Фактическая концентрация</summary>
            formula = formula.Replace(Const_Class.FaktDensities, value.Value.ToString());
            /// <summary>Допустимая концентрация(НОРМА)</summary>
            if (value.ValueNorm != null)
            { formula = formula.Replace(Const_Class.PermissibleDensities, value.ValueNorm.To.ToString()); }
            /// <summary>Коэффициент</summary>
            if (value.CoefficientValue != null)
            { formula = formula.Replace(Const_Class.Coeff, value.CoefficientValue.Value.ToString()); }
            /// <summary>Тариф загрязнения</summary>
            if (value.PriceNorm != null)
            { formula = formula.Replace(Const_Class.TariffPollution, value.PriceNorm.Price.ToString()); }
        }

        #endregion
    }
}