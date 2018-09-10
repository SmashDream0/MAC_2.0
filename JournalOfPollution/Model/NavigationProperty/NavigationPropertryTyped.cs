using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model.NavigationProperty
{
    public class NavigationPropertryTyped<ModelT>
        : NavigationProperty<ModelT, uint>
        where ModelT : MyTools.C_A_BaseFromAllDB
    {
        public NavigationPropertryTyped(uint id)
            : base(id, (model) => model.ID)
        { }
    }
}