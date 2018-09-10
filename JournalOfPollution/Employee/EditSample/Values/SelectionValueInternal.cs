using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.EditSample
{
    internal class SelectionValueInternal
        : BaseValue
    {
        public SelectionValueInternal(MAC_2.Model.ValueSelection valueSelection)
            : base(valueSelection.Pollution.BindName)
        { this._valueSelection = valueSelection; }
        public SelectionValueInternal(MAC_2.Model.SelectionWell selectionWell, MAC_2.Model.Pollution pollution)
            : base(pollution.BindName)
        {
            this._selectionWell = selectionWell;
            this._pollution = pollution;
        }

        MAC_2.Model.SelectionWell _selectionWell = null;
        MAC_2.Model.Pollution _pollution = null;
        MAC_2.Model.ValueSelection _valueSelection = null;

        public override decimal? Value
        {
            get
            {
                if (_valueSelection == null)
                { return null; }
                else
                { return _valueSelection.ValueRound; }
            }
            set { setValue(value); }
        }

        private void setValue(decimal? value)
        {
            if (_valueSelection == null)
            {
                if (value.HasValue)
                {
                    var id = (uint)G.ValueSelection.QUERRY()
                        .ADD
                            .C(C.ValueSelection.Pollution, _pollution.ID)
                            .C(C.ValueSelection.SelectionWell, _selectionWell.ID)
                            .C(C.ValueSelection.Value, value.Value)
                        .DO()[0].Value;

                    _valueSelection = Helpers.LogicHelper.ValuesSelectionLogic.FirstModel(id);
                }
            }
            else
            {
                _valueSelection.Value = value.Value;
            }
        }
    }
}