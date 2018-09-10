using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Windows;
using System.Windows.Controls;
using AutoTable;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.PrintForm
{
    public class ActSelect_Print_Class : BasePrint
    {
        public ActSelect_Print_Class(Model.NegotiationAssistant negotiationAssistant)
            : base(data.ETypeTemplate.ActSelection)
        {
            this._negotiationAssistant = negotiationAssistant;
        }

        NegotiationAssistant _negotiationAssistant;

        protected override void internalStart()
        {
            {
                CP = new Control_Print();
                CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
                var DateSelect = new DateSelector(_negotiationAssistant.YMD);
                CP.Elems.SetFromGrid(DateSelect.View);
                CP.ShowDialog();

                if (_negotiationAssistant.WorkerID == 0)
                {
                    MessageBox.Show("Не выбран пробоотборщик!");
                    return;
                }

                _negotiationAssistant.YMD = MyTools.YMD_From_DateTime(DateSelect.dateTime);
            }

            Objecte obj = _negotiationAssistant.Objecte;
            Accredit acc = AdditionnTable.GetAccredit();
            string sampler = _negotiationAssistant.Worker.Post_FIO;

            foreach (var one in obj.Wells)
            {
                book = TemplateStorage.WorkBook;
                Substitute = new CellExchange_Class(book.GetSheetAt(0));
                var Details = obj.Client.Detail;
                Substitute.AddExchange("{абонент}", Details.FullName, 0);
                if (obj.Separate)
                { Substitute.AddExchange("{юридический адрес}", obj.Detail.LegalAdres, 0); }
                else
                { Substitute.AddExchange("{юридический адрес}", Helpers.LogicHelper.AdresLogic.FirstModel(Details.AdresLegalID).Adr, 0); }
                Substitute.AddExchange("{тип колодца}", new TypeWell(one.TypeWellID).FullName + " " + new TypeWell(one.TypeWellID).CurtName + '-' + one.Number, 0);
                Substitute.AddExchange("{место отбора}", obj.Adres.CutAdres(false), 0);
                Substitute.AddExchange("{аккредитация}", acc.Text, 0);
                Substitute.AddExchange("{дата аккредитации}", acc.YMDFrom, 0);
                Substitute.AddExchange("{пробоотборщик}", sampler, 0);
                //Substitute.AddExchange("{представитель абонента}", sample == null ? string.empty : sample.IDRepresentative > 0 ? new Representative(sample.IDRepresentative).Post_FIO : string.empty, 0);
                Substitute.Exchange();
                Print("Акты", "акт отбора пробы " + new TypeWell(one.TypeWellID).FullName + ' ' + one.Number, EPathPrint.Documents);
            }
        }
    }
}