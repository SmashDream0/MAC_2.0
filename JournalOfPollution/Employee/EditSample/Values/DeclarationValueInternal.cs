using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.EditSample
{
    internal class DeclarationValueInternal
        : BaseValue
    {
        public DeclarationValueInternal(MAC_2.Model.DeclarationValue declarationValue)
            : base(declarationValue.Pollution.BindName)
        { this._declarationValue = declarationValue; }
        public DeclarationValueInternal(MAC_2.Model.Declaration declaration, MAC_2.Model.Pollution pollution)
            : base(pollution.BindName)
        {
            this._declaration = declaration;
            this._pollution = pollution;
        }

        MAC_2.Model.Declaration _declaration = null;
        MAC_2.Model.Pollution _pollution = null;
        MAC_2.Model.DeclarationValue _declarationValue = null;

        public override decimal? Value
        {
            get
            {
                if (_declarationValue == null)
                { return null; }
                else
                { return _declarationValue.ToRound; }
            }
            set { setValue(value); }
        }

        private void setValue(decimal? value)
        {
            if (_declarationValue == null)
            {
                if (value.HasValue)
                {
                    var id = (uint)G.DeclarationValue.QUERRY()
                        .ADD
                            .C(C.DeclarationValue.Pollution, _pollution.ID)
                            .C(C.DeclarationValue.Declaration, _declaration.ID)
                            .C(C.DeclarationValue.To, value.Value)
                        .DO()[0].Value;

                    _declarationValue = Helpers.LogicHelper.DeclarationValueLogic.FirstModel(id);
                }
            }
            else
            {
                _declarationValue.To = value.Value;
            }
        }
    }
}