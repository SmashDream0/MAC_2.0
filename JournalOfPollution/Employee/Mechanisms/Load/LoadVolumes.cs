using AutoTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.Windows.Media;
using MAC_2.PrintForm;
using System.Windows.Controls;
using System.Windows;
using MAC_2.Model;

namespace MAC_2.Employee.Mechanisms
{
    public class LoadVolumes : MyTools.C_A_BaseLoad_Excel
    {
        public LoadVolumes()
            : base()
        {
            DG.AutoGenerateColumns =
            DG.CanUserDeleteRows =
            DG.CanUserSortColumns =
            DG.CanUserAddRows = false;
            DG.LoadingRow += (sender, e) =>
              {
                  if ((e.Row.DataContext as show).Volumes == null)
                  { e.Row.Background = Brushes.Red; }
                  else if ((e.Row.DataContext as show).NumberFolder > 0)
                  { e.Row.Background = Brushes.Green; }
                  else
                  { e.Row.Background = Brushes.Yellow; }
              };
        }

        List<show> Files;
        show[] FILE;
        class show : I_Base_IDandValues
        {
            public show(uint objecteID, Dictionary<string, string> Values)
            {
                SetValues(Values);
                this.ID = objecteID;
                SetValues();
            }
            public show(show file, double[] Volume, string Acount)
            {
                this.ID = file.ID;
                this.NameClient = file.NameClient;
                this.INN = file.INN;
                this.Adres = file.Adres;
                this.period = file.period;
                this.Volume = Volume.Sum();
                this.Acount = Acount;
                this.Volumes = Volume.Select(x => x.ToString()).Aggregate((a, b) => $"{a} + {b}");
                SetValues();
            }
            public show(SelectionWell sw)
            {
                client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.GetIDValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client));
                NameClient = client.Detail.FullName.StringDivision(30);
                INN = client.INN;
                objecte = sw.Objecte;
                Adres = objecte.Adres;
                sample = sw.Sample;
            }

            public string NameClient { get; internal set; }
            public string INN { get; internal set; }
            public double Volume { get; internal set; }
            public string Acount { get; internal set; }
            public string Volumes { get; }
            public Period period = null;
            public uint ID { get; }
            public Dictionary<string, object> Values { get; internal set; }
            public Sample sample;
            public Client client;
            public string Tarif => (period == null ? String.Empty : $"{period.Price.ToString("#.00")} от {MyTools.YearMonth_From_YM(period.YM)}");

            public Objecte objecte;
            public int NumberFolder => objecte == null ? 0 : objecte.NumberFolder;

            public string Adres { get; internal set; }
            public string AdresFromBase => objecte == null ? string.Empty : objecte.Adres.StringDivision(40);

            private void SetValues()
            {
                Values = new Dictionary<string, object>();
                Values.Add(col.name, NameClient);
                Values.Add(col.inn, INN);
                Values.Add(col.adres, Adres);
            }
            public void SetClient(SelectionWell sw, Objecte obj = null)
            {
                if (sw == null)
                { return; }
                client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.GetIDValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client));
                if (obj == null)
                { objecte = client.Objects.FirstOrDefault(x => ComparisonAdres(x.Adres, Adres) || Acount.Split(',').FirstOrDefault(y => x.Accounts.Contains(y)) != null); }
                else
                { objecte = obj; }
                if (objecte != null)
                {
                    List<string> ac = new List<string>();
                    foreach (var one in Acount.Split(','))
                    {
                        if (!objecte.Accounts.Contains(one))
                        { ac.Add(one); }
                    }
                    if (ac.Count > 0)
                    { objecte.SetAccounts(ac.ToArray()); }
                    sample = Helpers.LogicHelper.SampleLogic.FirstModel(sw.SampleID);
                }
            }
            public void SetClient(SelectionWell[] sw)
            {
                client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.First().GetIDValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client));
                objecte = client.Objects.FirstOrDefault(x => ComparisonAdres(x.Adres, Adres) || Acount.Split(',').FirstOrDefault(y => x.Accounts.Contains(y)) != null);
                if (objecte != null)
                {
                    List<string> ac = new List<string>();
                    foreach (var one in Acount.Split(','))
                    {
                        if (!objecte.Accounts.Contains(one))
                        { ac.Add(one); }
                    }
                    if (ac.Count > 0)
                    { objecte.SetAccounts(ac.ToArray()); }
                    var sel = sw.FirstOrDefault(x => x.GetIDValue(C.SelectionWell.Well, C.Well.Object) == objecte.ID);
                    if (sel != null)
                    { sample = Helpers.LogicHelper.SampleLogic.FirstModel(sel.SampleID); }
                }

            }
            private void SetValues(Dictionary<string, string> Values)
            {
                foreach (var value in Values)
                {
                    switch (value.Key)
                    {
                        case col.name:
                            {
                                NameClient = value.Value.StringDivision(30);
                                break;
                            }
                        case col.inn:
                            {
                                INN = value.Value;
                                break;
                            }
                        case col.adres:
                            {
                                Adres = value.Value.StringDivision(30);
                                break;
                            }
                        case col.volnew:
                            {
                                period = _periods[(int)EPeriod.newP];
                                goto case col.volume;
                            }
                        case col.volold:
                            {
                                period = _periods[(int)EPeriod.oldP];
                                goto case col.volume;
                            }
                        case col.volume:
                            {
                                Volume = value.Value.TryParseDouble();
                                break;
                            }
                        case col.acount:
                            {
                                Acount = value.Value;
                                break;
                            }
                    }
                }
            }
        }

        #region нарезка адресов и сравнение

        private static bool ComparisonAdres(string Adres1, string Adres2)
        {
            Adres1 = CutAdr(Adres1);
            Adres2 = CutAdr(Adres2);

            if (Adres1.Contains(Adres2) || Adres2.Contains(Adres1))
            { return true; }
            return false;
        }

        private static string CutAdr(string adr)
        {
            return adr
                .CutAdres(false)
                .ToLower()
                .Replace("дом", string.Empty)
                .Replace("ул.", string.Empty)
                .Replace("№", string.Empty)
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);
        }

        #endregion

        #region Колонки и их обработка

        struct col
        {
            public const string name = "наименование предприятия";
            public const string inn = "инн";
            public const string volume = "объем, м3";
            public const string adres = "место отбора";
            public const string acount = "л/с";

            public const string volold = "объём по старому тарифу, м3";
            public const string volnew = "объём по новому тарифу, м3";
        }
        protected override void GetColumn()
        {
            column = new string[]
            {
                col.name,
                col.inn,
                col.volume,
                col.adres,
                col.acount
            };
            columnDop = new string[]
            {
                col.volold,
                col.volnew
            };
        }
        protected override void AgainsCreateColumn()
        {
            if (ColumnsBook.ContainsKey(col.volnew) || ColumnsBook.ContainsKey(col.volold))
            { LoadPeriod(); }
        }
        protected override void GetDGColumn()
        {
            MyTools.C_Setting_DGColumn setting = new MyTools.C_Setting_DGColumn();
            List<MyTools.C_DGColumn_Bind> list = new List<MyTools.C_DGColumn_Bind>();
            list.Add(new MyTools.C_DGColumn_Bind(col.name, MyTools.E_TypeColumnDG.Text, "NameClient", setting));
            //list.Add(new MyTools.C_DGColumn_Bind(col.inn, MyTools.E_TypeColumnDG.Text, "INN", setting));
            list.Add(new MyTools.C_DGColumn_Bind(col.volume, MyTools.E_TypeColumnDG.Text, "Volume", setting));
            list.Add(new MyTools.C_DGColumn_Bind("объёмы", MyTools.E_TypeColumnDG.Text, "Volumes", setting));
            if (ColumnsBook.ContainsKey(col.volnew) || ColumnsBook.ContainsKey(col.volold))
            { list.Add(new MyTools.C_DGColumn_Bind("тариф", MyTools.E_TypeColumnDG.Text, "Tarif", setting)); }
            list.Add(new MyTools.C_DGColumn_Bind(col.adres, MyTools.E_TypeColumnDG.Text, "Adres", setting));
            list.Add(new MyTools.C_DGColumn_Bind("Номер папки", MyTools.E_TypeColumnDG.Text, "NumberFolder", setting));
            list.Add(new MyTools.C_DGColumn_Bind("Адрес из БАЗЫ", MyTools.E_TypeColumnDG.Text, "AdresFromBase", setting));
            ColumnsFromDG = list.ToArray();
        }

        #region период
        private enum EPeriod { oldP, newP }
        private static Period[] _periods;
        private void LoadPeriod()
        {
            Period currentPeriod = null, nextPeriod = null;

            foreach (var period in AdditionnTable.ListPeriod)
            {
                if (period.YM <= DateControl_Class.SelectMonth)
                { currentPeriod = period; }
                else if (period.YM == DateControl_Class.SelectMonth + 1)
                { nextPeriod = period; }
            }

            _periods = new Period[2];

            _periods[(int)EPeriod.oldP] = currentPeriod;
            _periods[(int)EPeriod.newP] = nextPeriod;
        }
        #endregion

        #endregion

        SearchGrid_Window sg_W;
        Dictionary<string, string> values;
        protected override void Start()
        {
            var temp = new List<show>();
            int count = sheet.LastRowNum;
            uint ID = 1;
            for (int Nrow = StartRow; Nrow < sheet.LastRowNum + 1; Nrow++)
            {
                values = new Dictionary<string, string>();
                IRow row = sheet.GetRow(Nrow);

                if (row == null)
                { break; }

                foreach (var one in ColumnsBook)
                {
                    ICell cell = row.GetCell(one.Value);

                    values.Add(one.Key, cell.ToString().Trim().Trim('\''));
                }
                if (values.ContainsKey(col.volold) || values.ContainsKey(col.volnew))
                { values.Remove(col.volume); }
                temp.Add(new show(ID, values));
                ID++;
            }
            Files = temp.GroupBy(x => x.INN + x.Adres).Select(x => new show(x.First(), x.Select(y => y.Volume).ToArray(), x.Select(y => y.Acount.Replace("'", string.Empty)).Aggregate((a, b) => $"{a},{b}"))).ToList();
            Compare();
            ComparisonVolume();

            ContextMenu();

            DG.MouseDoubleClick += (sender, e) =>
              {
                  if (DG.SelectedIndex == -1)
                  { return; }

                  var select = (DG.Items[DG.SelectedIndex] as show);

                  if (select.objecte == null && select.client != null)
                  {
                      sg_W = new SearchGrid_Window(select.client.Objects.ToArray(), new C_SettingSearchDataGrid(DefDeleg: true, DopText: $"{select.Adres.Replace("\n", "")}{(select.sample == null ? "\nНе найден отбор!" : string.Empty)}"));
                      sg_W.ShowDialog();
                      if (sg_W.SelectID > 0)
                      {
                          if (SWs.FirstOrDefault(x => x.ObjectID == sg_W.SelectID) == null)
                          {
                              if (MessageBox.Show($"Объект: {Helpers.LogicHelper.ObjecteLogic.FirstModel(sg_W.SelectID).Adres}\nне отбирался {MyTools.YearMonth_From_YM(date)}г\nХотите удалить запись?", "Убрать не отобранный объект?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                              {
                                  select.client.Objects.First(x => x.ID == sg_W.SelectID).SetAccounts(select.Acount.Split(','));
                                  Files.Remove(select);
                                  ResetDG();
                                  return;
                              }
                          }
                          else
                          {
                              select.SetClient(SWs.First(x => x.ObjectID == sg_W.SelectID), select.client.Objects.First(x => x.ID == sg_W.SelectID));
                              //Волшебная строка закраски ROW
                              (DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex) as DataGridRow).Background = Brushes.Green;
                              //Волшебная строка закраски ROW
                          }
                      }
                  }
                  else if (select.Volumes == null || select.client == null)
                  {
                      sg_W = new SearchGrid_Window(FILE, new C_SettingSearchDataGrid(DefDeleg: true, DopText: $"Клиент - {select.NameClient}\nАдрес- {select.Adres}\nИНН- {select.INN}"));
                      sg_W.ShowDialog();
                      if (sg_W.SelectID > 0)
                      {
                          show fil = FILE.First(x => x.ID == sg_W.SelectID);
                          Files[DG.SelectedIndex] = new show(select, fil.Volumes.Split('+').Select(x => x.TryParseDouble()).ToArray(), fil.Acount);
                          Files[DG.SelectedIndex].SetClient(SWs.FirstOrDefault(x => (string)x.GetValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client, C.Client.INN) == Files[DG.SelectedIndex].INN));
                          Objecte obj = Files[DG.SelectedIndex].objecte;
                          if (obj != null)
                          {
                              Files[DG.SelectedIndex].SetClient(SWs.First(x => x.ObjectID == obj.ID), obj);
                              (DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex) as DataGridRow).Background = Brushes.Green;
                          }
                          else
                          { (DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex) as DataGridRow).Background = Brushes.Yellow; }
                      }
                  }
              };

            ResetDG();
        }

        private void ResetDG()
        {
            DG.ItemsSource = null;
            DG.ItemsSource = Files;
        }

        /// <summary>Контекстное меню для таблицы</summary>
        private void ContextMenu()
        {
            DG.ContextMenu = new System.Windows.Controls.ContextMenu();
            MenuItem miDelObj = new MenuItem();
            miDelObj.Header = "Отчистить объект";
            miDelObj.Click += (sender, e) =>
            {
                if (DG.SelectedIndex == -1)
                { return; }
                if (MessageBox.Show("Отчистить объект?", "Отчистка привязки объекта", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var select = (DG.Items[DG.SelectedIndex] as show);
                    select.objecte.DelectAccounts(select.Acount.Split(','));
                    select.objecte = null;
                    (DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex) as DataGridRow).Background = Brushes.Yellow;
                }
            };
            DG.ContextMenu.Items.Add(miDelObj);

            MenuItem miDelClient = new MenuItem();
            miDelClient.Header = "Отчистить клиента";
            miDelClient.Click += (sender, e) =>
            {
                if (DG.SelectedIndex == -1)
                { return; }
                if (MessageBox.Show("Отчистить клиента?", "Отчистка привязки клиента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var select = (DG.Items[DG.SelectedIndex] as show);
                    if (select.objecte != null)
                    {
                        select.objecte.DelectAccounts(select.Acount.Split(','));
                        select.objecte = null;
                    }
                    select.client = null;
                    (DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex) as DataGridRow).Background = Brushes.Red;
                }
            };
            DG.ContextMenu.Items.Add(miDelClient);

            MenuItem miDelRow = new MenuItem();
            miDelRow.Header = "Удалить запись";
            miDelRow.Click += (sender, e) =>
            {
                if (DG.SelectedIndex == -1)
                { return; }
                var select = (DG.Items[DG.SelectedIndex] as show);
                if (MessageBox.Show($"Удалить запись?\nКлиент: {select.NameClient}\nАдрес: {select.Adres}", "Удаление записи", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Files.Remove(select);
                    ResetDG();
                }
            };
            DG.ContextMenu.Items.Add(miDelRow);
        }

        #region обработка отборов

        uint[] Samples;
        List<SelectionWell> SWs;
        int date;
        /// <summary>Отсеять не отобранных</summary>
        private void Compare()
        {
            string text = SearthCellFromMark("Объем сточных вод, сброшенных абонентами МУП \"Астрводоканал\" с ", false).StringCellValue;
            date = (int)MyTools.StringDATE_In_intDate(text.Substring(text.IndexOf(" с ") + 3, 10), MyTools.EInputDate.YMD, MyTools.EInputDate.YM, true);
            G.SelectionWell.QUERRY()
                .SHOW
                .WHERE
                    .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(date)
                .AND
                    .AC(C.SelectionWell.Number).More.BV(0)
                .DO();
            SWs = new List<SelectionWell>();
            Samples = new uint[G.SelectionWell.Rows.Count];
            for (int i = 0; i < Samples.Length; i++)
            {
                Samples[i] = G.SelectionWell.Rows.Get_UnShow<uint>(i, C.SelectionWell.Sample);
                if (SWs.FirstOrDefault(x => x.ObjectID == G.SelectionWell.Rows.Get_UnShow<uint>(i, C.SelectionWell.Well, C.Well.Object)) == null)
                { SWs.Add(Helpers.LogicHelper.SelectionWellLogic.FirstModel(G.SelectionWell.Rows.GetID(i))); }
            }
            var temp = SWs.Select(y => new { INN = y.GetValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client, C.Client.INN).ToString(), sw = y });
            FILE = Files.ToArray();
            Files.Clear();
            foreach (var one in temp.GroupBy(x => x.INN))
            {
                if (one.Count() > 1)
                {
                    var fils = FILE.Where(x => x.INN == one.Key);
                    if (fils != null)
                    {
                        foreach (var fil in fils)
                        { fil.SetClient(one.Select(x => x.sw).ToArray()); }
                        Files.AddRange(fils);
                    }
                }
                else
                {
                    var fil = FILE.FirstOrDefault(x => x.INN == one.Key);
                    if (fil != null)
                    { fil.SetClient(one.First().sw); }
                    else
                    { fil = new show(one.First().sw); }
                    Files.Add(fil);
                }
            }
        }

        #endregion

        #region обработка объёмов

        Volume[] volumes;
        /// <summary>Пообъектная проверка</summary>
        private void ComparisonVolume()
        {
            var querry = G.Volume.QUERRY()
                .SHOW
                .WHERE
                .ID(0);

            foreach (var one in Samples)
            { querry.OR.C(C.Volume.Sample, one); }

            querry.DO();

            volumes = new Volume[G.Volume.Rows.Count];
            for (int i = 0; i < volumes.Length; i++)
            { volumes[i] = Helpers.LogicHelper.VolumeLogic.FirstModel(G.Volume.Rows.GetID(i)); }

            foreach (var one in volumes)
            {
                var fil = Files.FirstOrDefault(x => x.sample != null && x.sample.ID == one.SampleID);
                if (fil != null)
                {
                    if (fil.Volume > 0 && fil.Volume == one.Value)
                    { Files.Remove(fil); }
                }
            }

            foreach (var one in Files.Where(x => x.objecte != null && x.Volume > 0).ToArray())
            {
                if (SWs.FirstOrDefault(x => x.ObjectID == one.objecte.ID) == null)
                { Files.Remove(one); }
            }
        }

        #endregion

        public override bool StartLoad()
        {
            int num = 0;
            double volume = 0;

            foreach (var file in Files.ToArray())
            {
                if (file.objecte == null)
                { continue; }

                if (file.sample == null)
                {
                    if (file.Volume > 0)
                    { Files.Remove(file); }

                    continue;
                }

                if (file.period != null)
                {
                    if (volumes.FirstOrDefault(x => x.SampleID == file.sample.ID && x.PeriodID == file.period.ID) != null)
                    {
                        G.Volume.QUERRY()
                            .SET
                            .C(C.Volume.Value, file.Volume)
                            .WHERE
                            .ID(volumes.First(x => x.SampleID == file.sample.ID).ID)
                            .DO();
                    }
                    else
                    {
                        MyTools.AddRowFromTable(G.Volume,
                            new KeyValuePair<int, object>(C.Volume.Sample, file.sample.ID),
                            new KeyValuePair<int, object>(C.Volume.Period, file.period == null ? AdditionnTable.ListPeriod.Last(x => x.YM < date).ID : file.period.ID),
                            new KeyValuePair<int, object>(C.Volume.Value, file.Volume));
                    }

                    Files.Remove(file);
                    num++;
                    volume += file.Volume;
                }
            }

            if (num > 0)
            {
                MessageBox.Show($"Выгружено записей: {num} на объём: {volume}");
                ResetDG();
                return true;
            }
            else
            {
                MessageBox.Show($"Ничего выгрузить не удалось =(");
                return false;
            }
        }
        protected override void LoadMenu()
        { InstructionsMessage_Class.LoadInstructions(ThisMenu, data.ETypeInstruction.LoadVolume); }
    }
}