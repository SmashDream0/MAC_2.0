using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Интерфейс значений</summary>
    public interface IValue
    { decimal? Value { get; set; } }

    /// <summary>Значение</summary>
    public class ValueWork : IValue
    {
        public ValueWork(ValueSelection valueSelection)
        { this._valueSelection = valueSelection; }

        public ValueWork(Pollution pollution, SelectionWell selectionWell)
        {
            this._pollution = pollution;
            this._selectionWell = selectionWell;
        }

        private ValueSelection _valueSelection;
        private Pollution _pollution;
        private SelectionWell _selectionWell;
        
        /// <summary>Непосредственно значение</summary>
        public decimal? Value
        {
            get
            {
                if (_valueSelection != null)
                {
                    return _valueSelection.ValueRound;
                }
                return null;
            }
            set
            {
                if (_valueSelection != null)
                { _valueSelection.Value = value.Value; }
                else
                {
                    var ID = MyTools.AddRowFromTable(G.ValueSelection,
                          new KeyValuePair<int, object>(C.ValueSelection.Value, value.Value),
                          new KeyValuePair<int, object>(C.ValueSelection.Pollution, this._pollution.ID),
                          new KeyValuePair<int, object>(C.ValueSelection.SelectionWell, this._selectionWell.ID)
                          );

                    _valueSelection = Helpers.LogicHelper.ValuesSelectionLogic.FirstModel(ID);

                    _valueSelection.Add(_pollution);
                    _valueSelection.Add(_selectionWell);

                    _selectionWell.Add(_valueSelection);
                }
            }
        }
    }
}
