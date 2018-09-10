using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Employee.Mechanisms;
using MAC_2.Model;

namespace MAC_2.Calc
{
    public class Calc_621 : BaseCalc_Class
    {
        public Calc_621(Sample sample, Objecte objecte, Resolution norm) : base(sample, objecte, norm)
        { }
        public override KeyValuePair<uint, Summs>[] Calc()
        {
            LoadValues();
            Value = _values.Select(x => new clValue(x.Value, x.SelectionWell.Well, x.Pollution)).ToArray();
            Value = Compl(Value, data.EGettingValue.Medium);
            Value = CompareWithNorm(Value);
            Value = Compilation(Value);
            Value = CompareWithCoefficient(Value);
            Value = CompareWithPriceNorm(Value);

            var summs = new List<KeyValuePair<uint, Summs>>();
            var koeff = RunFormulaVolume(Value);

            if (koeff.Count > 0)
            {
                foreach (var one in _sample.Volumes)
                {
                    //Summs.Add(new KeyValuePair<uint, Summs>(one.ID, new Summs(koeff.First(x => x.Key == one.ID).Value, PollutionBase_Class.AllPrice.First(x => x.ID == one.PeriodID), false)));
                    //var t = new Summs(koeff.First(x => x.Key == one.ID).Value, PollutionBase_Class.AllPrice.First(x => x.ID == one.PeriodID));
                    summs.Add(new KeyValuePair<uint, Summs>(one.ID, new Summs(koeff.First(x => x.Key == one.ID).Value.Where(x => x.Summ > 0).ToList(), PollutionBase_Class.AllPeriod.First(x => x.ID == one.PeriodID))));
                }
            }

            OUT = summs.ToArray();
            return base.Calc();
        }
    }
}