using MAC_2.Employee.Mechanisms;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using MAC_2.Calc;
using MAC_2.Model;

namespace MAC_2.PrintForm
{
    public class CalculationFees_Print_Class : BasePrint
    {
        public CalculationFees_Print_Class(Sample sample) : base(data.ETypeTemplate.CalculationFees)
        {
            _sample = sample;
            //values = sample
            wells = sample.SelectionWells.ToArray();
            obj = sample.SelectionWells.First().Objecte;
            Start();
        }
        Sample _sample;
        SelectionWell[] wells;
        Objecte obj;
        bool R621, R644;

        protected override void internalStart()
        {
            book = TemplateStorage.WorkBook;
            if(!CreateStyle())
            { return; }
            string ListName = "621";
            #region 621
            Resolution resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(ListName));
            if (obj.CanResolution(resolution.ID))
            {
                GeneralSumma = new List<decimal>();

                var index = book.GetSheetIndex("Расчёт");

                sheet = book.CloneSheet(index);

                book.SetSheetName(book.NumberOfSheets - 1, ListName);
                Substitute = new CellExchange_Class(sheet);
                LoadTextTemplate(sheet, ListName);
                ClientLoad();
                Table621_1(resolution);
                if (!Table621_2(resolution))
                { Substitute.AddExchange(mark.blok2, e => e.Row.ZeroHeight = true, 0); }
                else
                { Substitute.AddExchange(mark.blok2, "", 0); }
                ResizeHeight(sheet, 3, sheet.LastRowNum);
                Substitute.AddExchange(mark.pay, GeneralSumma.Sum().ToMoney(), 1);
                R621 = GeneralSumma.Sum() > AdditionnTable.GetPeriod.MinLimits;
                Substitute.Exchange();
                ClearRowFromMark(sheet, mark.job);
            }
            #endregion
            #region 644
            ListName = "644";
            resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(ListName));
            if (obj.CanResolution(resolution.ID))
            {                
                GeneralSumma = new List<decimal>();
                sheet = book.CloneSheet(book.GetSheetIndex("Расчёт"));
                book.SetSheetName(book.NumberOfSheets - 1, ListName);
                Substitute = new CellExchange_Class(sheet);
                LoadTextTemplate(sheet, ListName);
                ClientLoad();
                if (Table644(resolution))
                {
                    Substitute.AddExchange(mark.pay, GeneralSumma.Sum().ToMoney(), 1);
                    R644 = GeneralSumma.Sum() > AdditionnTable.GetPeriod.MinLimits;
                    Substitute.Exchange();
                    ClearRowFromMark(sheet, mark.job);
                }
                else
                { book.RemoveSheetAt(book.NumberOfSheets - 1); }
            }
            #endregion
            book.RemoveSheetAt(book.GetSheetIndex("Расчёт"));
            Letter();
            Print("Расчёты платы", "Расчёт платы", EPathPrint.Documents);
        }
        ISheet sheet;

        #region обобщённые
        /// <summary>Заполнения оглавления</summary>
        private void ClientLoad()
        {
            Substitute.AddExchange(mark.client, $"{obj.Client.Detail.FullName}\n{obj.Adres}", 0);
            MonthYearSelect();

            var workers = AdditionnTable.GetWorkers(data.ETypeTemplate.CalculationFees, true,"Расчёт");
            IRow row = SearchRowFromMark(sheet, mark.job);
            foreach (var one in workers)
            {
                row.CopyRowTo(row.RowNum + 1);
                Substitute.AddExchange(mark.job, one.Post, 1);
                Substitute.AddExchange(mark.fio, one.FIO, 1);
            }
        }
        #endregion

        struct mark
        {
            public const string client = "{клиент}";
            public const string fio = "{фио}";
            public const string job = "{должность}";
            public const string pay = "{к оплате}";

            public const string blok1 = "{1 блок}";
            public const string blok2 = "{2 блок}";

            public const string table1 = "{таблица 1}";
            public const string table2 = "{таблица 2}";

            public const string formula = "{формула}";

            public const string adder621 = "{приложение 621-П}";
            public const string adder644= "{приложение 644}";
        }

        #region работа с таблицами
        MyTools.C_TableExcel table;
        /// <summary>Сумма к оплате</summary>
        List<decimal> GeneralSumma;

        #region таблицы 621

        private void Table621_1(Resolution resolution)
        {
            IRow row = SearchRowFromMark(sheet, mark.table1, false);
            if (row == null)
            { throw new Exception("Не найдена метка первой таблицы"); }
            table = new MyTools.C_TableExcel(row.RowNum, row.FirstCellNum, Styles.s_RLTB_CC_T10_W);
            table.columns.AddRange(DefCol);
            table.columns.Add(new MyTools.C_ColumnExcel("Q", 4, 4));
            table.columns.Add(new MyTools.C_ColumnExcel("Мф\n(ФК-ДК)хQ/1000", 5, 5));
            table.columns.Add(new MyTools.C_ColumnExcel("Н", 6, 4));
            table.columns.Add(new MyTools.C_ColumnExcel("Кп", 7, 1));

            Calc621(new Calc_621(_sample, obj, resolution), true);
            Summ(table.MaxRowValue, "7 постановления №621-П");

            CreateTable(sheet, table);
            ResizeWidth(sheet, MyTools.ETypeFormatBook.Vertical, table);
            ResizeHeight(sheet,  table);
        }

        private bool Table621_2(Resolution resolution)
        {
            IRow row = SearchRowFromMark(sheet, mark.table2, false);
            if (row == null)
            { throw new Exception("Не найдена метка второй таблицы"); }
            table = new MyTools.C_TableExcel(row.RowNum, row.FirstCellNum, Styles.s_RLTB_CC_T10_W);
            table.columns.AddRange(DefCol);
            table.columns.Add(new MyTools.C_ColumnExcel("Т", 4, 5, ColumnMarged: 1));
            table.columns.Add(new MyTools.C_ColumnExcel("Q", 6, 4, ColumnMarged: 1));

            if (!Calc621(new Calc_621(_sample, obj, resolution), false))
            { return false; }
            Summ(table.MaxRowValue, "8 постановления №621-П");

            CreateTable(sheet, table,false,false);
            return true;
        }

        private bool Calc621(BaseCalc_Class calc, bool PriceNorm)
        {
            calc.Calc();
            int index = 1;
            BaseCalc_Class.clValue[] noNormValues;
            List<decimal> summa = new List<decimal>();

            if (PriceNorm)
            {
                noNormValues = calc.Value.Where(x => x.PriceNorm != null).OrderBy(x => x.Pollution.Number).ToArray();

                foreach (var vol in _sample.Volumes)
                {
                    foreach (var one in noNormValues)
                    {
                        var volume = calc.Answer.FirstOrDefault(x => x.Key == vol.ID);

                        if(volume.Value == null)
                        { continue; }

                        var value = volume.Value.Summ.FirstOrDefault(x => x.PollutionID == one.Pollution.ID);

                        if (value == null)
                        { continue; }

                        SetDefCol(index, one);

                        table.value.Add(new MyTools.C_ValueCell(calc.FromulaNotTrKf(one), index - 1, 5));

                        table.value.Add(new MyTools.C_ValueCell(Math.Round(one.PriceNorm.Price, 2), index - 1, 6));

                        table.value.Add(new MyTools.C_ValueCell(one.CoefficientValue != null ? Math.Round(one.CoefficientValue.Value, 1) : 1, index - 1, 7));

                        //summa.Add(Math.Round(calc.RunFormula(one), 2, MidpointRounding.AwayFromZero));
                        summa.Add(calc.Answer.First(x => x.Key == vol.ID).Value.Summ.First(x => x.PollutionID == one.Pollution.ID).Summ);
                        table.value.Add(new MyTools.C_ValueCell(summa.Last().ToMoney(), index - 1, 8, style: Styles.s_RLTB_RC_T10_W));

                        index++;
                    }
                }

                table.value.Add(new MyTools.C_ValueCell(_sample.Volumes.Sum(x => x.Value), 0, 4, index - 2));
                table.value.Add(new MyTools.C_ValueCell(summa.Sum().ToMoney(), index - 1, 8, style: Styles.s_RLTB_RC_T10_W));
                GeneralSumma.Add(Math.Round(summa.Sum() * (1 + AdditionnTable.GetPeriod.NDS / 100), 2, MidpointRounding.AwayFromZero));
                table.value.Add(new MyTools.C_ValueCell(GeneralSumma.Last().ToMoney(), index, 8, style: Styles.s_RLTB_RC_T10_W_B));
            }
            else
            {
                noNormValues = calc.Value.Where(x => x.PriceNorm == null).OrderBy(x => x.Pollution.Number).ToArray();

                if (noNormValues.Length == 0)
                { return false; }

                foreach (var vol in _sample.Volumes)
                {
                    foreach (var one in noNormValues)
                    {
                        var answerVolume = calc.Answer.FirstOrDefault(x => x.Key == vol.ID);

                        if (answerVolume.Value != null)
                        {
                            var pollutionValue = answerVolume.Value.Summ.FirstOrDefault(x => x.PollutionID == one.Pollution.ID);

                            if (pollutionValue != null)
                            {
                                SetDefCol(index, one);

                                //table.value.Add(new MyTools.C_ValueCell(calc.RunningFormulaVolume(one, vol.ID), index - 1, 5));
                                summa.Add(pollutionValue.Summ);
                                //summa.Add(Math.Round(calc.RunFormula(one, vol.ID), 2));
                                table.value.Add(new MyTools.C_ValueCell(summa.Last().ToMoney(), index - 1, 8, style: Styles.s_RLTB_RC_T10_W));

                                index++;
                            }
                        }
                    }

                    table.value.Add(new MyTools.C_ValueCell(vol.Value, index - noNormValues.Length - 1, 6, noNormValues.Length - 1, 1));
                    table.value.Add(new MyTools.C_ValueCell(Math.Round(Helpers.LogicHelper.PeiodLogic.FirstModel(vol.PeriodID).Price, 2, MidpointRounding.AwayFromZero), index - noNormValues.Length - 1, 4, noNormValues.Length - 1, 1));
                }

                table.value.Add(new MyTools.C_ValueCell(summa.Sum().ToMoney(), index - 1, 8, style: Styles.s_RLTB_RC_T10_W));
                GeneralSumma.Add(Math.Round(summa.Sum() * (1 + AdditionnTable.GetPeriod.NDS / 100), 2, MidpointRounding.AwayFromZero));
                table.value.Add(new MyTools.C_ValueCell(GeneralSumma.Last().ToMoney(), index, 8, style: Styles.s_RLTB_RC_T10_W_B));
            }

            return true;
        }

        #endregion

        #region таблица 644

        private bool Table644(Resolution resolution)
        {
            IRow row = SearchRowFromMark(sheet, mark.table1, false);
            if (row == null)
            { throw new Exception("Не найдена метка первой таблицы"); }
            table = new MyTools.C_TableExcel(row.RowNum, row.FirstCellNum, Styles.s_RLTB_CC_T10_W);
            table.columns.AddRange(DefCol);

            table.columns.Add(new MyTools.C_ColumnExcel("T", 4, 2));
            table.columns.Add(new MyTools.C_ColumnExcel("Q", 5, 4, 0, 1));
            table.columns.Add(new MyTools.C_ColumnExcel("", 6, 2));
            table.columns.Add(new MyTools.C_ColumnExcel("K", 7, 5));

            BaseCalc_Class calc = new Calc_644(_sample, obj, resolution);
            var resultCalc = calc.Calc();
            if (resultCalc == null || resultCalc.Length == 0)
            { return false; }
            FormulControl(calc);
            Calc644(calc);
            Summ(table.MaxRowValue, "123, 123(1) постановление РФ №644");

            CreateTable(sheet, table);
            ResizeWidth(sheet, MyTools.ETypeFormatBook.Vertical, table);
            ResizeHeight(sheet, table);
            return true;
        }

        private void Calc644(BaseCalc_Class calc)
        {
            int index = 1;
            BaseCalc_Class.clValue[] values;
            List<decimal> summa = new List<decimal>();
            values = calc.Value.OrderBy(x => x.Pollution.Number).ToArray();

            foreach (var vol in _sample.Volumes)
            {
                Period period = Helpers.LogicHelper.PeiodLogic.FirstModel(vol.PeriodID);
                foreach (var value in values)
                {
                    SetDefCol(index, value);

                    decimal K = calc.RunFormula(value).Value;

                    table.value.Add(new MyTools.C_ValueCell(K, index - 1, 7));
                    //summa.Add(calc.Answer.First(x => x.Key == vol.ID).Value.Summ.First(x => x.Key == one._Pollution.ID).Value);
                    summa.Add(calc.Answer.First(x => x.Key == vol.ID).Value.Summ.First(x => x.PollutionID == value.Pollution.ID).Summ);
                    //summa.Add(Math.Round(K * (decimal)vol.Value * period.Price, 2, MidpointRounding.AwayFromZero));
                    table.value.Add(new MyTools.C_ValueCell(summa.Last().ToMoney(), index - 1, 8, style: Styles.s_RLTB_RC_T10_W));
                    index++;
                }

                table.value.Add(new MyTools.C_ValueCell(period.Price.ToMoney(), index - values.Length - 1, 4, values.Length - 1));
                table.value.Add(new MyTools.C_ValueCell(vol.Value, index - values.Length - 1, 5, values.Length - 1, 1));
            }

            table.value.Add(new MyTools.C_ValueCell(summa.Sum().ToMoney(), index - 1, 8, style: Styles.s_RLTB_RC_T10_W));
            GeneralSumma.Add(Math.Round(summa.Sum() * (1 + AdditionnTable.GetPeriod.NDS / 100), 2, MidpointRounding.AwayFromZero));
            table.value.Add(new MyTools.C_ValueCell(GeneralSumma.Last().ToMoney(), index, 8, style: Styles.s_RLTB_RC_T10_W_B));
        }

        private void FormulControl(BaseCalc_Class calc)
        {
            string formula = string.Empty;
            foreach (var one in PollutionBase_Class.AllCalculationFormul.Where(x => x.Label.Length > 0).OrderBy(x => x.Number).GroupBy(x => x.Label))
            {
                foreach (var pol in one.Select(x => x.PollutionID))
                {
                    if (calc.Value.Select(x => x.Pollution.ID).Contains(pol))
                    {
                        formula += $" + {one.Key}";
                        goto Next;
                    }
                }
                Substitute.AddExchange(one.Key, (e) => e.Row.ZeroHeight = true, 1);
                Next:;
            }
            Substitute.AddExchange(mark.formula, formula.Substring(3), 1);
        }

        #endregion

        private void SetDefCol(int Index, BaseCalc_Class.clValue value)
        {
            table.value.Add(new MyTools.C_ValueCell(Index, Index - 1, 0));
            table.value.Add(new MyTools.C_ValueCell(value.Pollution.FullName.TextToUpper(), Index - 1, 1, style: Styles.s_RLTB_LC_T10_W));
            table.value.Add(new MyTools.C_ValueCell(value.Value, Index - 1, 2));
            table.value.Add(new MyTools.C_ValueCell(value.ValueNorm.ToRound, Index - 1, 3));
        }

        private void Summ(int Row, string Punkt)
        {
            Row--;
            table.value.Add(new MyTools.C_ValueCell($"Итого согласно п. {Punkt}:", Row, 0, 0, 7, Styles.s_RLTB_LC_T10_W));
            table.value.Add(new MyTools.C_ValueCell($"Итого с НДС согласно п. {Punkt}:", Row + 1, 0, 0, 7, Styles.s_RLTB_LC_T10_W));
        }

        private MyTools.C_ColumnExcel[] DefCol => new MyTools.C_ColumnExcel[]
                {
                new MyTools.C_ColumnExcel("№\nп/п", 0, 0),
                new MyTools.C_ColumnExcel("Наименование\n загрязняющего\n вещества", 1, 6),
                new MyTools.C_ColumnExcel("ФК", 2, 3),
                new MyTools.C_ColumnExcel("ДК", 3, 3),
                new MyTools.C_ColumnExcel("Сумма, руб", 8, 5)
                };
        #endregion

        #region Письмо

        private void Letter()
        {
            sheet = book.GetSheet("Письмо");
            if (!R621 && !R644)
            {
                book.RemoveSheetAt(book.GetSheetIndex("Письмо"));
                return;
            }
            Substitute = new CellExchange_Class(sheet);
            ClientName(obj.Client);
            ObjectAdres(obj,true);
            MonthYearSelect();

            NumberFolder(obj.NumberFolder);

            var worker = AdditionnTable.GetSigner(data.ETypeTemplate.CalculationFees, "Письмо");
            Substitute.AddExchange(mark.job, worker.Post, 1);
            Substitute.AddExchange(mark.fio, worker.FIO, 1);

            int Radder621 = SearchRowFromMark(sheet, mark.adder621).RowNum;

            Resolution resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621"));
            ResolutionClarify(resolution, R621);
            if (R621)
            { Substitute.AddExchange(mark.adder621, GetNormDoc(resolution), 1); }
            else
            { Substitute.AddExchange(mark.adder621, e => { e.Row.ZeroHeight = true; }, 1); }

            resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644"));
            ResolutionClarify(resolution, R644);
            if (R644)
            { Substitute.AddExchange(mark.adder644, GetNormDoc(resolution), 1); }
            else
            { Substitute.AddExchange(mark.adder644, e => { e.Row.ZeroHeight = true; }, 1); }
            Substitute.Exchange();
            ResizeHeight(sheet, Radder621, Radder621 + 3);
        }

        private string GetNormDoc(Resolution resolution)
        {
            var query = G.NormDoc.QUERRY()
                .SHOW
                .WHERE
                .C(C.NormDoc.Resolution, resolution.ID);
            var volumes = _sample.Volumes.ToArray();

            if (volumes.Length > 1)
            {
                query.AND.OB().C(C.NormDoc.Volume, _sample.Volumes.First().ID);
                for (int i = 1; i < volumes.Length; i++)
                { query.OR.C(C.NormDoc.Volume, volumes[i].ID); }
                query.CB();
            }
            else
            { query.AND.C(C.NormDoc.Volume, _sample.Volumes.First().ID); }
            query.DO();

            string result = resolution.GetResolutionClarify.Note+'\n';
            NormDoc[] normDocs = new NormDoc[G.NormDoc.Rows.Count];
            for (int i = 0; i < normDocs.Length; i++)
            {
                normDocs[i] = new NormDoc(G.NormDoc.Rows.GetID(i));
                result += normDocs[i].Text + '\n';
            }
            return result.Trim('\n');
        }

        #endregion
    }
}