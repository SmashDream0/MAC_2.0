using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Декларация</summary>
    public class Declaration : MyTools.C_A_BaseFromAllDB
    {
        public Declaration(uint ID, bool LoadAll = false, bool CanEdit = true) : base(G.Declaration, ID, CanEdit)
        {
            InitializeNavigations();
        }

        private void InitializeNavigations()
        {
            DeclarationValues = new NavigationProperty.ListNavigationPropertyTyped<DeclarationValue>(this, (model) => model.DeclarationID);
            DeclarationValues.OnAdd += (model) => model.Add(this);

            _well = new NavigationProperty.NavigationPropertryTyped<Well>(this.WellID);
            _well.OnAdd += (model) => model.Add(this);
        }

        private NavigationProperty.NavigationPropertryTyped<Well> _well;
        public Well Well => _well.Model;

        /// <summary>Действует с в месяцах</summary>
        public int YM => T.Declaration.Rows.Get<int>(ID, C.Declaration.YM);
        /// <summary>Наименование</summary>
        public string Name => T.Declaration.Rows.Get<string>(ID, C.Declaration.Name);

        public uint WellID => T.Declaration.Rows.Get_UnShow<uint>(ID, C.Declaration.Well);

        public NavigationProperty.ListNavigationPropertyTyped<DeclarationValue> DeclarationValues
        { get; private set; }

        public bool Add(Well well) => _well.Add(well);
    }
}
