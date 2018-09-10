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

namespace MAC_2.Employee
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

                    new EditSample.EditValue_Window((DG.Items[DG.SelectedIndex] as SHOW).SelectionWell).ShowDialog();
                }
            };
            MyTools.BindExp_DG(DG);
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

            List<SelectionWell> SW = new List<SelectionWell>();

            foreach (SHOW one in DG.SelectedItems)
            { SW.Add(one.SelectionWell); }
            
            DG.ContextMenu.Items.Clear();

            MenuItem miCalc = new MenuItem { Header = "Расчёт" };
            miCalc.Click += (senderC, eC) =>
              {
                  if (item.Volume.Length > 0)
                  { new CalculationFees_Print_Class(item.SelectionWell.Sample); }
                  else
                  { MessageBox.Show("Объём не задан!"); }
              };

            DG.ContextMenu.Items.Add(miCalc);

            MenuItem miExtract = new MenuItem { Header = "Выписка" };
            miExtract.Click += (senderE, eE) =>
            {
                print = new Extract_Print_Class(item.SelectionWell.Sample);
                print.Start();
            };
            DG.ContextMenu.Items.Add(miExtract);

            MenuItem miProtocol = new MenuItem { Header = "Протокол" };
            miProtocol.Click += (senderE, eE) =>
            {
                if (SW.Count > 0)
                {
                    print = new Protocol_Print_Class(SW.ToArray());
                    print.Start();
                }
            };
            DG.ContextMenu.Items.Add(miProtocol);

            MenuItem miInspectionResult = new MenuItem { Header = "Результат контроля" };
            miInspectionResult.Click += (senderE, eE) =>
            {
                print = new InspectionResult_Print_Class(item.SelectionWell);
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
            public string ExtraValue;
        }

        /// <summary>Создать колонки</summary>
        public void CreateColumn()
        {
            var settingOneWay = new MyTools.C_Setting_DGColumn();

            var Result = new List<column>();

            var isReadOnly = false;

            Result.Add(new column(ThisColumn.Number, "Number", ThisColumn.Numbern, settingOneWay));
            Result.Add(new column(ThisColumn.Company, "Company", ThisColumn.Companyn, settingOneWay));
            Result.Add(new column(ThisColumn.Well, "Well", ThisColumn.Welln, settingOneWay));
            Result.Add(new column(ThisColumn.Folder, "Folder", ThisColumn.Foldern, settingOneWay));
            Result.Add(new column(ThisColumn.Status, "Status", ThisColumn.Statusn, settingOneWay));
            Result.Add(new column(ThisColumn.DateTime, "DateTime", ThisColumn.DateTimen, settingOneWay));
            Result.Add(new column(ThisColumn.Volume, "Volume", ThisColumn.Volumen, new MyTools.C_Setting_DGColumn(BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged, isReadOnly)));
            Result.Add(new column(ThisColumn.Summ, "Summ", ThisColumn.Summn, settingOneWay));

            int now = Result.Count;

            {
                var pollutions = Helpers.LogicHelper.PollutionLogic.Find();

                Result.AddRange(pollutions.Select(x => new column(x.CurtName.SymbolConverter(), $"Values[{x.Index}].Value", x.Number + now, new MyTools.C_Setting_DGColumn(BindingMode.TwoWay, IsReadOnly: isReadOnly)) { ExtraValue = "skip"}));
            }

            columns = Result.ToArray();
        }

        /// <summary>Перерисовать колонки</summary>
        public void DrawColumns()
        {
            DG.Columns.Clear();

            foreach (var column in columns)
            { DG.Columns.Add(column.Column); }

            DG.FrozenColumnCount = 9;

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
                this.summs = summs;
                this.Values = Values;

                result = new Dictionary<string, string>();
                result.Add(ThisColumn.Number, Number);
                result.Add(ThisColumn.Company, Company);
                result.Add(ThisColumn.Status, Status);
                result.Add(ThisColumn.DateTime, DateTime);
            }

            public readonly SelectionWell SelectionWell;
            private Client client => SelectionWell.Objecte.Client;
            public uint ID => SelectionWell.WellID;
            public string Number => SelectionWell.FormatNumber;
            public string Company => client.Detail.FullName.ToString().StringDivision();
            public string Well => Helpers.LogicHelper.WellLogic.FirstModel(SelectionWell.WellID).PresentNumber;
            public int Folder => client.ObjAtWell(ID).NumberFolder;
            public string Status => T.Status.Rows.Get<string>(SelectionWell.Sample.StatusID, C.Status.Name);
            public string DateTime => MyTools.YearMonthDayHoureMinutes_From_YMDHM(SelectionWell.YMDHM).StringDivision(7);
            public string Volume
            {
                get
                {
                    return SelectionWell.Sample.Volumes.Any() ?
                                          SelectionWell.Sample.Volumes.Count() > 1 ?
                                          SelectionWell.Sample.Volumes.Select(x => $"{Math.Round(AdditionnTable.ListPeriod.First(y => y.ID == x.PeriodID).Price, 2)} - {x.Value}").Aggregate((a, b) => $"{a}\n{b}") :
                                          SelectionWell.Sample.Volumes.First().Value.ToString()
                                          : string.Empty;
                }
                set
                {
                    double[] newVolumes;

                    {
                        var strVolumes = value.Split(' ');

                        var newVolumesList = new List<double>();

                        foreach (var strVolume in strVolumes)
                        {
                            double newVolume;
                            if (double.TryParse(strVolume, out newVolume))
                            { newVolumesList.Add(newVolume); }
                            else
                            {
                                Messages.StaticMessager.ErrorMessage.ShowMessage($"Не удалось распознать значение \"{strVolume}\" как число." + Environment.NewLine +
                                                                                 "Формат занесения объёмов:" + Environment.NewLine +
                                                                                 "1 объём (000,000)" + Environment.NewLine +
                                                                                 "2 объёма (000,000 000,000) - убедитесь что между объёмами есть пробел" + Environment.NewLine +
                                                                                 "Первый объём по старому тарифу, второй объём по новому" + Environment.NewLine +
                                                                                 "Символ \",\" равнозначен символу \".\"");
                                return;
                            }
                        }

                        newVolumes = newVolumesList.ToArray();
                    }

                    if (newVolumes.Length > 1 && !Helpers.PeriodHelper.DifferentPeriods)
                    {
                        Messages.StaticMessager.ErrorMessage.ShowMessage("В текущем месяце не может быть задано несколько объёмов!");
                        return;
                    }

                    if (newVolumes.Length > 2)
                    {
                        Messages.StaticMessager.ErrorMessage.ShowMessage("Объёмов не может быть больше двух!");
                        return;
                    }

                    var oldVolumes = SelectionWell.Sample.Volumes.ToArray();
                    var periods = new[] { Helpers.PeriodHelper.CurrentPeriod, Helpers.PeriodHelper.NextPeriod };

                    for (int i = 0; i < 2; i++)
                    {
                        if (oldVolumes.Length < i + 1 && newVolumes[i] > i)
                        { AddVolume(periods[i], newVolumes[i]); }
                        else
                        {
                            if (newVolumes.Length > i)
                            {
                                oldVolumes[i].Value = newVolumes[i];
                                oldVolumes[i].PeriodID = periods[i].ID;
                            }
                        }
                    }

                    {
                        this.SelectionWell.Sample.ClearVolumes();
                        var loadedVolumes = Helpers.LogicHelper.VolumeLogic.Find(this.SelectionWell.SampleID);

                        foreach (var loadedVolume in loadedVolumes)
                        { this.SelectionWell.Sample.Add(loadedVolume); }
                    }
                }
            }

            private void AddVolume(Period period, double volume)
            {
                G.Volume.QUERRY()
                .ADD
                .C(C.Volume.Period, period.ID)
                .C(C.Volume.Sample, this.SelectionWell.SampleID)
                .C(C.Volume.Value, volume)
                .DO();
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
            StopSearch();
        }
        /// <summary>Заполнить лист хранилище</summary>
        private void DRAW_THIS()
        {
            PollutionBase_Class.LoadSelectedWells(DateControl_Class.SelectMonth);

            var actualSelectionWells = Helpers.LogicHelper.SelectionWellLogic.Find(DateControl_Class.SelectMonth, 1);

            foreach (var actualSelectionWell in actualSelectionWells)
            {
                List<KeyValuePair<string, decimal>> Summs = new List<KeyValuePair<string, decimal>>();
                //расчеты
                Sample samp = actualSelectionWell.Sample;
                Objecte obj = actualSelectionWell.Well.Objecte;
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

                Dictionary<string, ValueWork> values = new Dictionary<string, ValueWork>();
                //концентрации
                foreach (var pollution in PollutionBase_Class.AllPolutions)
                { values.Add(pollution.BindName, new ValueWork(pollution, actualSelectionWell)); }

                foreach (var selectionValue in actualSelectionWell.ValueSelections)
                {
                    values[selectionValue.Pollution.BindName] = new ValueWork(selectionValue);
                }

                Values.Add(new SHOW(actualSelectionWell, Summs.ToArray(), values.Values.ToArray()));
            }
        }
        
        public void ColumnsSelector(MenuItem MI)
        {
            MI.Items.Clear();

            foreach (var column in columns)
            {
                if (column.ExtraValue == null)
                {
                    var CB = new CheckBox();
                    CB.Content = column.Column.Header;
                    CB.IsChecked = true;
                    CB.Click += (sender, e) => column.Column.Visibility = (CB.IsChecked.Value ? Visibility.Visible : Visibility.Hidden);
                    MI.Items.Add(CB);
                }
            }
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