using AutoTable;
using MAC_2.Calc;
using MAC_2.Employee.Mechanisms;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MAC_2.Model;

namespace MAC_2.PrintForm
{
    public class Registry_Print_Class : BasePrint
    {
        public Registry_Print_Class() : base(data.ETypeTemplate.Registry)
        {        }

        protected override void internalStart()
        {
            #region 644
            book = TemplateStorage.WorkBook;
            if (CreateStyle())
            {
                name = "644";
                sheet = book.GetSheet("Реестр");
                Progress_ = new Progress_Form($"Сбор по {name}", new CreateTable());
                Progress_.ShowDialog();
                LoadCap(name);
                Print("Реестр", $"Реестр {name} {MyTools.YearMonth_From_YM(DateControl_Class.SelectMonth)}", EPathPrint.Documents);
            }
            #endregion

            #region 621
            book = TemplateStorage.WorkBook;
            if (CreateStyle())
            {
                name = "621";
                sheet = book.GetSheet("Реестр");
                Progress_ = new Progress_Form($"Сбор по {name}",new CreateTable());
                Progress_.ShowDialog();
                LoadCap(name);
                Print("Реестр", $"Реестр {name} {MyTools.YearMonth_From_YM(DateControl_Class.SelectMonth)}", EPathPrint.Documents);
            }
            #endregion            
        }
        static string name;
        Progress_Form Progress_;
        static ISheet sheet;
        private void LoadCap(string name)
        {
            Substitute = new CellExchange_Class(sheet);
            Substitute.AddExchange("{месяц}", MyTools.Month_From_M_C_R(MyTools.M_From_YM(DateControl_Class.SelectMonth), Reg: MyTools.ERegistor.ToLower), 0);            
            Substitute.AddExchange("{год}", DateControl_Class.SelectYear, 0);
            MonthYear();

            Worker workers = AdditionnTable.GetSigner(data.ETypeTemplate.Registry, "директор");
            Substitute.AddExchange("{должность директора}", workers.Post, 0);
            Substitute.AddExchange("{фио директора}", workers.rFIO, 0);
            workers = AdditionnTable.GetSigner(data.ETypeTemplate.Registry, "подписывающий");
            Substitute.AddExchange("{должность}", workers.Post, 0);
            Substitute.AddExchange("{фио}", workers.FIO, 0);
            LoadTextTemplate(sheet, name, 1);
            Substitute.AddExchange("{итоговая сумма}", summa.ToMoney(), 0);
            Substitute.Exchange();
        }
        static BaseCalc_Class calc;
        static decimal summa;

        class CreateTable : Progress_Form.AObject
        {
            public CreateTable() : base(false)
            { TitleText = "Прогруженно "; }       
            protected override bool Do()
            {
                summa = 0;
                IRow row = SearchRowFromMark(sheet, "{таблица}", false);
                if (row == null)
                { throw new Exception("Не найдена метка первой таблицы"); }
                table = new MyTools.C_TableExcel(row.RowNum, row.FirstCellNum, Styles.s_RLTB_CC_T10_W);
                table.columns.Add(new MyTools.C_ColumnExcel("№\nп/п", 0, 1));
                table.columns.Add(new MyTools.C_ColumnExcel("Наименование предприятия", 1, 50));
                table.columns.Add(new MyTools.C_ColumnExcel("ИНН", 2, 10));
                table.columns.Add(new MyTools.C_ColumnExcel("Сумма с\nучётом НДС", 3, 10));
                shows = new List<ThisShow>();
                int sam = 0;

                var samples = Helpers.LogicHelper.SampleLogic.Find(DateControl_Class.SelectMonth, 1);
                samples = samples.Where(x => x.SelectionWells.Any()).ToArray();
                var count = samples.Count();

                foreach (var sample in samples)
                {
                    Action(count, ++sam);

                    var sw = sample.SelectionWells.First();

                    Client client = sw.Objecte.Client;
                    Objecte obj = client.Objects.First(x => x.ID == sw.ObjectID);
                    switch (name)
                    {
                        case "621":
                            {
                                calc = new Calc_621(sample, obj, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(name)));
                                break;
                            }
                        case "644":
                            {
                                calc = new Calc_644(sample, obj, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(name)));
                                break;
                            }
                    }
                    calc.Calc();
                    if (calc.Answer == null || calc.Answer.Sum(x => x.Value.SummNDS) < AdditionnTable.GetPeriod.MinLimits)
                    { continue; }
                    shows.Add(new ThisShow(client.Detail.FullName, client.INN, calc.Answer.Sum(x => x.Value.SummNDS), obj.OrderDistrict));
                }
                int index = 0;
                if(shows.Count==0)
                {
                    MessageBox.Show("Не найдены превышения!");
                    return false;
                }
                //shows = shows.OrderBy(x => x.Order).ThenByDescending(x => x.Summa).ToList();
                shows = shows.OrderByDescending(x => x.Summa).ToList();
                foreach (var one in shows)
                {
                    table.value.Add(new MyTools.C_ValueCell(index + 1, index, 0, style: Styles.s_RLTB_RC_T10_W));
                    table.value.Add(new MyTools.C_ValueCell(one.Name.StringDivision(70), index, 1, style: Styles.s_RLTB_LC_T10_W));
                    table.value.Add(new MyTools.C_ValueCell(one.INN, index, 2, style: Styles.s_RLTB_LC_T10_W));
                    table.value.Add(new MyTools.C_ValueCell(one.Summa.ToMoney(), index, 3, style: Styles.s_RLTB_RC_T10_W));
                    summa += one.Summa;
                    index++;
                }
                CreateTable(sheet, table);
                ResizeWidth(sheet, MyTools.ETypeFormatBook.Vertical, table);
                ResizeHeight(sheet, table);
                return true;
            }
        }
        static MyTools.C_TableExcel table;
        static List<ThisShow> shows;
        /*
        private static void CreateTable()
        {
            foreach (var one in PollutionBase_Class.AllSample)
            {
                var sw = PollutionBase_Class.ListSelectionWell.First(x => x.SampleID == one.ID);
                Client client = AllClients.ClientAtIDObjecte(sw.ObjectID);
                Objecte obj = client.Objects.First(x => x.ID == sw.ObjectID);
                switch (name)
                {
                    case "621":
                        {
                            calc = new Calc_621(one, obj, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(name)));
                            break;
                        }
                    case "644":
                        {
                            calc = new Calc_644(one, obj, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(name)));
                            break;
                        }
                }
                calc.Calc();
                if (calc.Answer == null || calc.Answer.Sum(x => x.Value.Summ) < AdditionnTable.GetPeriod.MinLimits)
                { continue; }
                shows.Add(new ThisShow(client.GetDetail.FullName, client.INN, calc.Answer.Sum(x => Math.Round(x.Value.SummNDS, 2, MidpointRounding.AwayFromZero)), obj.OrderDistrict));
            }
            int index = 0;
            shows = shows.OrderBy(x => x.Order).ThenByDescending(x => x.Summa).ToList();
            foreach (var one in shows)
            {
                table.value.Add(new MyTools.C_ValueCell(index + 1, index, 0, s_RLTB_RC_T10_W));
                table.value.Add(new MyTools.C_ValueCell(one.Name.StringDivision(70), index, 1, s_RLTB_LC_T10_W));
                table.value.Add(new MyTools.C_ValueCell(one.INN, index, 2, s_RLTB_LC_T10_W));
                table.value.Add(new MyTools.C_ValueCell(one.Summa.ToMoney(), index, 3, s_RLTB_RC_T10_W));
                summa += one.Summa;
                index++;
            }
            CreateTable(sheet, table);
            ResizeWidth(sheet, MyTools.ETypeFormatBook.Vertical, table);
            ResizeHeight(sheet, table);
        }
        */
        class ThisShow
        {
            public ThisShow(string Name, string INN, decimal Summa, int Order)
            {
                this.Name = Name;
                this.INN = INN;
                this.Summa = Summa;
                this.Order = Order;
            }
            public readonly string Name;
            public readonly string INN;
            public readonly decimal Summa;
            public readonly int Order;
        }
    }
}