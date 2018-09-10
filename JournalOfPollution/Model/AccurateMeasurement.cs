using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;
using MAC_2.Model.NavigationProperty;

namespace MAC_2.Model
{
    public class AccurateMeasurement : MyTools.C_A_BaseFromAllDB
    {
        public AccurateMeasurement(uint ID, bool CanEdit = true) : base(G.AccurateMeasurement, ID, CanEdit)
        {
            this.Pollution = new NavigationProperty.NavigationPropertryTyped<Model.Pollution>(this.PollutionID);
        }

        /// <summary>Ключ показателя</summary>
        public uint PollutionID => G.AccurateMeasurement.Rows.Get_UnShow<uint>(ID, C.AccurateMeasurement.Pollution);
        /// <summary>От</summary>
        public decimal From => G.AccurateMeasurement.Rows.Get<decimal>(ID, C.AccurateMeasurement.From);
        /// <summary>До</summary>
        public decimal To => G.AccurateMeasurement.Rows.Get<decimal>(ID, C.AccurateMeasurement.To);
        /// <summary>Действует с</summary>
        public int YM => G.AccurateMeasurement.Rows.Get<int>(ID, C.AccurateMeasurement.YM);
        /// <summary>Расчёт в процентах</summary>
        public bool IsPercent => G.AccurateMeasurement.Rows.Get<bool>(ID, C.AccurateMeasurement.IsPercent);
        /// <summary>Значение</summary>
        public decimal Value => G.AccurateMeasurement.Rows.Get<decimal>(ID, C.AccurateMeasurement.Number);

        public NavigationPropertryTyped<Pollution> Pollution
        { get; private set; }
    }
}
