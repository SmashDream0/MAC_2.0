using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Logic
{
    public class AccurateMeasurementLogic
        : BaseLogicTyped<AccurateMeasurement>
    {
        public AccurateMeasurementLogic() : base(T.AccurateMeasurement)
        { }

        protected override AccurateMeasurement internalGetModel(uint id)
        { return new AccurateMeasurement(id); }

        public override IEnumerable<AccurateMeasurement> Find()
        {
            return getQuerryResult($"all",
                (table) =>
                {
                    table.QUERRY().SHOW.DO();
                }
                ,(result)=>
                {
                    var pollutions = LogicHelper.PollutionLogic.Find();
                    var dictionary = LogicHelper.PollutionLogic.GetDictionary(pollutions);

                    foreach (var accurateMeasurement in result)
                    {
                        if (dictionary.ContainsKey(accurateMeasurement.PollutionID))
                        {
                            var pollution = dictionary[accurateMeasurement.PollutionID];

                            accurateMeasurement.Pollution.Add(pollution);
                        }
                    }

                });
        }

    }
}
