using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Объём</summary>
    public class Volume : MyTools.C_A_BaseFromAllDB
    {
        public Volume(uint ID) : base(G.Volume, ID, false)
        {
            this._period = new NavigationProperty.NavigationPropertryTyped<Period>(this.PeriodID);
        }
        /// <summary>Объём</summary>
        public double Value
        {
            get { return T.Volume.Rows.Get<double>(ID, C.Volume.Value); }
            set { SetOneValue(C.Volume.Value, value); }
        }
        /// <summary>ID прайса</summary>
        public uint PeriodID
        {
            get { return T.Volume.Rows.Get_UnShow<uint>(ID, C.Volume.Period); }
            set { SetOneValue(C.Volume.Period, value); }
        }
        /// <summary>ID пробы</summary>
        public uint SampleID
        {
            get { return T.Volume.Rows.Get_UnShow<uint>(ID, C.Volume.Sample); }
            set { SetOneValue(C.Volume.Sample, value); }
        }

        private Model.NavigationProperty.NavigationPropertryTyped<Period> _period;

        public Period Period => _period.Model;

        public bool Add(Period period)
        {
            return this._period.Add(period);
        }
    }
}
