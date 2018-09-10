using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Employee.Windows.Norm
{
    public class Resolution_NormValues
    {
        public Resolution_NormValues(ResolutionClarify resolutionClarify)
        {
            this._resolutionClarify = resolutionClarify;

            Initialize();
        }

        public void Initialize()
        {
            Dictionary<uint, int> unitsDict;

            var pollutions = Helpers.LogicHelper.PollutionLogic.Find().ToArray();

            {
                var units = _resolutionClarify.ValueNorms.GroupBy(x => x.UnitID).Where(x => x.Key > 0).Select(x => x.First().Unit).ToArray();
                int unitIndex = 1;
                unitsDict = units.ToDictionary(x => x.ID, x => unitIndex++);

                this._values = new NormUnitValue[units.Length, pollutions.Length];

                if (units.Length == 0)
                {
                    for (int j = 0; j < pollutions.Length; j++)
                    {
                        this[j, 0] = new NormUnitValue(pollutions[j].ID, this._resolutionClarify.ID, 0);
                        this[j] = new NormPriceValue(pollutions[j].ID, this._resolutionClarify.ID);
                    }
                }
                else
                {
                    for (int j = 0; j < pollutions.Length; j++)
                    {
                        for (int i = 0; i < units.Length; i++)
                        { this[j, i] = new NormUnitValue(pollutions[j].ID, this._resolutionClarify.ID, units[i].ID); }
                    }
                }
            }

            foreach (var normValue in this._resolutionClarify.ValueNorms)
            {
                int unitIndex;
                unitsDict.TryGetValue(normValue.UnitID, out unitIndex);

                var pollutionIndex = normValue.Pollution.Index;

                this[pollutionIndex, unitIndex] = new NormUnitValue(normValue.ID);
            }

            foreach (var price in this._resolutionClarify.PriceNorms)
            {
                var pollutionIndex = price.Pollution.Index;

                this[pollutionIndex] = new NormPriceValue(price.ID);
            }
        }

        private readonly ResolutionClarify _resolutionClarify;

        private NormUnitValue[,] _values;
        private NormPriceValue[] _prices;

        public NormUnitValue this[int pollutionIndex, int unitIndex]
        {
            get => _values[unitIndex, pollutionIndex];
            private set => _values[unitIndex, pollutionIndex] = value;
        }

        public NormPriceValue this[int pollutionIndex]
        {
            get => _prices[pollutionIndex];
            private set => _prices[pollutionIndex] = value;
        }
    }
}