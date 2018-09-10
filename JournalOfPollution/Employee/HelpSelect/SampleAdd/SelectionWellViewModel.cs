using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using MAC_2.Employee.Mechanisms;

namespace MAC_2.Employee.HelpSelect
{
    public class SelectionWellViewModel
    {
        public SelectionWellViewModel(Model.NegotiationAssistant negotiationAssistant, Model.Well well, Model.SelectionWell selectionWell = null)
        {
            this.NegotiationAssistant = negotiationAssistant;
            this.Well = well;
            this.SelectionWell = selectionWell;
        }

        public Model.Well Well { get; private set; }
        public Model.NegotiationAssistant NegotiationAssistant { get; private set; }
        public Model.SelectionWell SelectionWell { get; private set; }

        public int Number
        {
            get
            {
                if (SelectionWell == null)
                { return 0; }
                else
                { return SelectionWell.Number; }
            }
            set
            {
                if (SelectionWell == null)
                {
                    if (NegotiationAssistant.SampleID == 0)
                    {
                        NegotiationAssistant.SampleID = MyTools.AddRowFromTable(G.Sample,
                          new KeyValuePair<int, object>(C.Sample.YM, DateControl_Class.SelectMonth),
                          new KeyValuePair<int, object>(C.Sample.Worker, NegotiationAssistant.WorkerID),
                          new KeyValuePair<int, object>(C.Sample.Status, (uint)data.EStatus.Selected));
                    }

                    var selectionWellID = MyTools.AddRowFromTable(G.SelectionWell,
                            new KeyValuePair<int, object>(C.SelectionWell.Well, Well.ID),
                            new KeyValuePair<int, object>(C.SelectionWell.Sample, NegotiationAssistant.SampleID),
                            new KeyValuePair<int, object>(C.SelectionWell.Number, value)
                            );

                    this.SelectionWell = Helpers.LogicHelper.SelectionWellLogic.FirstOrDefault(selectionWellID);
                }
                else
                {
                    SelectionWell.Number = value;
                }
            }
        }
    }
}
