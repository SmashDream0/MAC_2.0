using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.EditSample
{
    class ValueCalc : BaseValue
    {
        public ValueCalc(string name, decimal Value) 
            : base(name)
        {
            this.Value = Value;
        }
        public ValueCalc(string name) 
            : base(name)
        {
            this.Value = null;
        }

        public override decimal? Value { get; set; }
    }
}
