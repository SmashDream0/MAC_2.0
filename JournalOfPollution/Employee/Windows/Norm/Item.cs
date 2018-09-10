using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Employee.Mechanisms;
using System.Windows;

namespace MAC_2.Employee.Windows.Norm
{
    class PollutionItem : NormBaseValue
    {
        public PollutionItem(uint ID)
        {
            this._pollution = Helpers.LogicHelper.PollutionLogic.FirstModel(ID);
            this._accuratesItems = PollutionBase_Class.AccurateMeasurements.Where(x => x.PollutionID == ID).ToArray();
                        
            {
                this.ValueNorm = new Dictionary<string, NormBaseValue>();

                var valueNorms = PollutionBase_Class.AllValueNorm.Where(x => x.PollutionID == ID);

                foreach (var valueNorm in valueNorms)
                {
                    var normUnitValue = new NormUnitValue(valueNorm.ID);

                    ValueNorm.Add($"{valueNorm.UnitID}|{valueNorm.ResolutionClarifyID}", normUnitValue);
                }
            }

            {
                var priceNorms = PollutionBase_Class.AllPriceNorm.Where(x => x.PollutionID == ID);

                this.priceNorms = new Dictionary<string, NormBaseValue>();

                foreach (var priceNorm in priceNorms)
                {
                    var normPriceValue = new NormPriceValue(priceNorm.ID);

                    this.priceNorms.Add($"{priceNorm.ResolutionClarifyID}", normPriceValue);
                }
            }

            {
                //теперь все прочие сочетания
                var resolutionClarifies = Helpers.LogicHelper.ResolutionClarifyLogic.Find(DateControl_Class.SelectMonth);
                var units = Helpers.LogicHelper.UnitLogic.Find();

                foreach (var resolutionClarify in resolutionClarifies)
                {
                    {
                        var key = $"{resolutionClarify.ID}";

                        if (!this.priceNorms.ContainsKey(key))
                        {
                            this.priceNorms.Add(key, new NormPriceValue(this._pollution.ID, resolutionClarify.ID));
                        }
                    }

                    {
                        var key = $"{0}|{resolutionClarify.ID}";

                        if (!this.ValueNorm.ContainsKey(key))
                        {
                            this.ValueNorm.Add(key, new NormUnitValue(this._pollution.ID, resolutionClarify.ID));
                        }
                    }

                    foreach (var unit in units)
                    {
                        var key = $"{unit.ID}|{resolutionClarify.ID}";

                        if (!this.ValueNorm.ContainsKey(key))
                        {
                            this.ValueNorm.Add(key, new NormUnitValue(this._pollution.ID, resolutionClarify.ID, unit.ID));
                        }
                    }
                }
            }
        }

        private Pollution _pollution;

        private IEnumerable<AccurateMeasurement> _accuratesItems;

        public Dictionary<string, NormBaseValue> ValueNorm { get; }
        public Dictionary<string, NormBaseValue> priceNorms { get; }

        public int Key
        {
            get { return _pollution.Key; }
            set
            {
                if (PollutionBase_Class.AllPolutions.FirstOrDefault(x => x.Key == value) != null)
                { MessageBox.Show("Нельзя задать такой ключ!", "Повтор ключа"); }
                else
                { _pollution.Key = value; }
            }
        }

        public int Number
        {
            get { return _pollution.Number; }
            set
            {
                if (PollutionBase_Class.AllPolutions.FirstOrDefault(x => x.Number == value) != null)
                { MessageBox.Show($"Такой порядковый номер зарезервирован{PollutionBase_Class.AllPolutions.First(x => x.Number == value).CurtName}"); }
                else
                { _pollution.Number = value; }
            }
        }

        public string CurtName
        {
            get { return _pollution.CurtName; }
            set { _pollution.CurtName = value; }
        }

        public string FullName
        {
            get { return _pollution.FullName; }
            set { _pollution.FullName = value; }
        }

        public int Round
        {
            get { return _pollution.Round; }
            set { _pollution.Round = value; }
        }

        public bool Show
        {
            get { return _pollution.Show; }
            set { _pollution.Show = value; }
        }

        public override string Value
        {
            get => getAccurates();
            set
            {

            }
        }

        private string _accurates;

        private string getAccurates()
        {
            if (_accurates == null)
            {
                if (_accuratesItems.Any())
                {
                    _accurates = _accuratesItems.OrderBy(x => x.From)
                        .Select(x => $"{gecimalToString(x.From)} - {gecimalToString(x.To)}{(x.Value > 0 ? $"={gecimalToString(x.Value)}" : string.Empty)}\n")
                        .Aggregate((a, b) => a + b).Trim('\n');
                }
                else
                { _accurates = string.Empty; }
            }

            return _accurates;
        }
    }
}
