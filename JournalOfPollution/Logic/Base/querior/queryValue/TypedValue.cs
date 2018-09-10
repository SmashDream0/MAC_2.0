using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Logic.Base.querior.queryValue
{
    class TypedValue<T> : baseValue
    {
        public TypedValue(T value)
        { this._value = value; }

        private T _value;
        public override object Value => _value;

        internal override object MakeQuery(object queryObject)
        {
            return ((DataBase.IShortBOperand)queryObject).BV<T>(_value);
        }
    }
}
