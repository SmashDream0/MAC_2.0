using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class AdresLogic
        : BaseLogicTyped<Adres>
    {
        public AdresLogic() : base(T.AdresReference)
        { }

        protected override Adres internalGetModel(uint id)
        { return new Adres(id); }

    }
}
