using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Коэффициент</summary>
    public class Coefficient : MyTools.C_A_BaseFromAllDB
    {
        public Coefficient(uint ID, bool LoadAll = false, bool CanEdit = true) : base(G.Coefficient, ID, CanEdit)
        { }
        /// <summary>Месяц действия с</summary>
        public int YMFrom => T.Coefficient.Rows.Get<int>(ID, C.Coefficient.YMFrom);
        /// <summary>Месяц действия до</summary>
        public int YMTo => T.Coefficient.Rows.Get<int>(ID, C.Coefficient.YMTo);
        /// <summary>ID постановления</summary>
        public uint ResolutionID => T.Coefficient.Rows.Get_UnShow<uint>(ID, C.Coefficient.Resolution);
        /// <summary>ID Загрязнения</summary>
        public uint PollutionID => T.Coefficient.Rows.Get_UnShow<uint>(ID, C.Coefficient.Pollution);
        /// <summary>Для сравнения</summary>
        public bool Compare => T.Coefficient.Rows.Get<bool>(ID, C.Coefficient.Compare);

        private Dictionary<uint, CoefficientValue> _coefficientValues = new Dictionary<uint, Model.CoefficientValue>();

        public IEnumerable<CoefficientValue> CoefficientValues => _coefficientValues.Values;

        public bool Add(CoefficientValue coefficientValue)
        {
            if (coefficientValue.CoefficientID == this.ID)
            {
                if (_coefficientValues.ContainsKey(coefficientValue.ID))
                { _coefficientValues[coefficientValue.ID] = coefficientValue; }
                else
                { _coefficientValues.Add(coefficientValue.ID, coefficientValue); }

                coefficientValue.Add(this);
                return true;
            }
            else
            { return false; }
        }
    }
}
