using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.EditSample
{
    class MiddleValue : BaseValue
    {
        public MiddleValue(string name)
            : base(name)
        { }

        public decimal Summ { get; private set; }
        public int Count { get; private set; }

        public void Add(decimal value)
        {
            Summ += value;
            Count++;
        }

        public override decimal? Value
        {
            get
            {
                if (Count > 0)
                { return Summ / Count; }
                else
                { return null; }
            }
            set { }
        }
    }
}