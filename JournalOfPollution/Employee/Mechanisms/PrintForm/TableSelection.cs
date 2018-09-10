using AutoTable;
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
    public class TableSelection: MyTools.C_A_Base_Excel
    {
        public TableSelection(Objecte obj, Sample sample)
        {
            this.obj = obj;
            this.sample = sample;
        }
        #region работа с таблицей
        Objecte obj;
        Sample sample;
        double MonthVolume;
        public MyTools.C_TableExcel table;
        /// <summary>Отрисовка таблицы</summary>
        public void CreateTable(ICell cell, SelectionWell selectionWell)
        {
            table = CreateColumn(cell);

            for (int i = 1; i < 16; i++)
            { table.value.Add(new MyTools.C_ValueCell(i, 1, i - 1)); }

            table.value.Add(new MyTools.C_ValueCell("мг / л", 0, 6));
            table.value.Add(new MyTools.C_ValueCell("мг / л", 0, 7));
            table.value.Add(new MyTools.C_ValueCell("мг / л", 0, 10));
            table.value.Add(new MyTools.C_ValueCell("тонн", 0, 8));
            table.value.Add(new MyTools.C_ValueCell("тонн", 0, 9));
            table.value.Add(new MyTools.C_ValueCell("тонн", 0, 11));

            ValueSelection[] valueSelections = selectionWell.ValueSelections.Where(x => x.Value > 0).ToArray();

            table.value.Add(new MyTools.C_ValueCell(MyTools.StringDate_From_YMDHMS(selectionWell.YMDHM, MyTools.EInputDate.YMDHM, MyTools.EInputDate.YMD), 2, 0, valueSelections.Length - 1));
            MonthVolume = obj.GetMidMonthVolume(DateControl_Class.SelectYear - 1).Volume;
            table.value.Add(new MyTools.C_ValueCell(MonthVolume, 2, 1, valueSelections.Length - 1));
            int row = 2;

            Declaration declair = selectionWell.Well.Declaration;

            foreach (var valueSelection in valueSelections)
            {
                Pollution pollution = valueSelection.Pollution;
                table.value.Add(new MyTools.C_ValueCell(pollution.FullName.TextToUpper().StringDivision(4), row, 2, style: Styles.s_RLTB_LC_T10_W));
                if (pollution.Key > 0)
                { table.value.Add(new MyTools.C_ValueCell(pollution.Key, row, 3)); }
                table.value.Add(new MyTools.C_ValueCell(valueSelection.ValueRound.KillZero(), row, 4));
                table.value.Add(new MyTools.C_ValueCell(((decimal)MonthVolume * valueSelection.Value).ToMath(6, 4, 1), row, 5, style: Styles.s_RLTB_RC_T10_W));
                table.value.AddRange(GetResolution(valueSelection, "621", row, 6, 8, 12));
                table.value.AddRange(GetResolution(valueSelection, "644", row, 7, 9, 13));

                DeclarationValue declarationValue = declair != null ? declair.DeclarationValues.FirstOrDefault(x => x.PollutionID == pollution.ID) : null;
                if (declarationValue != null)
                {
                    decimal result = 0;
                    if (pollution.HasRange)
                    { table.value.Add(new MyTools.C_ValueCell($"{declarationValue.FromRound.KillZero()}-{declarationValue.ToRound.KillZero()}", row, 10)); }
                    else
                    {
                        table.value.Add(new MyTools.C_ValueCell(declarationValue.ToRound.KillZero(), row, 10));
                        result = (decimal)MonthVolume * declarationValue.To;
                        if (result > 0)
                        { table.value.Add(new MyTools.C_ValueCell(result.ToMath(6, 4, 1), row, 11, style: Styles.s_RLTB_RC_T10_W)); }
                        result = Math.Round(valueSelection.Value / declarationValue.To, 1);
                        if (result > 1)
                        { table.value.Add(new MyTools.C_ValueCell(result.ToMoney(null, 1), row, 14)); }
                    }
                }
                row++;
            }

            CreateTable(cell.Sheet, table);
            ResizeWidth(cell.Sheet, MyTools.ETypeFormatBook.Horizontal, table);
            ResizeHeight(cell.Sheet, table);
        }
        private List<MyTools.C_ValueCell> GetResolution(ValueSelection one, string resolutionName, int row, int Col1, int Col2, int Col3)
        {
            Resolution resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(resolutionName));
            List<MyTools.C_ValueCell> result = new List<MyTools.C_ValueCell>();
            decimal value = 0;
            if (obj.CanResolution(resolution.ID))
            {
                uint IDUnit = resolution.CurtName.Contains("621") ? one.SelectionWell.Well.UnitID : 0;
                ValueNorm VN = PollutionBase_Class.GetValueNorm(resolution.ID, one.Pollution.ID, IDUnit);
                if (VN != null)
                {
                    if (one.Pollution.HasRange)
                    {
                        if (VN.FromRound > 0 && VN.ToRound > 0)
                        { result.Add(new MyTools.C_ValueCell($"{VN.FromRound.KillZero()}-{VN.ToRound.KillZero()}", row, Col1)); }
                    }
                    else
                    {
                        var toValue = VN.ToRound;

                        if (toValue > 0)
                        {
                            table.value.Add(new MyTools.C_ValueCell(VN.ToRound.KillZero(), row, Col1));
                            value = (decimal)MonthVolume * VN.To;
                            if (value > 0)
                            { table.value.Add(new MyTools.C_ValueCell(value.ToMath(6, 4, 1), row, Col2, style: Styles.s_RLTB_RC_T10_W)); }
                            value = Math.Round(one.Value / VN.To, 1);
                            if (value > 1)
                            { table.value.Add(new MyTools.C_ValueCell(value.ToMoney(null, 1), row, Col3)); }
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>Создаю колонки для таблицы</summary>
        /// <param name="cell">Стартовая ячейка</param>
        private MyTools.C_TableExcel CreateColumn(ICell cell)
        {
            MyTools.C_TableExcel result = new MyTools.C_TableExcel(cell.RowIndex, cell.ColumnIndex, Styles.s_RLTB_CC_T10_W);
            result.columns.Add(new MyTools.C_ColumnExcel("Дата отбора проб", 0, 2, 1));
            //result.columns.Add(new MyTools.C_ColumnExcel("Место отбора проб", 1, 6, 1));
            result.columns.Add(new MyTools.C_ColumnExcel("Расход сточных вод (м^3/мес.)".SymbolConverter(), 1, 1, 1));
            result.columns.Add(new MyTools.C_ColumnExcel("Наименование загрязняющих веществ", 2, 6, 1));
            result.columns.Add(new MyTools.C_ColumnExcel("Код загрязн. вещества", 3, 0.6, 1));
            result.columns.Add(new MyTools.C_ColumnExcel("Фактическая концентрация загрязняющих веществ(мг/л)", 4, 4.5, 1));
            result.columns.Add(new MyTools.C_ColumnExcel("Фактическай сброс загрязняющ. веществ(тонн)", 5, 5, 1));
            result.columns.Add(new MyTools.C_ColumnExcel("Норматив допустимого сброса(лимит на сброс)", 6, 0.7, 0, 3));
            result.columns.Add(new MyTools.C_ColumnExcel(string.Empty, 7, 0.7));
            result.columns.Add(new MyTools.C_ColumnExcel(string.Empty, 8, 5));
            result.columns.Add(new MyTools.C_ColumnExcel(string.Empty, 9, 5));
            result.columns.Add(new MyTools.C_ColumnExcel("Сведения декларации о составе и свойствах сточных вод", 10, 1, 0, 1));
            result.columns.Add(new MyTools.C_ColumnExcel(string.Empty, 11, 4));
            result.columns.Add(new MyTools.C_ColumnExcel("Кратность превышения", 12, 0.7, 0, 2));
            result.columns.Add(new MyTools.C_ColumnExcel(string.Empty, 13, 0.7));
            result.columns.Add(new MyTools.C_ColumnExcel(string.Empty, 14, 0.7));
            result.MinHeighColumn = 900;
            return result;
        }

        public void Signature(ICell cell , CellExchange_Class Substitute)
        {
            //cell.Sheet.CopyRow(cell.RowIndex, cell.RowIndex + 1);
            //Substitute.AddExchange("{пример}", "*12 345 E-6 читай 12 345 x 10^-^6".SymbolConverter(), 1);
            Resolution resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621"));
            if (obj.CanResolution(resolution.ID))
            {
                Substitute.AddExchange("{пример}", "колонки 7, 9, 13 согласно: " + resolution.GetResolutionClarify.FullName, 1);
                cell.Sheet.CopyRow(cell.RowIndex, cell.RowIndex + 1);
            }
            resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644"));
            if (obj.CanResolution(resolution.ID))
            {
                Substitute.AddExchange("{пример}", "колонки 8, 10, 14 согласно: " + resolution.GetResolutionClarify.FullName, 1);
                cell.Sheet.CopyRow(cell.RowIndex, cell.RowIndex + 1);
            }
            Substitute.AddExchange("{пример}", "колонка 15=5/11", 1);
        }

        #endregion
    }
}
