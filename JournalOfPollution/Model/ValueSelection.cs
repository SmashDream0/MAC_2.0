using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Значение в отборе</summary>
    public class ValueSelection : MyTools.C_A_BaseFromAllDB
    {
        public ValueSelection(uint ID, bool CanEdit = true)
            : base(G.ValueSelection, ID, CanEdit)
        { }
        /// <summary>Значение</summary>
        public decimal Value
        {
            get { return T.ValueSelection.Rows.Get<decimal>(ID, C.ValueSelection.Value); }
            set { SetOneValue(C.ValueSelection.Value, value); }
        }
        /// <summary>Значение с урезанными знаками</summary>
        public decimal ValueRound => Math.Round(Value, Pollution.Round);
        /// <summary>Меясц отбора</summary>
        public int YMSample => T.ValueSelection.Rows.Get<int>(ID, C.ValueSelection.SelectionWell, C.SelectionWell.Sample, C.Sample.YM);
        /// <summary>ID отбора</summary>
        public uint SampleID => T.ValueSelection.Rows.Get_UnShow<uint>(ID, C.ValueSelection.SelectionWell, C.SelectionWell.Sample);
        /// <summary>ID выбранного колодеца</summary>
        public uint SelectionWellID => T.ValueSelection.Rows.Get_UnShow<uint>(ID, C.ValueSelection.SelectionWell);

        public uint PollutionID => T.ValueSelection.Rows.Get_UnShow<uint>(ID, C.ValueSelection.Pollution);

        /// <summary>Номер</summary>
        public int NumberSel => T.ValueSelection.Rows.Get<int>(ID, C.ValueSelection.SelectionWell, C.SelectionWell.Number);

        /// <summary>выбранный колодец</summary>        
        public SelectionWell SelectionWell
        { get; private set; }
        /// <summary>загрязнение</summary>
        public Pollution Pollution
        { get; private set; }

        public bool Add(Pollution pollution)
        {
            if (this.PollutionID == pollution.ID)
            {
                this.Pollution = pollution;

                return true;
            }
            else
            { return false; }
        }

        public bool Add(SelectionWell selectionWell)
        {
            if (this.SelectionWellID == selectionWell.ID)
            {
                this.SelectionWell = selectionWell;

                return true;
            }
            else
            { return false; }
        }

        public override string ToString()
        {
            return $"{Pollution} = {ValueRound}";
        }
    }
}
