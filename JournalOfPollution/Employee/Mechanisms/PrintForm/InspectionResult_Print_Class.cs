using AutoTable;
using MAC_2.Calc;
using MAC_2.Employee.Mechanisms;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MAC_2.Model;

namespace MAC_2.PrintForm
{
    public class InspectionResult_Print_Class : BasePrint
    {
        public InspectionResult_Print_Class(Model.SelectionWell selectionWell) : base(data.ETypeTemplate.InspectionResult)
        { this._selectionWell = selectionWell; }

        Model.SelectionWell _selectionWell;
        ISheet sheet;

        MyTools.C_TableExcel table;
        ValueSelection[] values;
        SelectionWell[] _selectionWells;

        public int StartYM { get; private set; }
        public int MonthCount { get; private set; }
        public bool ShowCalcs { get; private set; }
        public bool ShowNorms { get; private set; }

        protected override void internalStart()
        {
            book = TemplateStorage.WorkBook;
            if (!StartMenu())
            { return; }
            LoadTitle();
            if (!LoadTable())
            { return; }
            Print("Результаты контроля", "Результат контроля", EPathPrint.Documents);
        }

        struct mark
        {
            public const string period = "{период}";

            public const string job = "{должность}";
            public const string fio = "{фио}";

            public const string table = "{таблица}";
        }

        private void LoadTitle()
        {
            Substitute = new CellExchange_Class(sheet);

            ClientName(this._selectionWell.Objecte.Client, false);
            ClientAdres(this._selectionWell.Objecte.Client, false);
            ObjectAdres(this._selectionWell.Objecte, false);
            Substitute.AddExchange(mark.period,
                $"{MyTools.YearMonth_From_YM(StartYM, MyTools.EDateTimeTypes.BeautifulWords)} - {MyTools.YearMonth_From_YM(StartYM + MonthCount - 1, MyTools.EDateTimeTypes.BeautifulWords)}",
                0);
            Worker worker = AdditionnTable.GetSigner(data.ETypeTemplate.InspectionResult, "Результат");
            Substitute.AddExchange(mark.fio, worker.FIO, 0);
            Substitute.AddExchange(mark.job, worker.Post, 0);
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(8, 8, 6, 8 + (MonthCount > 6 ? MonthCount / 2 : 0) + (ShowNorms ? 2 : 0)));

            Substitute.Exchange();
            int span = 2 + MonthCount + (ShowNorms ? 2 : 0);
            for (int i = 0; i < 5; i++)
            { sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i, 0, span)); }
        }

        private MyTools.C_ColumnExcel[] DefCol => new MyTools.C_ColumnExcel[]
        {
            new MyTools.C_ColumnExcel("№\nп/п", 0, 1,2),
            new MyTools.C_ColumnExcel("Показатели", 1, 6,2),
            new MyTools.C_ColumnExcel("Ср. за год", MonthCount+2, 2, 2)
        };

        private void loadData()
        {
            var selectedWells = new List<SelectionWell>();
            var values = new List<ValueSelection>();

            uint objectID = _selectionWell.ObjectID;

            for (int i = 0; i < MonthCount; i++)
            {
                var selectionWell = Helpers.LogicHelper.SelectionWellLogic.FirstOrDefault(StartYM + i, objectID, 1);

                if (selectionWell != null)
                {
                    selectedWells.Add(selectionWell);
                    values.AddRange(selectionWell.ValueSelections);
                }
            }

            this.values = values.ToArray();
            this._selectionWells = selectedWells.ToArray();
        }

        private bool LoadTable()
        {
            if (!LoadBase())
            { return false; }

            loadData();

            LoadCap();

            int indexRow = 0;

            var pollutions = new Dictionary<Pollution, int>();

            {   //делаю список показателей
                var prePollutions = new HashSet<Pollution>();

                foreach (var selectionWell in _selectionWells)
                {
                    foreach (var selectionValue in selectionWell.ValueSelections)
                    {
                        if (!prePollutions.Contains(selectionValue.Pollution))
                        { prePollutions.Add(selectionValue.Pollution); }
                    }
                }

                int index = 0;
                pollutions = prePollutions.ToDictionary(x => x, x => index++);
            }

            foreach (var selectionWell in _selectionWells)
            {
                foreach (var selectionValue in selectionWell.ValueSelections)
                {

                }

                if (ShowNorms)
                {

                }
                if (ShowCalcs)
                {

                }
            }

            foreach (var one in values.GroupBy(x => x.Pollution).OrderBy(x => x.Key.Number).ToArray())
            {
                table.value.Add(new MyTools.C_ValueCell(indexRow + 1, indexRow + 2, 0));
                table.value.Add(new MyTools.C_ValueCell(MyTools.TextToUpper(one.Key.FullName).SymbolConverter(), indexRow + 2, 1));
                normPage normPage = null;
                if (ShowNorms)
                { normPage = LoadNorm(one.Key, one.First().SelectionWell.Well, indexRow); }
                foreach (var sw in one.OrderBy(x => x.SelectionWell.Number).ThenBy(x => x.SelectionWell.YMDHM).GroupBy(x => MyTools.YM_From_YMDHM(x.SelectionWell.YMDHM)).ToArray())
                {
                    string value = string.Empty;
                    foreach (var val in sw)
                    { value += $"{val.ValueRound}\n"; }
                    ICellStyle style = null;
                    if (normPage != null)
                    { style = normPage.Verify(sw.Sum(x => x.ValueRound) / sw.Count()); }
                    table.value.Add(new MyTools.C_ValueCell(value.Trim('\n'), indexRow + 2, sw.Key - StartYM + 2, style: style));
                }
                table.value.Add(new MyTools.C_ValueCell(Math.Round(one.Sum(x => x.Value) / one.Count(), one.Key.Round, MidpointRounding.AwayFromZero), indexRow + 2, MonthCount + 2));
                indexRow++;
            }
            LoadVolume();
            for (int i = 2; i < 2 + MonthCount; i++)
            {
                for (int j = 2; j < indexRow + 2; j++)
                {
                    if (table.value.Find(x => x.row == j && x.col == i) == null)
                    { table.value.Add(new MyTools.C_ValueCell('-', j, i)); }
                }
            }
            if (ShowCalcs)
            {
                indexRow += 2;
                table.value.Add(new MyTools.C_ValueCell("Расчёт 621-П", 1 + indexRow, 1));
                table.value.Add(new MyTools.C_ValueCell("Расчёт 644", 2 + indexRow, 1));
                foreach (var one in values.GroupBy(x => MyTools.YM_From_YMDHM(x.SelectionWell.YMDHM)).ToArray())
                {
                    Calc_621 calc_621 = new Calc_621(Helpers.LogicHelper.SampleLogic.FirstModel(one.First().SampleID), this._selectionWell.Objecte, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621")));
                    calc_621.Calc();
                    table.value.Add(new MyTools.C_ValueCell(calc_621.Answer != null ? calc_621.Answer.Sum(x => x.Value.SummNDS).ToMoney() : "", 1 + indexRow, one.Key - StartYM + 2, style: style[0]));

                    Calc_644 calc_644 = new Calc_644(Helpers.LogicHelper.SampleLogic.FirstModel(one.First().SampleID), this._selectionWell.Objecte, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644")));
                    calc_644.Calc();
                    table.value.Add(new MyTools.C_ValueCell(calc_644.Answer != null ? calc_644.Answer.Sum(x => x.Value.SummNDS).ToMoney() : "", 2 + indexRow, one.Key - StartYM + 2, style: style[1]));
                }
                for (int i = 0; i < MonthCount; i++)
                {
                    if (table.value.Find(x => x.col == i + 2 && x.row == indexRow + 1) == null)
                    { table.value.Add(new MyTools.C_ValueCell("", 1 + indexRow, i + 2, style: style[0])); }
                    if (table.value.Find(x => x.col == i + 2 && x.row == indexRow + 2) == null)
                    { table.value.Add(new MyTools.C_ValueCell("", 2 + indexRow, i + 2, style: style[1])); }
                }
            }
            CreateTable(sheet, table);
            ResizeWidth(sheet, MonthCount > 6 ? MyTools.ETypeFormatBook.Horizontal : MyTools.ETypeFormatBook.Vertical, table);
            ResizeHeight(sheet, table);
            return true;
        }

        private normPage LoadNorm(Pollution polution, Well well, int row)
        {
            normPage normPage = new normPage();
            normPage.Set621(polution.ID, well);
            normPage.Set644(polution.ID, well);
            if (normPage._621 != null)
            { table.value.Add(new MyTools.C_ValueCell($"{(polution.HasRange ? $"{normPage._621.FromRound}-" : "")}{normPage._621.ToRound}", row + 2, 3 + MonthCount, style: style[0])); }
            else
            { table.value.Add(new MyTools.C_ValueCell("", row + 2, 3 + MonthCount, style: style[0])); }
            if (normPage._644 != null)
            { table.value.Add(new MyTools.C_ValueCell($"{(polution.HasRange ? $"{normPage._644.FromRound}-" : "")}{normPage._644.ToRound}", row + 2, 4 + MonthCount, style: style[1])); }
            else
            { table.value.Add(new MyTools.C_ValueCell("", row + 2, 4 + MonthCount, style: style[1])); }
            return normPage;
        }
        static ICellStyle[] style;
        class normPage
        {
            public void Set621(uint PollutionID, Well well)
            {
                _621 = PollutionBase_Class.AllValueNorm.FirstOrDefault(x =>
             x.PollutionID == PollutionID
             && x.ResolutionClarify.Resolution.CurtName.Contains("621")
             && ((x.Unit == null && well.UnitID == 0) ? true : (x.Unit != null && x.Unit.ID == well.UnitID)));
            }
            public void Set644(uint PollutionID, Well well)
            {
                _644 = PollutionBase_Class.AllValueNorm.FirstOrDefault(x =>
             x.PollutionID == PollutionID
             && x.ResolutionClarify.Resolution.CurtName.Contains("644"));
            }
            public ValueNorm _621;
            public ValueNorm _644;
            public ICellStyle Verify(decimal value)
            {
                int result = -1;
                if (_621 != null)
                {
                    if (_621.Pollution.HasRange)
                    {
                        if (!(_621.From <= value && _621.To >= value))
                        { result++; }
                    }
                    else
                    {
                        if (!(_621.To >= value))
                        { result++; }
                    }
                }
                if (_644 != null)
                {
                    if (_644.Pollution.HasRange)
                    {
                        if (!(_644.From <= value && _644.To >= value))
                        { result += 2; }
                    }
                    else
                    {
                        if (!(_644.To >= value))
                        { result += 2; }
                    }
                }
                return result > -1 ? style[result] : null;
            }
        }

        private void LoadVolume()
        {
            int row = table.MaxRowValue + 1;
            table.value.Add(new MyTools.C_ValueCell("Объёмы", row, 1));
            foreach (var item in values.GroupBy(x => MyTools.YM_From_YMDHM(x.SelectionWell.YMDHM)).Select(x => new { YM = x.Key, sample = Helpers.LogicHelper.SampleLogic.FirstModel(x.First().SampleID) }).ToArray())
            { table.value.Add(new MyTools.C_ValueCell(item.sample.Volumes.Sum(x => x.Value), row, item.YM - StartYM + 2)); }
        }

        private bool LoadBase()
        {
            var row = SearchRowFromMark(sheet, mark.table, false);

            if (row == null)
            { throw new Exception("Не найдена метка таблицы"); }

            CreateStyle();
            
            table = new MyTools.C_TableExcel(row.RowNum, row.FirstCellNum, Styles.s_RLTB_CC_T10_W);

            return true;
        }
        protected override bool CreateStyle()
        {
            base.CreateStyle();
            style = new ICellStyle[3];
            style[0] = book.CreateCellStyle();
            style[1] = book.CreateCellStyle();
            style[2] = book.CreateCellStyle();
            style[0].WrapText = style[1].WrapText = style[2].WrapText = true;
            style[0].Alignment = style[1].Alignment = style[2].Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            style[0].VerticalAlignment = style[1].VerticalAlignment = style[2].VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            IFont font10 = book.CreateFont();
            font10.FontName = "Times New Roman";
            font10.FontHeightInPoints = (short)10;
            style[0].SetFont(font10);
            style[1].SetFont(font10);
            style[2].SetFont(font10);
            style[0].BorderRight = style[0].BorderLeft = style[0].BorderTop = style[0].BorderBottom =
            style[1].BorderRight = style[1].BorderLeft = style[1].BorderTop = style[1].BorderBottom =
            style[2].BorderRight = style[2].BorderLeft = style[2].BorderTop = style[2].BorderBottom = BorderStyle.Thin;
            style[0].FillPattern = style[1].FillPattern = style[2].FillPattern = FillPattern.SolidForeground;
            style[0].FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            style[1].FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightYellow.Index;
            style[2].FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            return true;
        }
        private void LoadCap()
        {
            table.columns.AddRange(DefCol);
            for (int i = 0; i < MonthCount; i++)
            { table.columns.Add(new MyTools.C_ColumnExcel(MyTools.Month_From_M_C_R(MyTools.M_From_YM(StartYM + i)), i + 2, 3)); }
            int column = 0;
            foreach (var item in values.OrderBy(x => x.SelectionWell.Number).ThenBy(x => x.SelectionWell.YMDHM).GroupBy(x => MyTools.YM_From_YMDHM(x.SelectionWell.YMDHM)).ToArray())
            {
                column = item.Key - StartYM + 2;
                string number = string.Empty;
                foreach (var sw in item.GroupBy(x => x.SelectionWell.Number))
                { number += $"{sw.Key}, "; }
                number = $"{number.Substring(0, number.Length - 2)}-С-{MyTools.YearMonth_From_YM(item.Key, DivisionSymbol: "/")}";
                table.value.Add(new MyTools.C_ValueCell(number, 0, column, style: Styles.s_RLTB_CC_T6_W));
                table.value.Add(new MyTools.C_ValueCell(MyTools.YearMonthDay_From_YMD(MyTools.YMD_From_YMDHM(item.First().SelectionWell.YMDHM)), 1, column,style: Styles.s_RLTB_CC_T9_W));
            }
            if (ShowNorms)
            {
                table.columns.Add(new MyTools.C_ColumnExcel(PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621")).GetResolutionClarify.FullName, 3 + MonthCount, 5, 2, style: Styles.s_RLTB_CC_T6_W));
                table.columns.Add(new MyTools.C_ColumnExcel(PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644")).GetResolutionClarify.FullName, 4 + MonthCount, 5, 2, style: Styles.s_RLTB_CC_T6_W));
            }
        }

        #region предстартовая подготовка

        private bool StartMenu()
        {
            CP = new Control_Print();
            CP.Elems.SetColumnsFromGrid(2, MyTools.GL_Auto);

            CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
            CP.Elems.SetFromGrid(new TextBlock { Text = "Объект:" }, Column: 0);
            TextBlock tbObj = new TextBlock { Text = this._selectionWell.Objecte.Client.Detail.FullName };
            //tbObj.MouseLeftButtonDown += (sender, e) =>
            //  {
            //      SearchGrid_Window sg = new SearchGrid_Window(G.Objecte )
            //  };
            CP.Elems.SetFromGrid(tbObj);


            CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
            DateSelector DateSelect = new DateSelector((int)MyTools.GetNowDate(MyTools.EInputDate.YMD) - 365, "Период отсчёта");
            CP.Elems.SetFromGrid(DateSelect.View, Column: 0, ColumnSpan: 2);

            CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
            CP.Elems.SetFromGrid(new TextBlock { Text = "Колличество месяцев для выгрузки" }, Column: 0);
            ComboBox cbMonths = new ComboBox();
            for (int i = 1; i < 13; i++)
            { cbMonths.Items.Add(i); }
            cbMonths.SelectedIndex = cbMonths.Items.Count - 1;
            CP.Elems.SetFromGrid(cbMonths);

            CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
            CheckBox cbSumm = new CheckBox
            {
                IsChecked = true,
                Content = "Выводить сумму"
            };
            CP.Elems.SetFromGrid(cbSumm, Column: 0, ColumnSpan: 2);

            CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
            CheckBox cbNorms = new CheckBox
            {
                IsChecked = true,
                Content = "Выводить нормативы"
            };
            CP.Elems.SetFromGrid(cbNorms, Column: 0, ColumnSpan: 2);

            CP.ShowDialog();
            StartYM = DateSelect.dateTime.Year * 12 + DateSelect.dateTime.Month;
            MonthCount = cbMonths.SelectedIndex + 1;
            ShowCalcs = (bool)cbSumm.IsChecked;
            ShowNorms = (bool)cbNorms.IsChecked;
            if (MonthCount > 6)
            {
                book.RemoveSheetAt(book.GetSheetIndex("Результат вертикальный"));
                sheet = book.GetSheet("Результат горизонтальный");
            }
            else
            {
                book.RemoveSheetAt(book.GetSheetIndex("Результат горизонтальный"));
                sheet = book.GetSheet("Результат вертикальный");
            }

            return sheet != null;
        }

        #endregion
    }
}