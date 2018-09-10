using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Employee.HelpSelect.Selector
{
    public class NegotiationAssistantSearch
        : Model.NegotiationAssistant, ISearch, IThisShow
    {
        public NegotiationAssistantSearch(Model.NegotiationAssistant negotiationAssistant)
            : base(negotiationAssistant.ID)
        {
            this.Add(negotiationAssistant.Objecte);

            if (negotiationAssistant.Sample != null)
            { this.Add(negotiationAssistant.Sample); }

            if (negotiationAssistant.Worker != null)
            { this.Add(negotiationAssistant.Worker); }

            _values = new Dictionary<string, string>();
            _values.Add("Наименование", Objecte.Client.Detail?.FullName.StringDivision());
            _values.Add("Адрес", Objecte.Adres.StringDivision(30));

            _prevSample = Helpers.LogicHelper.SampleLogic.FirstOrDefault(Helpers.PeriodHelper.YM - 1, negotiationAssistant.Objecte.ID);
        }

        private readonly Dictionary<string, string> _values;
        private readonly Model.Sample _prevSample;

        public Dictionary<string, string> values => _values;

        public uint NegotiationAssistandID
        {
            get => this.ID;
            set { }
        }

        public string Name => this.Objecte.Client.Detail.FullName.StringDivision();

        public string Number => this.Objecte.NumberFolder.ToString();
        public string Adres => this.Objecte.Adres.StringDivision(30);
        public string Well => this.Objecte.Wells.Count().ToString();
        public string DateProspective => (YMD > 0 ? MyTools.YearMonthDay_From_YMD(YMD) : "нет акта");
        public string DateLastSelect => (_prevSample != null ? MyTools.YearMonth_From_YM(_prevSample.YM) : String.Empty);
        public string FIO_Post => (Worker == null ? String.Empty: Worker.FIO);
        public string DumpPool => (base.Sample.SelectionWells.Select(x => Helpers.LogicHelper.UnitLogic.FirstModel(x.Well.UnitID).Name).Aggregate((a, b) => a + "," + b));// Dumps.Length > 0 ? Dumps.Select(x => new Unit(x)).Select(x => x.Name).Aggregate((a, b) => a + "," + b) : string.Empty;
    }

    public interface IThisShow
    {
        uint ID { get; }
        uint NegotiationAssistandID { get; set; }
    }
}
