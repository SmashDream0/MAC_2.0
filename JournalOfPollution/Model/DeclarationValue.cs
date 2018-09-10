using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Значения декларации</summary>
    public class DeclarationValue : MyTools.C_A_BaseFromAllDB
    {
        public DeclarationValue(uint ID, bool CanEdit = true) : base(G.DeclarationValue, ID, CanEdit)
        { }

        public string PollutionName => T.Pollution.Rows.Get<string>(PollutionID, C.Pollution.CurtName);
        /// <summary>ID загрязнение</summary>
        public uint PollutionID => T.DeclarationValue.Rows.Get_UnShow<uint>(ID, C.DeclarationValue.Pollution);
        /// <summary>От</summary>
        public decimal From => T.DeclarationValue.Rows.Get<decimal>(ID, C.DeclarationValue.From);
        /// <summary>От</summary>
        public decimal FromRound => Math.Round(T.DeclarationValue.Rows.Get<decimal>(ID, C.DeclarationValue.From), Pollution.Round);
        /// <summary>До</summary>
        public decimal To
        {
            get => T.DeclarationValue.Rows.Get<decimal>(ID, C.DeclarationValue.To);
            set => T.DeclarationValue.Rows.Set(ID, C.DeclarationValue.To, value);
        }
        /// <summary>До</summary>
        public decimal ToRound => Math.Round(T.DeclarationValue.Rows.Get<decimal>(ID, C.DeclarationValue.To), Pollution.Round);

        public uint DeclarationID => T.DeclarationValue.Rows.Get_UnShow<uint>(ID, C.DeclarationValue.Declaration);
        public uint WellID => T.DeclarationValue.Rows.Get_UnShow<uint>(ID, C.DeclarationValue.Declaration, C.Declaration.Well);

        public Declaration Declaration
        { get; private set; }
        public Pollution Pollution
        { get; private set; }

        public bool Add(Declaration declaration)
        {
            if (declaration.ID == DeclarationID)
            {
                this.Declaration = declaration;
                return true;
            }
            else
            { return false; }
        }

        public bool Add(Pollution polution)
        {
            if (polution.ID == PollutionID)
            {
                this.Pollution = polution;
                return true;
            }
            else
            { return false; }
        }

        public override string ToString()
        {
            return $"{PollutionName} = {FromRound}-{ToRound}";
        }
    }
}
