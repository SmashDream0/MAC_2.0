using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoTable;
using NPOI.SS.UserModel;
using MAC_2.Model;

namespace MAC_2.Employee.Mechanisms
{
    public class SyncFromCalc : MyTools.C_A_BaseLoad_Excel
    {
        public SyncFromCalc()
        {
            DG.CanUserDeleteRows =
            DG.CanUserAddRows=
            DG.AutoGenerateColumns = false;
            message = string.Empty;
        }

        protected override void LoadBook(string path)
        {
            path = @"C:\АСУП-Водоканал-Астрахань\JCalc\Синхронизация\ПДК.xlsx";
            if (!File.Exists(path))
            {
                MessageBox.Show($"Файл по пути \"{path}\" не найден");
                path = null;
            }
            base.LoadBook(path);
        }
        
        progressSetAdd aObject;
        /// <summary>Сообщение об окончании</summary>
        static string message;
        /// <summary>Загрузка принудительно</summary>
        static bool ForceDownload;
        class progressSetAdd : Progress_Form.AObject
        {
            public progressSetAdd() : base(true)
            { _MaxCount = shows.Count; }

            protected override bool Do()
            {
                var values = PollutionBase_Class.GetValuesFromYM(YM);
                var select = PollutionBase_Class.GetSelectionWellFromYM(YM);
                int act = 0;
                foreach (var one in shows)
                {
                    Action(++act);
                    var sw = select.FirstOrDefault(x => x.Number == one.select);
                    if (sw == null)
                    {
                        message += $"{one.select}-C-{MyTools.YearMonth_From_YM(YM, DivisionSymbol: "/")}, ";
                        continue;
                    }
                    foreach (var val in one.value)
                    {
                        var value = values.FirstOrDefault(x => x.NumberSel == one.select && x.Pollution.UniqueKey == val.Key);
                        if (value != null)
                        {
                            if (value.Value != val.Value)
                            {
                                if (!ForceDownload)
                                { compares.Add(new Compare(val.Key, value.Value, val.Value, sw)); }
                                else
                                { value.Value = val.Value; }
                            }
                        }
                        else
                        {
                            MyTools.AddRowFromTable(G.ValueSelection,
                                new KeyValuePair<int, object>(C.ValueSelection.Pollution, pollutions.First(x => x.UniqueKey == val.Key).ID),
                                new KeyValuePair<int, object>(C.ValueSelection.SelectionWell, sw.ID),
                                new KeyValuePair<int, object>(C.ValueSelection.Value, val.Value));
                        }
                    }
                }
                return true;
            }
        }
        static List<Compare> compares;
        class Compare: MyTools.C_A_PrintBase
        {
            public Compare(string UniqueKey, decimal Wes, decimal Become, SelectionWell sw)
            {
                this.UniqueKey = UniqueKey;
                this.Wes = Wes;
                this.Become = Become;
                this.sw = sw;
                Select = true;
            }
            public override uint ID => sw.ID;
            public override Dictionary<string, object> Values
            {
                get
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    result.Add(ColCompare.select, Number);
                    result.Add(ColCompare.wes, Wes.ToString());
                    result.Add(ColCompare.become, Become.ToString());
                    return result   ;
                }
            }
            public string Number => $"{sw.Number}-C-{MyTools.YearMonth_From_YM(YM, DivisionSymbol: "/")}";
            public string UniqueKey { get; }
            public decimal Wes { get; }
            public decimal Become { get; }
            public bool Select { get; set; }
            public readonly SelectionWell sw;
        }

        struct ColCompare
        {
            public const string replace = "На замену?";
            public const string select = "Отбор";
            public const string wes = "Было";
            public const string become = "Заменить на";
        }

        public override bool StartLoad()
        {
            aObject = new progressSetAdd();
            compares = new List<Compare>();
            _progress = new Progress_Form(aObject);
            _progress.ShowDialog();
            MessageBox.Show("Загрузка окончена!");
            if (message != string.Empty)
            { MessageBox.Show($"Не найденные записи в базе: {message.Trim(' ', ',')}"); }
            if (compares.Count > 0)
            {
                CustomMessage_Window customMessage = new CustomMessage_Window(new CustomMessage_Window.SettingWindow("Показатели не совпавшие по значениям", WindowStartupLocation.CenterScreen, CanResize: true));
                DataGrid DG = new DataGrid();

                List<MyTools.C_DGColumn_Bind> list = new List<MyTools.C_DGColumn_Bind>();
                list.Add(new MyTools.C_DGColumn_Bind(ColCompare.replace, MyTools.E_TypeColumnDG.CheckBox, "Select", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay)));
                list.Add(new MyTools.C_DGColumn_Bind(ColCompare.select, MyTools.E_TypeColumnDG.Text, "Number", setting));
                list.Add(new MyTools.C_DGColumn_Bind(ColCompare.wes, MyTools.E_TypeColumnDG.Text, "Wes", setting));
                list.Add(new MyTools.C_DGColumn_Bind(ColCompare.become, MyTools.E_TypeColumnDG.Text, "Become", setting));
                foreach (var one in list)
                { DG.Columns.Add(one.Column); }
                DG.AutoGenerateColumns = false;
                DG.ItemsSource = compares;
                customMessage.ShowControl.SetFromGrid(DG);

                Button bPrint = new Button();
                bPrint.Content = "Распечатать";
                bPrint.Click += (sender, e) =>
                   { MyTools.PrintList(compares, $"{Directory.GetCurrentDirectory()}\\Отчёты\\Синхронизация",$"Отчёт{DateTime.Now.ToShortDateString()}"); };
                customMessage.wpButtons.Children.Add(bPrint);

                Button bForceDownload = new Button();
                bForceDownload.Content = "Загрузить принудительно";
                bForceDownload.Click += (sender, e) =>
                  {
                      ForceDownload = true;
                      StartLoad();
                      if (compares.Count > 0)
                      {
                          DG.ItemsSource = null;
                          DG.ItemsSource = compares;
                      }
                      else
                      { bForceDownload.IsEnabled = false; }
                  };
                customMessage.wpButtons.Children.Add(bForceDownload);

                customMessage.ShowDialog();
            }
            CanReStart();
            return true;
        }

        #region Колонки и их обработка

        struct col
        { public const string select = "уникальные номера"; }

        static IEnumerable<Pollution> pollutions;
        protected override void GetColumn()
        {
            column = new string[]
            { col.select };
            pollutions = PollutionBase_Class.AllPolutions.Where(x => x.UniqueKey.Length > 0);
            columnDop = pollutions.Select(x => x.UniqueKey).ToArray();
        }
        MyTools.C_Setting_DGColumn setting;
        protected override void GetDGColumn()
        {
            setting = new MyTools.C_Setting_DGColumn();
            List<MyTools.C_DGColumn_Bind> list = new List<MyTools.C_DGColumn_Bind>();
            list.Add(new MyTools.C_DGColumn_Bind("Отбор", MyTools.E_TypeColumnDG.Text, "select", setting));
            foreach (var one in pollutions)
            { list.Add(new MyTools.C_DGColumn_Bind(one.CurtName, MyTools.E_TypeColumnDG.Text, $"value[{one.UniqueKey}]", setting)); }
            ColumnsFromDG = list.ToArray();
        }

        #endregion

        protected override void LoadMenu()
        { InstructionsMessage_Class.LoadInstructions(ThisMenu, data.ETypeInstruction.LoadSyncJPC); }
        public override bool CanReStart()
        { return false; }
        static List<show> shows;
        class show
        {
            public show(int select, Dictionary<string, decimal> value)
            {
                this.select = select;
                this.value = value;
            }
            public int select { get; internal set; }
            public Dictionary<string, decimal> value { get; internal set; }
        }
        static int YM;
        protected override void Start()
        {
            string dat = SearthCellFromMark("Выгрузка от:", false).StringCellValue;
            YM = (int)MyTools.StringDATE_In_intDate(dat.Substring(dat.IndexOf(':')+1), MyTools.EInputDate.YM, MyTools.EInputDate.YM);
            TextBlock text = new TextBlock();
            text.Background = Brushes.LightCoral;
            text.FontSize = 20;
            text.Text = dat;
            wpText.Children.Add(text);

            shows = new List<show>();
            progressOpenFile openFile = new progressOpenFile(sheet.LastRowNum - StartRow, new  Func<int, bool>(
                RowIndex =>
                {
                        IRow row = sheet.GetRow(RowIndex);
                        int select = 0;
                        Dictionary<string, decimal> value = new Dictionary<string, decimal>();
                        foreach (var one in ColumnsBook)
                        {
                            if (one.Key == col.select)
                            { select = row.GetCell(one.Value).ToString().TryParseInt(); }
                            else
                            {
                                if (one.Value > 0 && row.GetCell(one.Value).ToString() != "0")
                                { value.Add(one.Key, row.GetCell(one.Value).ToString().TryParseDecimal()); }
                            }
                        }
                    if (select != 0)
                    { shows.Add(new show(select, value)); }
                    return true;
                }));
            _progress = new Progress_Form(openFile);
            _progress.ShowDialog();
            DG.ItemsSource = shows;
        }
    }
}