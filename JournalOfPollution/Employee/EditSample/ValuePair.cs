using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.EditSample
{
    struct ValuePair
    {
        public decimal Value { get; set; }
        public int Count { get; set; }

        public decimal MiddleValue
        {
            get
            {
                if (Count > 0)
                { return Value / Count; }
                else
                { return 0; }
            }
        }
    }
}
