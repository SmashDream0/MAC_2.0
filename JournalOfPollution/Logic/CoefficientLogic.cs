using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class CoefficientLogic
        : BaseLogicTyped<Coefficient>
    {
        public CoefficientLogic() : base(T.Coefficient)
        { }

        protected override Coefficient internalGetModel(uint id)
        { return new Coefficient(id); }

        public override IEnumerable<Coefficient> Find()
        {
            return getQuerryResult($"all"
                , (table) =>
                 {
                     table.QUERRY().SHOW.DO();
                 }
                 , (result) =>
                  {
                      var coefficientValues = LogicHelper.CoefficientValueLogic.Find();

                      var dictionary = this.GetDictionary(result);

                      foreach (var coefficientValue in coefficientValues)
                      {
                          if (dictionary.ContainsKey(coefficientValue.CoefficientID))
                          {
                              var coefficient = dictionary[coefficientValue.CoefficientID];

                              coefficient.Add(coefficientValue);
                          }
                      }
                  });
        }
    }
}
