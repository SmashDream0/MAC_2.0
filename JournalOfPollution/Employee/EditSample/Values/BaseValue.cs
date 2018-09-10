using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.EditSample
{
    abstract class BaseValue : MAC_2.Model.IValue
    {
        public BaseValue(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
        public abstract decimal? Value
        { get; set; }

        public override string ToString()
        {
            return $"{Name}: {(Value.HasValue ? Value.ToString() : "null")}";
        }
    }
}
