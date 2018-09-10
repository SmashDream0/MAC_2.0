using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model.NavigationProperty
{
    public class ListNavigationPropertyTyped<ModelT>
        : ListNavigationProperty<ModelT, uint>
        where ModelT : MyTools.C_A_BaseFromAllDB
    {
        public ListNavigationPropertyTyped(MyTools.C_A_BaseFromAllDB parent, Func<ModelT, uint> getCheckValue)
            : base(parent.ID, getCheckValue)
        { this._parent = parent; }

        private readonly MyTools.C_A_BaseFromAllDB _parent;
    }
}