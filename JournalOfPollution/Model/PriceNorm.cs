using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Стоимость норматива</summary>
    public class PriceNorm : MyTools.C_A_BaseFromAllDB
    {
        public PriceNorm(uint ID, bool CanEdit = true) : base(G.PriceNorm, ID, CanEdit)
        { }
        /// <summary>ID Pollution</summary>
        public uint PollutionID => G.PriceNorm.Rows.Get_UnShow<uint>(ID, C.PriceNorm.Pollution);
        /// <summary>Загрязнение</summary>
        public Pollution Pollution
        { get; private set; }
        /// <summary>Актуализация постановления</summary>
        public ResolutionClarify ResolutionClarify
        { get; private set; }
        /// <summary>ID Актуализация постановления</summary>
        public uint ResolutionClarifyID => T.PriceNorm.Rows.Get_UnShow<uint>(ID, C.PriceNorm.ResolutionClarify);
        /// <summary>Стоимость</summary>
        public decimal Price
        {
            get => T.PriceNorm.Rows.Get<decimal>(ID, C.PriceNorm.Price);
            set => T.PriceNorm.Rows.Set(ID, C.PriceNorm.Price, value);
        }
        /// <summary>Множитель как коэффициент воздействия</summary>
        public double MultiAs
        {
            get => T.PriceNorm.Rows.Get<double>(ID, C.PriceNorm.MultiAs);
            set => T.PriceNorm.Rows.Set(ID, C.PriceNorm.MultiAs, value);
        }
        /// <summary>Время действия от</summary>
        public int YMFrom
        {
            get => T.PriceNorm.Rows.Get<int>(ID, C.PriceNorm.YMFrom);
            set => T.PriceNorm.Rows.Set(ID, C.PriceNorm.YMFrom, value);
        }
        /// <summary>Время действия до</summary>
        public int YMTo
        {
            get => T.PriceNorm.Rows.Get<int>(ID, C.PriceNorm.YMTo);
            set => T.PriceNorm.Rows.Set(ID, C.PriceNorm.YMTo, value);
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