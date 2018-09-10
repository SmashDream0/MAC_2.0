using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.Windows.Norm
{
    public abstract class NormBaseValue
    {
        private const string _decimalToStringFormat = "0.#############";

        protected string gecimalToString(decimal value)
        { return value.ToString(_decimalToStringFormat); }

        public abstract string Value
        {
            get;
            set;
        }
    }
}
