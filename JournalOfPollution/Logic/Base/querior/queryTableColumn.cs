using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Logic.Base.querior.queryValue;

namespace MAC_2.Logic.Base.querior
{
    class queryTableColumn : baseQuery
    {
        public queryTableColumn(int columnIndex, params int[] columnIndexes)
        {
            this._columnIndex = columnIndex;
            this._columnIndexes = columnIndexes;
        }

        private int _columnIndex;
        private int[] _columnIndexes;

        /// <summary>
        /// Задать значения условия
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="not">Отрицание</param>
        /// <param name="condition">Условие</param>
        /// <param name="value">Значение</param>
        public void SetCondition<T>(bool not, queryTable.ECondition condition, T value)
        {
            this.Not = not;
            this.Condition = condition;
            this._value = new TypedValue<T>(value);
        }

        private baseValue _value;

        public bool Not { get; set; }
        public queryTable.ECondition Condition { get; set; }
        public object Value => (_value == null ? null : _value.Value);

        internal override object MakeQuery(object queryObject)
        {
            if (Not)
            { queryObject = ((DataBase.IOAOperand)queryObject).NOT; }

            if (_columnIndexes.Length == 0)
            { queryObject = ((DataBase.IAOperations)queryObject).AC(_columnIndex); }
            else
            { queryObject = ((DataBase.IAOperations)queryObject).ARC(_columnIndex, _columnIndexes); }

            return _value.MakeQuery(queryObject);
        }

        internal object MakeQuery(object queryObject, int columnIndex, int[] columnIndexes)
        {
            if (Not)
            { queryObject = ((DataBase.IOAOperand)queryObject).NOT; }

            int[] resultColumnIndex;

            {
                var listColumnindex = new List<int>();

                listColumnindex.AddRange(columnIndexes);
                listColumnindex.Add(_columnIndex);
                listColumnindex.AddRange(_columnIndexes);

                resultColumnIndex = listColumnindex.ToArray();
            }

            queryObject = ((DataBase.IAOperations)queryObject).ARC(columnIndex, resultColumnIndex);

            return _value.MakeQuery(queryObject);
        }

        public override string ToString()
        {
            return $"{(this.Not ? "Not" : String.Empty)} Condition = {Condition} Value = {(Value ?? "null")}";
        }
    }
}
