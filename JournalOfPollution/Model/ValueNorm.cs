using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Значения норматива</summary>
    public class ValueNorm : MyTools.C_A_BaseFromAllDB
    {
        public ValueNorm(uint ID, bool CanEdit = true) : base(G.ValueNorm, ID, CanEdit)
        {
            _resolution = new NavigationProperty.NavigationPropertryTyped<Resolution>(this.ID);
        }
        /// <summary>ID Загрязнения</summary>
        public uint PollutionID => T.ValueNorm.Rows.Get_UnShow<uint>(ID, C.ValueNorm.Pollution);
        /// <summary>ID Актуализации постановления</summary>
        public uint ResolutionClarifyID => T.ValueNorm.Rows.Get_UnShow<uint>(ID, C.ValueNorm.ResolutionClarify);
        /// <summary>ID постановления</summary>
        public uint ResolutionID => T.ValueNorm.Rows.Get_UnShow<uint>(ID, C.ValueNorm.ResolutionClarify, C.ResolutionClarify.Resolution);
        /// <summary>от</summary>
        public decimal FromRound => Math.Round(T.ValueNorm.Rows.Get<decimal>(ID, C.ValueNorm.From), Pollution.Round);
        /// <summary>от</summary>
        public decimal From
        {
            get => T.ValueNorm.Rows.Get<decimal>(ID, C.ValueNorm.From);
            set => T.ValueNorm.Rows.Set(ID, C.ValueNorm.From, value);
        }
        /// <summary>до</summary>
        public decimal To
        {
            get => T.ValueNorm.Rows.Get<decimal>(ID, C.ValueNorm.To);
            set => T.ValueNorm.Rows.Set(ID, C.ValueNorm.To, value);
        }
        /// <summary>Множитель</summary>
        public decimal Multiplier
        {
            get => T.ValueNorm.Rows.Get<decimal>(ID, C.ValueNorm.Multiplier);
            set => T.ValueNorm.Rows.Set(ID, C.ValueNorm.Multiplier, value);
        }
        /// <summary>до</summary>
        public decimal ToRound => Math.Round(T.ValueNorm.Rows.Get<decimal>(ID, C.ValueNorm.To), Pollution.Round);
        /// <summary>Подразделение</summary>
        public uint UnitID => T.ValueNorm.Rows.Get_UnShow<uint>(ID, C.ValueNorm.Unit);

        /// <summary>Период действия от</summary>
        public int YMFrom
        {
            get => T.ValueNorm.Rows.Get_UnShow<int>(ID, C.ValueNorm.YMFrom);
            set => T.ValueNorm.Rows.Set(ID, C.ValueNorm.YMFrom, value);
        }
        /// <summary>Период действия до</summary>
        public int YMTo
        {
            get => T.ValueNorm.Rows.Get_UnShow<int>(ID, C.ValueNorm.YMTo);
            set => T.ValueNorm.Rows.Set(ID, C.ValueNorm.YMTo, value);
        }

        private NavigationProperty.NavigationPropertryTyped<Resolution> _resolution;

        public Resolution Resolution => _resolution.Model;

        public Unit Unit
        { get; private set; }
        /// <summary>Загрязнение</summary>
        public Pollution Pollution
        { get; private set; }
        /// <summary>Актуализация постановления</summary>
        public ResolutionClarify ResolutionClarify
        { get; private set; }

        public bool Add(Resolution resolution)
        { return _resolution.Add(resolution); }

        public bool Add(Unit unit)
        {
            if (this.UnitID == unit.ID)
            {
                Unit = unit;
                return true;
            }
            else
            { return false; }
        }

        public bool Add(Pollution pollution)
        {
            if (this.PollutionID == pollution.ID)
            {
                Pollution = pollution;
                return true;
            }
            else
            { return false; }
        }

        public bool Add(ResolutionClarify resolutionClarify)
        {
            if (this.ResolutionClarifyID == resolutionClarify.ID)
            {
                ResolutionClarify = resolutionClarify;
                return true;
            }
            else
            { return false; }
        }
    }
}
