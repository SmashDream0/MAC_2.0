using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Calc
{
    public class Calc_644 : BaseCalc_Class
    {
        public Calc_644(Sample sample, Objecte objecte, Resolution norm) : base(sample, objecte, norm)
        { }
        public Calc_644(ValueSelection[] Values, Objecte objecte, Resolution norm) : base(Values, objecte, norm)
        { }
        /// <summary>Основной расчёт</summary>
        /// <returns>В иделе здесь выкатывается уже сумма</returns>
        /// <param name="NDS"></param>
        /// <returns>Пара объём и сыммы</returns>
        public override KeyValuePair<uint, Summs>[] Calc()
        {
            LoadValues();
            Value = CompareWithDeclaration(_values);
            Value = CompareWithNorm(Value);
            Value = Compilation(Value);
            Value = CompilationFormuls(Value);
            Value = CompareWithCoefficient(Value);

            var summs = new List<KeyValuePair<uint, Summs>>();
            var koeff = RunFormulaNotVolume(Value);

            if (koeff.Any())
            {
                foreach (var volume in _sample.Volumes)
                { summs.Add(new KeyValuePair<uint, Summs>(volume.ID, new Summs(koeff.Select(x => new C_KeyPol_Summ(x.PollutionID, x.Summ * (decimal)volume.Value, x.IsTarif)).ToList(), PollutionBase_Class.AllPeriod.First(x => x.ID == volume.PeriodID), true))); }
            }

            OUT = summs.ToArray();
            return base.Calc();
        }
        /// <summary>Выбранные значения по нормативу</summary>
        /// <param name="values">Если ноль вытащит значения из кармана</param>
        public IEnumerable<clValue> ValueFromResolution(IEnumerable<ValueSelection> values)
        {
            var result = CompareWithDeclaration(values == null || !values.Any() ? this._values : values);

            return CompareWithNorm(result);
        }
    }
}