using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;
using MAC_2.Employee.Mechanisms;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.PrintForm
{
    public class Protocol_Print_Class : BasePrint
    {
        public Protocol_Print_Class(SelectionWell[] SelectionWell) : base(data.ETypeTemplate.Protocol)
        {
            if (StaticDate.SelectDate == 0)
            {
                SelectorDataTimer_Window select = new SelectorDataTimer_Window(0, SelectorDataTimer_Window.EType.Day);
                select.ShowDialog();
                if (select.SelectResult > 0)
                { StaticDate.SelectDate = select.SelectResult; }
            }
            _selectionWells = SelectionWell;
            
        }

        SelectionWell[] _selectionWells;

        ISheet sheet;

        protected override void internalStart()
        {
            book = TemplateStorage.WorkBook;

            if (book == null)
            { return; }

            foreach (var selectionWell in _selectionWells)
            {             
                sheet = book.GetSheet("Протокол").CopySheet(selectionWell.FormatNumber.KillChars_For_SaveFile());
                LoadTable(selectionWell);
                LoadTitle(selectionWell);                
            }

            book.RemoveSheetAt(0);
            Print("Протоколы", $"Протоколы {_selectionWells.Select(x=>$"{x.Number}").Aggregate((a,b)=>$"{a}, {b}")}", EPathPrint.Arhives);
        }

        struct mark
        {
            public const string acred = "{акредитация}";
            public const string fio_nac = "{фио начальник}";
            public const string number_prot = "{протокол номер}";
            public const string abon = "{абонент}";
            public const string yr_adr = "{юридический адрес}";
            public const string type_well = "{тип колодца}";
            public const string select_adr = "{место отбора}";
            public const string date_select = "{дата отбора}";
            public const string time_select = "{время отбора}";
            public const string repres = "{представитель абонента}";
            public const string fio = "{фио}";
            public const string job = "{должность}";
            public const string year = "{год}";
            public const string table = "{таблица}";
            public const string day = "{день}";
            public const string month = "{месяц}";
            public const string _month = "{_месяц}";
        }

        static class columns
        {
            public static MyTools.C_ColumnExcel number = new MyTools.C_ColumnExcel("№\nп/п", 0, 1, 1);
            public static MyTools.C_ColumnExcel pollution = new MyTools.C_ColumnExcel("Показатели", 1, 13, 1);
            public static MyTools.C_ColumnExcel unit = new MyTools.C_ColumnExcel("Ед-цы изм.", 2, 5, 1);
            public static MyTools.C_ColumnExcel result = new MyTools.C_ColumnExcel($"Результаты количественного анализа\n{mark.number_prot}", 5, 10, 1);
            public static MyTools.C_ColumnExcel method = new MyTools.C_ColumnExcel("Методики проведения испытаний", 6, 13, 1);
        }
        private void LoadTitle(SelectionWell selectionWell)
        {
            Substitute = new CellExchange_Class(sheet);
            Accredit accredit = AdditionnTable.GetAccredit();
            Substitute.AddExchange(mark.acred, $"{accredit.Text}\nВыдан {accredit.YMDFrom}", 0);
            Worker work = AdditionnTable.GetSigner(data.ETypeTemplate.Protocol, "Протокол");
            Substitute.AddExchange(mark.fio_nac, work.FIO, 0);
            Substitute.AddExchange(mark.number_prot, selectionWell.FormatNumber, 0);
            Substitute.AddExchange(mark.abon, selectionWell.Objecte.Client.Detail.FullName, 0);
            Substitute.AddExchange(mark.yr_adr, Helpers.LogicHelper.AdresLogic.FirstModel(selectionWell.Objecte.Client.Detail.AdresLegalID).Adr.CutAdres(false), 0);
            Substitute.AddExchange(mark.type_well, G.TypeWell.Rows.Get<string>(selectionWell.Well.TypeWellID, C.TypeWell.FullName), 0);
            Substitute.AddExchange(mark.select_adr, selectionWell.Objecte.Adres.CutAdres(false), 0);
            Substitute.AddExchange(mark.date_select, MyTools.StringDate_From_YMDHMS(selectionWell.YMDHM, MyTools.EInputDate.YMDHM, MyTools.EInputDate.YMD), 0);
            Substitute.AddExchange(mark.time_select, MyTools.StringDate_From_YMDHMS(selectionWell.YMDHM, MyTools.EInputDate.YMDHM, MyTools.EInputDate.YMDHM), 0);
            Substitute.AddExchange(mark.repres, new Representative(selectionWell.Sample.RepresentativeID).Post_FIO, 0);
            Substitute.AddExchange(mark.year, DateControl_Class.SelectYear, 0);
            MonthYear();
            NumberFolder(_selectionWells.Select(x=> x.Objecte.NumberFolder).ToArray());
            work = AdditionnTable.GetSigner(data.ETypeTemplate.Protocol, "Протокол");
            Substitute.AddExchange(mark.fio, work.FIO, 0);
            Substitute.AddExchange(mark.job, work.Post, 0);
            int day, month;
            MyTools.Y_M_D_From_YMD(StaticDate.SelectDate, out month, out month, out day);
            Substitute.AddExchange(mark.day, day, 0);
            Substitute.AddExchange(mark.month, MyTools.Month_From_M_C_R(month, Reg: MyTools.ERegistor.ToLower), 0);
            Substitute.AddExchange(mark._month, month.ToString("00"), 0);
            Substitute.Exchange();
        }

        private void LoadTable(SelectionWell selectionWell)
        {
            CreateStyle();
            IRow Row = SearchRowFromMark(sheet, mark.table);
            MyTools.C_TableExcel table = new MyTools.C_TableExcel(Row.RowNum, 0, Styles.s_RLTB_CC_T10_W);
            table.columns.Add(columns.number);
            table.columns.Add(columns.pollution);
            table.columns.Add(columns.unit);

            int colResolution = 3;
            foreach (var one in PollutionBase_Class.AllResolution)
            {
                table.value.Add(new MyTools.C_ValueCell(one.GetResolutionClarify.FullName, 0, colResolution));
                if (colResolution > 3)
                { table.columns.Add(new MyTools.C_ColumnExcel(string.Empty, colResolution, 10)); }
                colResolution++;
            }
            table.MinRowHeight.Add(1, 1800);
            table.columns.Add(new MyTools.C_ColumnExcel("Нормативные показатели", 3, 10, 0, colResolution - 4));
            table.columns.Add(columns.result);
            table.columns.Add(columns.method);

            var values = selectionWell.ValueSelections.OrderBy(x => x.Pollution.Number).ToArray();
            int row = 1;
            foreach (var one in values)
            {
                table.value.Add(new MyTools.C_ValueCell(row, row, 0));
                table.value.Add(new MyTools.C_ValueCell(one.Pollution.FullName, row, 1, 0, 0, Styles.s_RLTB_LC_T10_W));
                table.value.Add(new MyTools.C_ValueCell(AdditionnTable.AllUnits.First(x => x.ID == one.Pollution.UnitsID).Name, row, 2));
                colResolution = 3;
                foreach (var res in PollutionBase_Class.AllResolution)
                {
                    var val = PollutionBase_Class.AllValueNorm.FirstOrDefault(x => x.PollutionID == one.Pollution.ID && x.ResolutionID == res.ID);
                    if (val != null)
                    { table.value.Add(new MyTools.C_ValueCell($"{(val.From > 0 ? $"{val.FromRound}-{val.ToRound}" : val.ToRound.ToString())}", row, colResolution)); }
                    colResolution++;
                }
                table.value.Add(new MyTools.C_ValueCell(Calc(one), row, 5));
                table.value.Add(new MyTools.C_ValueCell(one.Pollution.Method, row, 6, 0, 0, Styles.s_RLTB_LC_T9_W));
                row++;
            }

            CreateTable(sheet, table);
            ResizeWidth(sheet, MyTools.ETypeFormatBook.My, table, 25400);
            ResizeHeight(sheet, table);
        }
        private string Calc(ValueSelection value)
        {
            var accM = PollutionBase_Class.AccurateMeasurements.FirstOrDefault(x => x.PollutionID == value.Pollution.ID && value.Value >= x.From && value.Value <= x.To);
            decimal result = value.Value * 0.01m * (accM != null ? accM.Value : 0);
            return $"{value.ValueRound} ± {Math.Round(result, 4)}";
        }
    }
}