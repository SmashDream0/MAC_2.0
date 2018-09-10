using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;

namespace MAC_2.Model
{
    /// <summary>Актуализация постановления</summary>
    public class ResolutionClarify : MyTools.C_A_BaseFromAllDB
    {
        public ResolutionClarify(uint id)
            : base(G.ResolutionClarify, id, true)
        {
            _resolution = new NavigationProperty.NavigationPropertryTyped<Resolution>(ResolutionID);
            _valueNorm = new NavigationProperty.ListNavigationPropertyTyped<ValueNorm>(this, (model) => model.ResolutionClarifyID);
            _priceNorm = new NavigationProperty.ListNavigationPropertyTyped<PriceNorm>(this, (model) => model.ResolutionClarifyID);
        }

        /// <summary>ID Постановление</summary>
        public uint ResolutionID => T.ResolutionClarify.Rows.Get_UnShow<uint>(ID, C.ResolutionClarify.Resolution);
        /// <summary>Полное наименование</summary>
        public string FullName => T.ResolutionClarify.Rows.Get<string>(ID, C.ResolutionClarify.FullName);
        /// <summary>Описание</summary>
        public string Note => T.ResolutionClarify.Rows.Get<string>(ID, C.ResolutionClarify.Note);
        /// <summary>Акты</summary>
        public string Acts => T.ResolutionClarify.Rows.Get<string>(ID, C.ResolutionClarify.Acts);
        /// <summary>Действует с</summary>
        public int YMFrom => T.ResolutionClarify.Rows.Get<int>(ID, C.ResolutionClarify.YMFrom);
        /// <summary>Действует до</summary>
        public int YMTo => T.ResolutionClarify.Rows.Get<int>(ID, C.ResolutionClarify.YMTo);
        /// <summary>Тип постановления</summary>
        public data.ETypeResolution TypeResolution => (data.ETypeResolution)T.ResolutionClarify.Rows.Get_UnShow<uint>(ID, C.ResolutionClarify.TypeResolution);
                
        private readonly NavigationProperty.NavigationPropertryTyped<Resolution> _resolution;
        private readonly NavigationProperty.ListNavigationPropertyTyped<ValueNorm> _valueNorm;
        private readonly NavigationProperty.ListNavigationPropertyTyped<PriceNorm> _priceNorm;

        public IEnumerable<ValueNorm> ValueNorms => _valueNorm;
        public IEnumerable<PriceNorm> PriceNorms => _priceNorm;

        public bool Add(ValueNorm valueNorm)
        { return _valueNorm.Add(valueNorm); }

        public bool Add(PriceNorm priceNorm)
        { return _priceNorm.Add(priceNorm); }

        /// <summary>Постановление</summary>
        public Resolution Resolution => _resolution.Model;

        public bool Add(Resolution resolution) => _resolution.Add(resolution);
    }
}
