using AutoTable;
using MAC_2.Employee.Mechanisms;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.PrintForm
{
    public class Journal_Print_Class : BasePrint
    {
        public Journal_Print_Class() : base(data.ETypeTemplate.Journal)
        { }

        protected override void internalStart()
        {
            book = TemplateStorage.WorkBook;
            CreateStyle();
            LoadTitle();
            LoadTable();
            Print("Журналы", $"Журнал на {MyTools.YearMonth_From_YM(DateControl_Class.SelectMonth)}", EPathPrint.Documents);
        }

        ISheet sheet;

        class mark
        {
            public const string fio = "{фио}";
            public const string job = "{должность}";

            public const string number_sample = "{номера проб}";
        }

        private void LoadTitle()
        {
            sheet = book.GetSheet("Титульник");
            Substitute = new CellExchange_Class(sheet);
            Worker worker = AdditionnTable.GetSigner(data.ETypeTemplate.Journal, "Титульник");

            Substitute.AddExchange(mark.fio, worker.FIO, 1);
            Substitute.AddExchange(mark.job, worker.Post, 1);

            Substitute.Exchange();
        }
        TableSelection tableSelection;
        private void LoadTable()
        {
            sheet = book.GetSheet("Таблица");
            Substitute = new CellExchange_Class(sheet);
            IRow row = SearchRowFromMark(sheet, StaticMark.abonent, false);

            var selectionWells = Helpers.LogicHelper.SelectionWellLogic.Find(DateControl_Class.SelectMonth, 1);

            foreach (var selectionWell in selectionWells)
            {
                sheet.ShiftRows(row.RowNum+3, sheet.LastRowNum, 0);
                row.CopyRowTo(row.RowNum + 3);
                sheet.GetRow(row.RowNum + 1).CopyRowTo(row.RowNum + 4);
                sheet.GetRow(row.RowNum + 2).CopyRowTo(row.RowNum + 5);
            }

            foreach (var selectionWell in selectionWells.OrderBy(x => x.Number))
            {
                Client client = selectionWell.Objecte.Client;

                ClientName(client, false);
                WellNumber(selectionWell);
                Substitute.AddExchange(mark.number_sample,
                    $"{selectionWell.Number}-С-{MyTools.YearMonth_From_YM(MyTools.YM_From_YMDHM(selectionWell.YMDHM), MyTools.EDateTimeTypes.DivisionSymbol, DivisionSymbol: "/")}",
                    1);
                row = SearchRowFromMark(sheet, "{таблица}");
                tableSelection = new TableSelection(selectionWell.Objecte, selectionWell.Sample);
                tableSelection.CreateTable(SearchCellFromMark(sheet, "{таблица}", false), selectionWell);                
            }

            tableSelection.Signature(SearchCellFromMark(sheet, "{пример}", false), Substitute);
            ResizeWidth(sheet, MyTools.ETypeFormatBook.Horizontal, tableSelection.table);
            MonthYearSelect();
            Worker worker = AdditionnTable.GetSigner(data.ETypeTemplate.Journal, "Таблица");
            Substitute.AddExchange(mark.fio, worker.FIO, 1);
            Substitute.AddExchange(mark.job, worker.Post, 1);
            Substitute.Exchange();
        }
    }
}