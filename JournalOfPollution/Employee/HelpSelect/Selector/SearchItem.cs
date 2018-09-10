using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.PrintForm;
using MAC_2.Model;
using AutoTable;
using MAC_2.Employee.Mechanisms;

namespace MAC_2.Employee.HelpSelect.Selector
{
    public abstract class SearchItem : ISearch
    {
        public void LoadSample(uint sampleID)
        {
            Sample = Logic.LogicInstances.SampleLogic.FirstOrDefault(sampleID);
        }

        public uint ID => Objecte.ID;
        protected Dictionary<string, string> result;
        public Dictionary<string, string> values => result;
        public Objecte Objecte;
        public Client Client => Objecte.Client;
        public Sample Sample;
        public string Name => Client.Detail?.FullName.StringDivision();
        public string DateLastSelect => Sample != null ? MyTools.YearMonth_From_YM(Sample.YM) : string.Empty;

        public string Number => Objecte.NumberFolder.ToString();
        public string Adres => Objecte.Adres.StringDivision(30);
        public string Well => Objecte.Wells.Count().ToString();
        public string MidMonthVolume => Objecte.GetMidMonthVolume(DateControl_Class.SelectYear - 1).Volume.ToString();

        public uint NegotiationAssistantID;
    }
}
