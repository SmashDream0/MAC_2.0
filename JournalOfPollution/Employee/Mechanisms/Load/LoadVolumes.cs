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

namespace MAC_2.Employee.Mechanisms.LoadVolume
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
                  if ((e.Row.DataContext as Item).Volumes == null)
                  { e.Row.Background = Brushes.Red; }
                  else if ((e.Row.DataContext as Item).NumberFolder > 0)
                  { e.Row.Background = Brushes.Green; }
                  else
                  { e.Row.Background = Brushes.Yellow; }
              };
        }

        public static Period CurrentPeriod { get; private set; }
        public static Period NewPeriod { get; private set; }

        private List<Item> _files;
        private Item[] _file;
        private Volume[] _volumes;
        private SearchGrid_Window _sg_W;
        private Dictionary<string, string> _values;

        #region Колонки и их обработка

        protected override void GetColumn()
        {
            column = new string[]
            {
                Columns.name,
                Columns.inn,
                Columns.volume,
                Columns.adres,
                Columns.acount
            };
            columnDop = new string[]
            {
                Columns.volold,
                Columns.volnew
            };
        }
        protected override void AgainsCreateColumn()
        {
            if (ColumnsBook.ContainsKey(Columns.volnew) || ColumnsBook.ContainsKey(Columns.volold))
            { LoadPeriod(); }
        }
        protected override void GetDGColumn()
        {
            MyTools.C_Setting_DGColumn setting = new MyTools.C_Setting_DGColumn();
            List<MyTools.C_DGColumn_Bind> list = new List<MyTools.C_DGColumn_Bind>();
            list.Add(new MyTools.C_DGColumn_Bind(Columns.name, MyTools.E_TypeColumnDG.Text, "NameClient", setting));
            //list.Add(new MyTools.C_DGColumn_Bind(col.inn, MyTools.E_TypeColumnDG.Text, "INN", setting));
            list.Add(new MyTools.C_DGColumn_Bind(Columns.volume, MyTools.E_TypeColumnDG.Text, "Volume", setting));
            list.Add(new MyTools.C_DGColumn_Bind("объёмы", MyTools.E_TypeColumnDG.Text, "Volumes", setting));
            if (ColumnsBook.ContainsKey(Columns.volnew) || ColumnsBook.ContainsKey(Columns.volold))
            { list.Add(new MyTools.C_DGColumn_Bind("тариф", MyTools.E_TypeColumnDG.Text, "Tarif", setting)); }
            list.Add(new MyTools.C_DGColumn_Bind(Columns.adres, MyTools.E_TypeColumnDG.Text, "Adres", setting));
            list.Add(new MyTools.C_DGColumn_Bind("Номер папки", MyTools.E_TypeColumnDG.Text, "NumberFolder", setting));
            list.Add(new MyTools.C_DGColumn_Bind("Адрес из БАЗЫ", MyTools.E_TypeColumnDG.Text, "AdresFromBase", setting));
            ColumnsFromDG = list.ToArray();
        }

        #region период
        private void LoadPeriod()
        {
            CurrentPeriod = Helpers.PeriodHelper.CurrentPeriod;
            NewPeriod = Helpers.PeriodHelper.NextPeriod;
        }

        #endregion

        #endregion
        protected override void Start()
        {
            var temp = new List<Item>();
            int count = sheet.LastRowNum;
            uint ID = 1;
            for (int Nrow = StartRow; Nrow < sheet.LastRowNum + 1; Nrow++)
            {
                _values = new Dictionary<string, string>();
                IRow row = sheet.GetRow(Nrow);

                if (row == null)
                { break; }

                foreach (var one in ColumnsBook)
                {
                    ICell cell = row.GetCell(one.Value);

                    _values.Add(one.Key, cell.ToString().Trim().Trim('\''));
                }
                if (_values.ContainsKey(Columns.volold) || _values.ContainsKey(Columns.volnew))
                { _values.Remove(Columns.volume); }
                temp.Add(new Item(ID, _values));
                ID++;
            }
            _files = temp.GroupBy(x => x.INN + x.Adres).Select(x => new Item(x.First(), x.Select(y => y.Volume).ToArray(), x.Select(y => y.Acount.Replace("'", string.Empty)).Aggregate((a, b) => $"{a},{b}"))).ToList();
            Compare();
            ComparisonVolume();

            ContextMenu();

            DG.MouseDoubleClick += (sender, e) =>
              {
                  if (DG.SelectedIndex == -1)
                  { return; }

                  var select = (DG.Items[DG.SelectedIndex] as Item);

                  if (select.objecte == null && select.client != null)
                  {
                      _sg_W = new SearchGrid_Window(select.client.Objects.ToArray(), new C_SettingSearchDataGrid(DefDeleg: true, DopText: $"{select.Adres.Replace("\n", "")}{(select.sample == null ? "\nНе найден отбор!" : string.Empty)}"));
                      _sg_W.ShowDialog();
                      if (_sg_W.SelectID > 0)
                      {
                          if (SWs.FirstOrDefault(x => x.ObjectID == _sg_W.SelectID) == null)
                          {
                              if (MessageBox.Show($"Объект: {Helpers.LogicHelper.ObjecteLogic.FirstModel(_sg_W.SelectID).Adres}\nне отбирался {MyTools.YearMonth_From_YM(date)}г\nХотите удалить запись?", "Убрать не отобранный объект?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                              {
                                  select.client.Objects.First(x => x.ID == _sg_W.SelectID).SetAccounts(select.Acount.Split(','));
                                  _files.Remove(select);
                                  ResetDG();
                                  return;
                              }
                          }
                          else
                          {
                              select.SetClient(SWs.First(x => x.ObjectID == _sg_W.SelectID), select.client.Objects.First(x => x.ID == _sg_W.SelectID));
                              //Волшебная строка закраски ROW
                              (DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex) as DataGridRow).Background = Brushes.Green;
                              //Волшебная строка закраски ROW
                          }
                      }
                  }
                  else if (select.Volumes == null || select.client == null)
                  {
                      _sg_W = new SearchGrid_Window(_file, new C_SettingSearchDataGrid(DefDeleg: true, DopText: $"Клиент - {select.NameClient}\nАдрес- {select.Adres}\nИНН- {select.INN}"));
                      _sg_W.ShowDialog();
                      if (_sg_W.SelectID > 0)
                      {
                          Item fil = _file.First(x => x.ID == _sg_W.SelectID);
                          _files[DG.SelectedIndex] = new Item(select, fil.Volumes.Split('+').Select(x => x.TryParseDouble()).ToArray(), fil.Acount);
                          _files[DG.SelectedIndex].SetClient(SWs.FirstOrDefault(x => (string)x.GetValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client, C.Client.INN) == _files[DG.SelectedIndex].INN));
                          Objecte obj = _files[DG.SelectedIndex].objecte;
                          if (obj != null)
                          {
                              _files[DG.SelectedIndex].SetClient(SWs.First(x => x.ObjectID == obj.ID), obj);
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
            DG.ItemsSource = _files;
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
                    var select = (DG.Items[DG.SelectedIndex] as Item);
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
                    var select = (DG.Items[DG.SelectedIndex] as Item);
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
                var select = (DG.Items[DG.SelectedIndex] as Item);
                if (MessageBox.Show($"Удалить запись?\nКлиент: {select.NameClient}\nАдрес: {select.Adres}", "Удаление записи", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _files.Remove(select);
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

                var wellID = G.SelectionWell.Rows.Get_UnShow<uint>(i, C.SelectionWell.Well);

                if (SWs.FirstOrDefault(x => x.WellID == wellID) == null)    //прежняя конструкция отбрасывала отборы с одинаковыми объектами и разными колодцами
                { SWs.Add(Helpers.LogicHelper.SelectionWellLogic.FirstModel(G.SelectionWell.Rows.GetID(i))); }
            }

            var innGroup = SWs.Select(y => new { INN = y.GetValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client, C.Client.INN).ToString(), sw = y });
            _file = _files.ToArray();
            _files.Clear();

            foreach (var innGroupItem in innGroup.GroupBy(x => x.INN))
            {
                if (innGroupItem.Count() > 1)
                {
                    var innFileItems = _file.Where(x => x.INN == innGroupItem.Key);

                    if (innFileItems != null)
                    {
                        foreach (var fil in innFileItems)
                        { fil.SetClient(innGroupItem.Select(x => x.sw).ToArray()); }

                        _files.AddRange(innFileItems);
                    }
                }
                else
                {
                    var innFileItem = _file.FirstOrDefault(x => x.INN == innGroupItem.Key);

                    if (innFileItem != null)
                    { innFileItem.SetClient(innGroupItem.First().sw); }
                    else
                    { innFileItem = new Item(innGroupItem.First().sw); }

                    _files.Add(innFileItem);
                }
            }
        }

        #endregion

        #region обработка объёмов
        /// <summary>Пообъектная проверка</summary>
        private void ComparisonVolume()
        {
            var query = G.Volume.QUERRY()
                .SHOW
                .WHERE
                .ID(0);

            foreach (var one in Samples)
            { query.OR.C(C.Volume.Sample, one); }

            query.DO();

            _volumes = new Volume[G.Volume.Rows.Count];
            for (int i = 0; i < _volumes.Length; i++)
            { _volumes[i] = Helpers.LogicHelper.VolumeLogic.FirstModel(G.Volume.Rows.GetID(i)); }

            foreach (var volume in _volumes)
            {
                var fileItem = _files.FirstOrDefault(x => x.sample != null && x.sample.ID == volume.SampleID);
                if (fileItem != null)
                {
                    if (fileItem.Volume > 0 && fileItem.Volume == volume.Value)
                    { _files.Remove(fileItem); }
                }
            }

            foreach (var fileItem in _files.Where(x => x.objecte != null && x.Volume > 0).ToArray())
            {
                if (SWs.FirstOrDefault(x => x.ObjectID == fileItem.objecte.ID) == null)
                { _files.Remove(fileItem); }
            }
        }

        #endregion

        public override bool StartLoad()
        {
            int num = 0;
            double value = 0;

            foreach (var file in _files.ToArray())
            {
                if (file.objecte == null)
                { continue; }

                if (file.sample == null)
                {
                    if (file.Volume > 0)
                    { _files.Remove(file); }

                    continue;
                }

                if (file.period != null)
                {
                    var volume = _volumes.FirstOrDefault(x => x.SampleID == file.sample.ID && x.PeriodID == file.period.ID);

                    if (volume != null)
                    {
                        G.Volume.QUERRY()
                            .SET
                            .C(C.Volume.Value, file.Volume)
                            .WHERE
                            .ID(volume.ID)
                            .DO();
                    }
                    else
                    {
                        MyTools.AddRowFromTable(G.Volume,
                            new KeyValuePair<int, object>(C.Volume.Sample, file.sample.ID),
                            new KeyValuePair<int, object>(C.Volume.Period, file.period.ID),
                            new KeyValuePair<int, object>(C.Volume.Value, file.Volume));
                    }

                    _files.Remove(file);
                    num++;
                    value += file.Volume;
                }
            }

            if (num > 0)
            {
                MessageBox.Show($"Выгружено записей: {num} на объём: {value}");
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