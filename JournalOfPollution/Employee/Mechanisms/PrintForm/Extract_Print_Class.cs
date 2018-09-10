using MAC_2.Employee.Mechanisms;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using AutoTable;
using System.IO;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.PrintForm
{
    public class Extract_Print_Class : BasePrint
    {
        public Extract_Print_Class(Sample sample) : base(data.ETypeTemplate.Extract)
        { this.sample = sample; }
        Sample sample;

        protected override void internalStart()
        {
            if (!Loader())
            { return; }
            book = TemplateStorage.WorkBook;
            CreateStyle();
            Letter();
            Applications();
            Print("Выписки", "Выписка", EPathPrint.Documents);
        }

        Objecte obj;
        Client client;
        SelectionWell[] selections;

        private bool Loader()
        {
            selections = sample.SelectionWells.ToArray();

            obj = selections.FirstOrDefault().Objecte;
            client = obj.Client;

            if (obj.GetMidMonthVolume(DateControl_Class.SelectYear - 1).Volume == 0)
            {
                MessageBox.Show("Среднемесячный объём отсутствует!");
                return false;
            }
            return true;
        }
        /// <summary>Обработка письма</summary>
        private void Letter()
        {
            Substitute = new CellExchange_Class(book.GetSheet("Письмо"));
            ClientName(client);
            ObjectAdres(obj, true);
            MonthYearSelect();
            NumberFolder(obj.NumberFolder);
            Substitute.AddExchange("{год месячного объёма}", DateControl_Class.SelectYear - 1, 0);
            Substitute.AddExchange("{листов}", selections.Length, 0);
            NumberFolder(obj.NumberFolder);
            var works = AdditionnTable.GetSigner(data.ETypeTemplate.Extract, "Письмо");
            Substitute.AddExchange("{должность}", works.Post, 0);
            Substitute.AddExchange("{фио}", works.FIO, 0);
            Substitute.Exchange();
        }
        TableSelection tableSelection;
        /// <summary>Приложение</summary>
        private void Applications()
        {
            foreach (var one in selections)
            {
                Well well = one.Well;

                ISheet sheet = book.GetSheet("Приложение").CopySheet(well.PresentNumber != string.Empty ? well.PresentNumber : "без_номера");
                Substitute = new CellExchange_Class(sheet);
                //Substitute.AddExchange("{объект}", obj.Adres + ' ' + well.PresentNumber, 0);
                Substitute.AddExchange("{абонент}", client.Detail.FullName, 0);
                Substitute.AddExchange("{колодец}", $"{well.FullName} - {well.PresentNumber}", 0);
                Substitute.AddExchange("{адрес}", (client.Objects.Any() ? obj.Adres.CutAdres(false) : string.Empty), 0);
                Substitute.AddExchange("{номер отбора}", one.FormatNumber, 0);
                {
                    var works = AdditionnTable.GetSigner(data.ETypeTemplate.Extract, "Приложение");
                    Substitute.AddExchange("{должность}", works.Post, 0);
                    Substitute.AddExchange("{фио}", works.FIO, 0);
                }
                tableSelection = new TableSelection(obj, sample);
                tableSelection.CreateTable(SearchCellFromMark(sheet, "{таблица}", false), one);
                tableSelection.Signature(SearchCellFromMark(sheet, "{пример}", false), Substitute);
                Substitute.AddExchange("{полное наименование колодца}", well.FullName + ' ' + well.PresentNumber, 0);
                Substitute.Exchange();
            }
            book.RemoveSheetAt(book.GetSheetIndex("Приложение"));
        }
    }
}