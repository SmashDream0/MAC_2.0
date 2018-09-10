using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Расчётная формула</summary>
    public class CalculationFormula : MyTools.C_A_BaseFromAllDB
    {
        public CalculationFormula(uint ID, bool CanEdit = true) : base(G.CalculationFormula, ID, CanEdit)
        { }

        /// <summary>Месяц действия с</summary>
        public int YM => T.CalculationFormula.Rows.Get<int>(ID, C.CalculationFormula.YM);
        /// <summary>ID нормы</summary>
        public uint ResolutionClarifyID => T.CalculationFormula.Rows.Get_UnShow<uint>(ID, C.CalculationFormula.ResolutionClarify);
        /// <summary>ID Загрязнения</summary>
        public uint PollutionID => T.CalculationFormula.Rows.Get_UnShow<uint>(ID, C.CalculationFormula.Pollution);
        /// <summary>Получаемое значение для формулы</summary>
        public data.EGettingValue GettingValueFormula => (data.EGettingValue)T.CalculationFormula.Rows.Get_UnShow<uint>(ID, C.CalculationFormula.GettingValueFormula);
        /// <summary>Получаемое значение для связи</summary>
        public data.EGettingValue GettingValueLink => (data.EGettingValue)T.CalculationFormula.Rows.Get_UnShow<uint>(ID, C.CalculationFormula.GettingValueLink);
        /// <summary>Групировачный номер</summary>
        public int Number => T.CalculationFormula.Rows.Get<int>(ID, C.CalculationFormula.Number);
        /// <summary>Формула</summary>
        public string Formula => T.CalculationFormula.Rows.Get<string>(ID, C.CalculationFormula.Formula);
        /// <summary>Метка</summary>
        public string Label => T.CalculationFormula.Rows.Get<string>(ID, C.CalculationFormula.Label);

        public ResolutionClarify ResolutionClarify
        { get; private set; }

        public Pollution Pollution
        { get; private set; }

        public bool Add(Pollution pollution)
        {
            if (PollutionID == pollution.ID)
            {
                Pollution = pollution;
                return true;
            }
            else
            { return false; }
        }

        public bool Add(ResolutionClarify resolutionClarify)
        {
            if (PollutionID == resolutionClarify.ID)
            {
                ResolutionClarify = resolutionClarify;
                return true;
            }
            else
            { return false; }
        }
    }
}
