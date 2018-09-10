using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.PrintForm;
using MAC_2.Model;
using MAC_2.Employee.Mechanisms;
using AutoTable;

namespace MAC_2.Employee.HelpSelect.Selector
{
    public class NegotiationItem : SearchItem
    {
        public NegotiationItem(uint ID)
        {
            this.ID = ID;
            this.NegotiationAssistantID = ID;
            Objecte = Logic.LogicInstances.ObjecteLogic.FirstOrDefault(ObjecteID);

            LoadSample(SampleID);

            result = new Dictionary<string, string>();
            result.Add("Наименование", Name);
            result.Add("Адрес", Adres);
        }

        public new readonly uint ID;
        public uint WorkerID
        {
            get { return T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Worker); }
            set { G.NegotiationAssistant.QUERRY().SET.C(C.NegotiationAssistant.Worker, value).WHERE.ID(ID).DO(); }
        }
        public uint SampleID => T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Sample);
        public uint ObjecteID => T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Objecte);
        public int YMD
        {
            get { return T.NegotiationAssistant.Rows.Get<int>(ID, C.NegotiationAssistant.YMD); }
            set { G.NegotiationAssistant.QUERRY().SET.C(C.NegotiationAssistant.YMD, value).WHERE.ID(ID).DO(); }
        }
        public string DateProspective => YMD > 0 ? MyTools.YearMonthDay_From_YMD(YMD) : "нет акта";
        public string FIO_Post => new Worker(WorkerID).FIO;
    }
}
