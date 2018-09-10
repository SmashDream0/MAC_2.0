using AutoTable;
using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MAC_2.Model;

namespace MAC_2.PrintForm
{
    public class Letter_Print_Class : BasePrint
    {
        public Letter_Print_Class(Model.NegotiationAssistant negotiationAssistant) : base(data.ETypeTemplate.LetterNotification)
        {
            this._negotiationAssistant = negotiationAssistant;
        }
        Model.NegotiationAssistant _negotiationAssistant;

        protected override void internalStart()
        {
            Objecte obj = _negotiationAssistant.Objecte;
            book = TemplateStorage.WorkBook;

            Substitute = new CellExchange_Class(book.GetSheetAt(0));
            ClientName(_negotiationAssistant.Objecte.Client);
            NumberFolder(obj.NumberFolder);

            #region Подписывающий
            var works = AdditionnTable.GetSigner(data.ETypeTemplate.LetterNotification, "Письмо");
            Substitute.AddExchange("{должность}", works.Post, 0);
            Substitute.AddExchange("{ФИО}", works.FIO, 0);
            #endregion

            #region адрес/дата            
            ObjectAdres(obj, true);
            CP = new Control_Print();
            CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
            var DateSelect = new DateSelector(_negotiationAssistant.YMD);
            CP.Elems.SetFromGrid(DateSelect.View);
            CP.ShowDialog();

            if (DateSelect.dateTime == null)
            { return; }

            _negotiationAssistant.YMD = MyTools.YMD_From_DateTime(DateSelect.dateTime);
            Substitute.AddExchange("{дата отбора}", MyTools.YearMonthDay_From_YMD(DateSelect.dateTime), 0);
            #endregion

            Substitute.AddExchange("{адрес отбора}", obj.Adres, 0);
            Substitute.Exchange();
            Print("Письма", "Письмо", EPathPrint.Documents);
        }
    }
}