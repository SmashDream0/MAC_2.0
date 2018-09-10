using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;
using System.Windows.Media;
using MAC_2.Employee.Mechanisms;
using MAC_2.Model.NavigationProperty;

namespace MAC_2.Model
{
    /// <summary>Постановление</summary>
    public class Resolution : MyTools.C_A_BaseFromAllDB
    {
        public Resolution(uint ID, bool CanEdit = true) : base(G.Resolution, ID, CanEdit)
        {
            bc = new BrushConverter();
            ListResolutionClarify = new ListNavigationPropertyTyped<ResolutionClarify>(this, (rc) => rc.ResolutionID);
            ListNormDoc = new ListNavigationPropertyTyped<NormDoc>(this, (nd) => nd.ResolutionID);

            ListResolutionClarify.OnAdd += (model) =>
            { model.Add(this); };
            ListNormDoc.OnAdd += (model) =>
            { model.Add(this); };
        }
        /// <summary>Краткое наименование</summary>
        public string CurtName => T.Resolution.Rows.Get<string>(ID, C.Resolution.CurtName);
        /// <summary>Принудительно отключено</summary>
        public bool OffFocre => T.Resolution.Rows.Get<bool>(ID, C.Resolution.OffForce);
        /// <summary>Действует с</summary>
        public int YMFrom => T.Resolution.Rows.Get<int>(ID, C.Resolution.YMFrom);
        /// <summary>Действует до</summary>
        public int YMTo => T.Resolution.Rows.Get<int>(ID, C.Resolution.YMTo);
        /// <summary>Цвет</summary>
        public Brush Color => (Brush)bc.ConvertFrom(T.Resolution.Rows.Get<string>(ID, C.Resolution.Color));
        private Dictionary<uint, ResolutionClarify> _listResolutionClarify = new Dictionary<uint, ResolutionClarify>();
        private Dictionary<uint, NormDoc> _listNormDoc = new Dictionary<uint, NormDoc>();

        public ListNavigationPropertyTyped<ResolutionClarify> ListResolutionClarify
        { get; private set; }

        public ListNavigationPropertyTyped<NormDoc> ListNormDoc
        { get; private set; }

        public ResolutionClarify GetResolutionClarify => ListResolutionClarify.FirstOrDefault(x => (x.YMFrom <= DateControl_Class.SelectMonth || x.YMFrom == 0) && (x.YMTo >= DateControl_Class.SelectMonth || x.YMTo == 0));
    }
}
