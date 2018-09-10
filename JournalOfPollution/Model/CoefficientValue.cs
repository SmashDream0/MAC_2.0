using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>значения коэффициента</summary>
    public class CoefficientValue : MyTools.C_A_BaseFromAllDB
    {
        public CoefficientValue(uint ID, bool CanEdit = true) : base(G.CoefficientValue, ID, CanEdit)
        { }
        /// <summary>от</summary>
        public decimal From => T.CoefficientValue.Rows.Get<decimal>(ID, C.CoefficientValue.From);
        /// <summary>до</summary>
        public decimal To => T.CoefficientValue.Rows.Get<decimal>(ID, C.CoefficientValue.To);
        /// <summary>Коэффициент</summary>
        public decimal Value => T.CoefficientValue.Rows.Get<decimal>(ID, C.CoefficientValue.Value);
        /// <summary>ID Коэффициент</summary>
        public uint CoefficientID => T.CoefficientValue.Rows.Get_UnShow<uint>(ID, C.CoefficientValue.Coefficient);

        public Coefficient Coefficient
        { get; private set; }

        public bool Add(Coefficient coefficient)
        {
            if (coefficient.ID == this.CoefficientID)
            {
                Coefficient = coefficient;
                return true;
            }
            else
            { return false; }
        }
    }
}
