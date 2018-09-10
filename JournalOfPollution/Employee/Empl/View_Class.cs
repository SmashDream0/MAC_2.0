using AutoTable;
using MAC_2.Calc;
using MAC_2.Employee.Mechanisms;
using MAC_2.PrintForm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MAC_2.Model;

namespace MAC_2.Employee.Empl
{
    public class View_Class : C_Search_Class
    {
        public View_Class(DataGrid DG, WrapPanel SearchWP) : base(DG, SearchWP)
        {
            PollutionBase_Class.LoadSample();

            DG.ContextMenuOpening += DG_ContextMenuOpening;
            CreateColumn();
            DG.MouseDoubleClick += (sender, e) =>
            {
                if (DG.SelectedIndex != -1)
                {
                    if ((DG.Items[DG.SelectedIndex] as SHOW).ID == 0)
                    { return; }
                    new EditValue_Window((DG.Items[DG.SelectedIndex] as SHOW).SelectionWell).ShowDialog();
                }
            };
            MyTools.BindExp_DG(DG);
            CanEdit = !DateControl_Class.ThisMonthDay;
            DG.ContextMenu = new ContextMenu();
        }
        /// <summary>Объект может иметь несколько объёмов</summary>
        static bool volumeTwo;
        BasePrint print;
        private void DG_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (DG.SelectedIndex == -1)
            { return; }

            var item = (DG.Items[DG.SelectedIndex] as SHOW);
            Objecte obj = AllClients.ObjecteAtWell(item.ID);
            List<SelectionWell> SW = new List<SelectionWell>();

            foreach (SHOW one in DG.SelectedItems)
            { SW.Add(one.SelectionWell); }

            Sample sample = SW.First().Sample;
            DG.ContextMenu.Items.Clear();

            MenuItem miCalc = new MenuItem { Header = "Расчёт" };
            miCalc.Click += (senderC, eC) =>
              {
                  if (item.Volume.Length > 0)
                  { new CalculationFees_Print_Class(sample); }
                  else
                  { MessageBox.Show("Объём не задан!"); }
              };

            DG.ContextMenu.Items.Add(miCalc);

            MenuItem miExtract = new MenuItem { Header = "Выписка" };
            miExtract.Click += (senderE, eE) =>
            {
                print = new Extract_Print_Class(sample);
                print.Start();
            };
            DG.ContextMenu.Items.Add(miExtract);

            MenuItem miProtocol = new MenuItem { Header = "Протокол" };
            miProtocol.Click += (senderE, eE) =>
            {
                print = new Protocol_Print_Class(SW.ToArray());
                print.Start();
            };
            DG.ContextMenu.Items.Add(miProtocol);

            MenuItem miInspectionResult = new MenuItem { Header = "Результат контроля" };
            miInspectionResult.Click += (senderE, eE) =>
            {
                print = new InspectionResult_Print_Class(obj.ID);
                print.Start();
            };
            DG.ContextMenu.Items.Add(miInspectionResult);
        }
        struct ThisColumn
        {
            public const string ID = "ID";
            public const int IDn = 0;
            public const string Number = "Номер";
            public const int Numbern = IDn + 1;
            public const string Company = "Организация";
            public const int Companyn = Numbern + 1;
            public const string Well = "Колодец";
            public const int Welln = Companyn + 1;
            public const string Folder = "Папка";
            public const int Foldern = Welln + 1;
            public const string Status = "Состояние";
            public const int Statusn = Foldern + 1;
            public const string DateTime = "Дата время";
            public const int DateTimen = Statusn + 1;
            public const string Volume = "Объём";
            public const int Volumen = DateTimen + 1;
            public const string Summ = "Сумма";
            public const int Summn = Volumen + 1;
            public const int Pollution = Summn + 1;
        }
        column[] columns;
        class column : MyTools.C_DGColumn_Bind
        {
            public column(string Header, string path, int Number, MyTools.C_Setting_DGColumn setting) : base(Header, MyTools.E_TypeColumnDG.Text, path, setting)
            {
                this.Number = Number;
                this.BIND = path;
            }
            public readonly int Number;
            public readonly string BIND;
        }
        bool CanEdit;

        /// <summary>Создать колонки</summary>
        public void CreateColumn()
        {
            MyTools.C_Setting_DGColumn settingOneWay = new MyTools.C_Setting_DGColumn();
            List<column> Result = new List<column>();
            Result.Add(new column(ThisColumn.Number, "Number", ThisColumn.Numbern, settingOneWay));
            Result.Add(new column(ThisColumn.Company, "Company", ThisColumn.Companyn, settingOneWay));
            Result.Add(new column(ThisColumn.Well, "Well", ThisColumn.Welln, settingOneWay));
            Result.Add(new column(ThisColumn.Folder, "Folder", ThisColumn.Foldern, settingOneWay));
            Result.Add(new column(ThisColumn.Status, "Status", ThisColumn.Statusn, settingOneWay));
            Result.Add(new column(ThisColumn.DateTime, "DateTime", ThisColumn.DateTimen, settingOneWay));
            Result.Add(new column(ThisColumn.Volume, "Volume", ThisColumn.Volumen, new MyTools.C_Setting_DGColumn(BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged, CanEdit)));
            Result.Add(new column(ThisColumn.Summ, "Summ", ThisColumn.Summn, settingOneWay));
            int now = Result.Count;

            {
                var pollutions = PollutionBase_Class.AllPolutions;

                Result.AddRange(pollutions.Select(x => new column(x.CurtName.SymbolConverter(), $"Values[{x.BindName}].Value", x.Number + now, new MyTools.C_Setting_DGColumn(BindingMode.TwoWay, IsReadOnly: CanEdit))));
            }

            columns = Result.ToArray();
        }

        /// <summary>Перерисовать колонки</summary>
        public void DrawColumns()
        {
            DG.Columns.Clear();
            foreach (var one in columns)
            {
                if (one.Number > 0 && (ColumnOff.Contains(one.Number) || (ColumnOff.Contains(ThisColumn.Pollution) && one.Number >= ThisColumn.Pollution)))
                { continue; }
                DG.Columns.Add(one.Column);
            }
            DG.FrozenColumnCount = 9 - ColumnOff.Count;
            Draw();
        }

        class SHOW : ISearch
        {
            public SHOW(
                SelectionWell selectionWell,
                KeyValuePair<string, decimal>[] summs,
                ValueWork[] Values)
            {
                this.SelectionWell = selectionWell;
                this.client = AllClients.ClientAtWell(ID);
                this.summs = summs;
                this.Values = Values;

                result = new Dictionary<string, string>();
                result.Add(ThisColumn.Number, Number);
                result.Add(ThisColumn.Company, Company);
                result.Add(ThisColumn.Status, Status);
                result.Add(ThisColumn.DateTime, DateTime);
            }

            public readonly SelectionWell SelectionWell;
            Client client;
            public uint ID => SelectionWell.WellID;
            public string Number => SelectionWell.FormatNumber;
            public string Company => ColumnOff.Contains(ThisColumn.Companyn) ? string.Empty : client.Detail.FullName.ToString().StringDivision();
            public string Well => ColumnOff.Contains(ThisColumn.Welln) ? string.Empty : Logic.LogicInstances.WellLogic.FirstOrDefault(SelectionWell.WellID).PresentNumber;
            public int Folder => client.ObjAtWell(ID).NumberFolder;
            public string Status => ColumnOff.Contains(ThisColumn.Statusn) ? string.Empty : T.Status.Rows.Get<string>(SelectionWell.Sample.StatusID, C.Status.Name);
            public string DateTime => ColumnOff.Contains(ThisColumn.DateTimen) ? string.Empty : MyTools.YearMonthDayHoureMinutes_From_YMDHM(SelectionWell.YMDHM).StringDivision(7);
            public string Volume
            {
                get
                {
                    return ColumnOff.Contains(ThisColumn.Volumen) ? string.Empty : SelectionWell.Sample.Volumes.Any() ?
                                          SelectionWell.Sample.Volumes.Count() > 1 ?
                                          SelectionWell.Sample.Volumes.Select(x => $"{Math.Round(AdditionnTable.ListPeriod.First(y => y.ID == x.PeriodID).Price, 2)} - {x.Value}").Aggregate((a, b) => $"{a}\n{b}") :
                                          SelectionWell.Sample.Volumes.First().Value.ToString()
                                          : string.Empty;
                }
                set
                {
                    if (volumeTwo)
                    {
                        int castil = 0;
                        foreach (var one in value.Split(' ').Select(x => x.Replace('.', ',').TryParseDouble()).ToArray().Reverse())
                        {
                            SelectionWell.Sample.Add(Logic.LogicInstances.VolumeLogic.FirstOrDefault(
                                MyTools.AddRowFromTable(G.Volume,
                                new KeyValuePair<int, object>(C.Volume.Value, one),
                                new KeyValuePair<int, object>(C.Volume.Period, castil == 0 ? AdditionnTable.GetPeriod.ID : AdditionnTable.ListPeriod.First(x => x.YM < AdditionnTable.GetPeriod.YM).ID),
                                new KeyValuePair<int, object>(C.Volume.Sample, SelectionWell.Sample.ID))
                                ));
                        }
                    }
                    else if (value.Split(' ').Length > 1)
                    { MessageBox.Show("В текущем месяце не может быть задано несколько объёмов!"); }
                    else if (value.Replace('.', ',').TryParseDouble() > 0)
                    {
                        double vol = value.Replace('.', ',').TryParseDouble();
                        if (SelectionWell.Sample.Volumes.FirstOrDefault(x => x.SampleID == SelectionWell.SampleID && x.PeriodID == AdditionnTable.GetPeriod.ID) == null)
                        {
                            SelectionWell.Sample.Add(Logic.LogicInstances.VolumeLogic.FirstOrDefault(
                            MyTools.AddRowFromTable(G.Volume,
                                new KeyValuePair<int, object>(C.Volume.Value, vol),
                                new KeyValuePair<int, object>(C.Volume.Period, AdditionnTable.GetPeriod.ID),
                                new KeyValuePair<int, object>(C.Volume.Sample, SelectionWell.SampleID))
                                ));
                        }
                        else
                        {
                            MyTools.SetValuseFromTable(G.Volume,
                            SelectionWell.Sample.Volumes.First(x => x.SampleID == SelectionWell.SampleID && x.PeriodID == AdditionnTable.GetPeriod.ID).ID,
                            new KeyValuePair<int, object>(C.Volume.Value, vol));
                        }
                    }
                }
            }
            public readonly KeyValuePair<string, decimal>[] summs;
            public string Summ => summs.Length > 0 ? summs.Select(x => x.Key + x.Value.ToMoney()).Aggregate((a, b) => $"{a}\n{b}") : string.Empty;
            public ValueWork[] Values { get; }

            Dictionary<string, string> result;
            public Dictionary<string, string> values => result;
        }
        /// <summary>Отрисовать</summary>
        public void Draw()
        {
            AllClients.LoadClients();
            ClearItemSource();
            DRAW_THIS();
            ShowDG();
            ResetItemSource(Values);
            StartSearch();
        }
        /// <summary>Выгрузить в DataGrid</summary>
        private void ShowDG()
        {
            bool two;
            two = volumeTwo = AdditionnTable.ListPeriod.FirstOrDefault(x => x.YM + 1 == AdditionnTable.GetPeriod.YM) != null;

            DG.CellEditEnding += (sender, e) =>
            {
                if (e.Column.Header.ToString() == ThisColumn.Volume && two)
                {
                    MessageBox.Show("Формат занесения объёмов\n1 объём (000,000)\n2 объёма (000,000 000,000) - убедитесь что между объёмами есть пробел\nПервый объём по старому тарифу, второй объём по новому\nСимвол , равнозначен символу .");
                    two = false;
                }
            };

            StopSearch();
        }
        /// <summary>Заполнить лист хранилище</summary>
        private void DRAW_THIS()
        {
            PollutionBase_Class.LoadSelectedWells(DateControl_Class.SelectMonth);

            var actualSelectionWells = PollutionBase_Class.ListSelectionWell.Where(x => x.Number > 0).OrderBy(x => x.Number).ToArray();
            
            foreach (var actialSelectionWell in actualSelectionWells)
            {
                List<KeyValuePair<string, decimal>> Summs = new List<KeyValuePair<string, decimal>>();
                //расчеты
                if (!ColumnOff.Contains(ThisColumn.Summn))
                {
                    Sample samp = actialSelectionWell.Sample;
                    Objecte obj = actialSelectionWell.Well.Objecte;
                    KeyValuePair<uint, BaseCalc_Class.Summs>[] summs;
                    Resolution resolution;

                    #region 644
                    resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644"));
                    if (obj.CanResolution(resolution.ID))
                    {
                        calc = new Calc_644(samp, obj, resolution);
                        summs = calc.Calc();

                        Summs.AddRange(summs.Select(x => new KeyValuePair<string, decimal>("644:", x.Value.SummNDS)));
                    }
                    #endregion

                    #region 621
                    resolution = PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621"));
                    if (obj.CanResolution(resolution.ID))
                    {
                        calc = new Calc_621(samp, obj, resolution);
                        summs = calc.Calc();

                        Summs.AddRange(summs.Select(x => new KeyValuePair<string, decimal>("621:", x.Value.SummNDS)));
                    }
                    #endregion
                }

                Dictionary<string, ValueWork> values = new Dictionary<string, ValueWork>();
                //концентрации
                if (!ColumnOff.Contains(ThisColumn.Pollution))
                {
                    foreach (var pollution in PollutionBase_Class.AllPolutions)
                    { values.Add(pollution.BindName, new ValueWork(pollution.ID, actialSelectionWell.ID)); }

                    foreach (var selectionValue in actialSelectionWell.ValueSelections)
                    {
                        values[selectionValue.Pollution.BindName] = new ValueWork(selectionValue.ID);
                    }
                }

                Values.Add(new SHOW(actialSelectionWell, Summs.ToArray(), values.Values.ToArray()));
            }
        }

        static List<int> ColumnOff;
        public void ColumnsSelector(MenuItem MI)
        {
            ColumnOff = Properties.Settings.Default.ColumnOff == "0" ? new List<int>() : Properties.Settings.Default.ColumnOff.Split('.').Select(x => x.TryParseInt()).ToList();
            foreach (var one in columns.Skip(1).Take(7))
            {
                CheckBox CB = new CheckBox();
                CB.Content = one.Column.Header;
                CB.IsChecked = !ColumnOff.Contains(one.Number);
                CB.Click += (sender, e) =>
                  {
                      if (!(bool)CB.IsChecked)
                      { ColumnOff.Add(one.Number); }
                      else
                      { ColumnOff.Remove(one.Number); }
                  };
                MI.Items.Add(CB);
            }
            CheckBox CBPl = new CheckBox();
            CBPl.Content = "Загрязнения";
            CBPl.FontSize = 12;
            CBPl.IsChecked = !ColumnOff.Contains(ThisColumn.Pollution);
            CBPl.Click += (sender, e) =>
            {
                if (!(bool)CBPl.IsChecked)
                { ColumnOff.Add(ThisColumn.Pollution); }
                else
                { ColumnOff.Remove(ThisColumn.Pollution); }
            };
            MI.Items.Add(CBPl);
            MI.SubmenuClosed += (sender, e) =>
              {
                  var off = Properties.Settings.Default.ColumnOff.Split('.').Select(x => x.TryParseInt()).ToArray();
                  if (ColumnOff.FirstOrDefault(x => !off.Contains(x)) != 0 || off.Length != ColumnOff.Count)
                  {
                      Properties.Settings.Default.ColumnOff = ColumnOff.Count > 0 ? ColumnOff.Select(x => x.ToString()).Aggregate((a, b) => $"{a}.{b}") : string.Empty;
                      Properties.Settings.Default.Save();
                      DrawColumns();
                  }
              };
            DrawColumns();
        }

        public enum EFilter { Number, Summ621, Summ644 }
        public void Filter(EFilter filter)
        {
            switch (filter)
            {
                case EFilter.Number:
                    {
                        Values = Values.OrderBy(x => (x as SHOW).SelectionWell.Number).ToList();
                        break;
                    }
                case EFilter.Summ621:
                    {
                        Values = Values.OrderBy(x => (x as SHOW).summs.FirstOrDefault(y => y.Key.Contains("621")).Value).ToList();
                        break;
                    }
                case EFilter.Summ644:
                    {
                        Values = Values.OrderBy(x => (x as SHOW).summs.FirstOrDefault(y => y.Key.Contains("644")).Value).ToList();
                        break;
                    }
            }
            ShowDG();
        }
        BaseCalc_Class calc;
    }
}