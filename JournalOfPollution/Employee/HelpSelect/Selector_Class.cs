using MAC_2.Employee.Mechanisms;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoTable;
using MAC_2.PrintForm;
using MAC_2.Model;

namespace MAC_2.Employee.HelpSelect
{
    /// <summary>
    /// Класс логики согласования отборов
    /// </summary>
    public class Selector_Class : C_Search_Class
    {
        public Selector_Class(DataGrid DG, WrapPanel wp, CheckBox ShowSelect, TextBlock text) : base(DG, wp)
        {
            this.text = text;
            this.ShowSelect = ShowSelect;
            ListUnit = new List<uint>();
            LoadContextMenu();
            LoadSelects();
            this.ShowSelect.Click += ShowSelect_Click;
            StartSearch();
        }

        CheckBox ShowSelect;
        TextBlock text;

        public IEnumerable<Unit> GetUnits()
        {
            var listUnit = new List<Unit>();

            foreach (ThisShow objecte in Values)
            {

            }

            return listUnit.ToArray();
        }
        
        private void ShowSelect_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ShowSelect.IsChecked)
            { ResetItemSource(Values); }
            else
            { ResetItemSource(Values.Where(x => (x as NegotiationAssistant).SampleID == 0).ToList()); }

            text.Text = $"План: {Values.Count} В обработке:{DG.Items.Count}";
        }
        ContextMenu ContextMenuDG;
        /// <summary>Помошник</summary>
        public void LoadSelects()
        {
            try
            {
                ShowSelect.IsEnabled = true;
                DG.Columns[6].Visibility = Visibility.Hidden;
                DG.Columns[7].Visibility = Visibility.Visible;
                DG.Columns[8].Visibility = Visibility.Visible;
            }
            catch
            { MessageBox.Show("Траблы с прятками колонок"); }
            try
            {
                var negotiationAssistants = Logic.LogicInstances.NegotiationAssistantLogic.Find(DateControl_Class.SelectMonth);

                Values.Clear();

                Values = negotiationAssistants.Select(x => (ISearch)new NegotiationAssistant(x.ID)).ToList();

                DG.MouseDoubleClick -= DG_Show_Add;
                DG.MouseDoubleClick += DG_Show_Edit;
                DG.LoadingRow -= DG_Show_FromEditor;
                DG.ContextMenu = ContextMenuDG;
            }
            catch
            { MessageBox.Show("Траблы в обработке"); }
            ShowSelect_Click(null, null);
            //StopSearch();
            
        }
        SearchGrid_Window SG_W;
        BasePrint print;
        private void DG_Show_Edit(object sender, MouseButtonEventArgs e)
        {
            if (DG.SelectedIndex > -1)
            {
                if (DG.CurrentCell.Column.DisplayIndex == 8)
                {
                    SG_W = new SearchGrid_Window(AdditionnTable.GetWorkerDT(data.ETypeTemplate.ActSelection), new C_SettingSearchDataGrid(DefDeleg: true, IDSelect: (DG.SelectedItem as NegotiationAssistant).WorkerID));
                    SG_W.ShowDialog();
                    ((NegotiationAssistant)DG.SelectedValue).WorkerID = SG_W.SelectID;
                    DG.Items.Refresh();
                }
            }
            if (DG.SelectedIndex > -1 && DG.CurrentCell.Column.DisplayIndex != 8)
            {
                new SampleAdd_Window((DG.SelectedItem as NegotiationAssistant).ID).ShowDialog();
                ShowSelect_Click(null, null);
            }
        }
        private void Print()
        {
            print.Start();
            DG.Items.Refresh();
        }
        private void LoadContextMenu()
        {
            ContextMenuDG = new ContextMenu();
            MenuItem MI_Act = new MenuItem();
            MI_Act.Header = "АКТ отбора стоков";
            MI_Act.Click += (sender, e) =>
            {
                var el = DG.SelectedItem as NegotiationAssistant; 
                print = new ActSelect_Print_Class(el.ID, el.client, el.sample, el.obj.ID);
                Print();
            };
            ContextMenuDG.Items.Add(MI_Act);

            MenuItem MI_Letter = new MenuItem();
            MI_Letter.Header = "Письмо-уведомление";
            MI_Letter.Click += (sender, e) =>
            {
                var el = DG.SelectedItem as NegotiationAssistant;
                print = new Letter_Print_Class(el.ID, el.client, el.obj.ID);
                Print();
            };
            ContextMenuDG.Items.Add(MI_Letter);
        }
        //public List<NegotiationAssistant> ListSelector;
        //public List<NegotiationAssistant> Show;
        public class NegotiationAssistant : Deff
        {
            public NegotiationAssistant(uint ID)
            {
                this.ID = ID;
                obj = Logic.LogicInstances.ObjecteLogic.FirstOrDefault(ObjecteID);

                LoadSample(SampleID);

                result = new Dictionary<string, string>();
                result.Add("Наименование", Name);
                result.Add("Адрес", Adres);
            }
            public readonly uint ID;
            uint ObjecteID => T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Objecte);
            public int YMD
            {
                get { return T.NegotiationAssistant.Rows.Get<int>(ID, C.NegotiationAssistant.YMD); }
                set { G.NegotiationAssistant.QUERRY().SET.C(C.NegotiationAssistant.YMD, value).WHERE.ID(ID).DO(); }
            }
            public string DateProspective => YMD > 0 ? MyTools.YearMonthDay_From_YMD(YMD) : "нет акта";
            public string FIO_Post => new Worker(WorkerID).FIO;
            public uint WorkerID
            {
                get { return T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Worker); }
                set { G.NegotiationAssistant.QUERRY().SET.C(C.NegotiationAssistant.Worker, value).WHERE.ID(ID).DO(); }
            }
            public uint SampleID => T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Sample);
        }
        /// <summary>Редактор</summary>
        public void LoadEditor()
        {
            ShowSelect.IsEnabled = false;
            DG.Columns[6].Visibility = Visibility.Visible;
            DG.Columns[7].Visibility = Visibility.Hidden;
            DG.Columns[8].Visibility = Visibility.Hidden;

            ClearItemSource();

            var objects = Logic.LogicInstances.ObjecteLogic.Find(DateControl_Class.SelectMonth, true);

            {
                var dictionary = new Dictionary<uint, ThisShow>();

                foreach (var obj in objects)
                {
                    var thisShow = new ThisShow(obj);

                    Values.Add(thisShow);

                    dictionary.Add(obj.ID, thisShow);
                }

                var negotiationAssistants = Logic.LogicInstances.NegotiationAssistantLogic.Find(DateControl_Class.SelectMonth);

                foreach (var negotiationAssistant in negotiationAssistants)
                {
                    if (dictionary.ContainsKey(negotiationAssistant.ObjectID))
                    {
                        var thisShow = dictionary[negotiationAssistant.ObjectID];

                        thisShow.NegotiationAssistandID = negotiationAssistant.ID;
                    }
                }
            }

            DG.LoadingRow += DG_Show_FromEditor;
            DG.MouseDoubleClick += DG_Show_Add;
            DG.MouseDoubleClick -= DG_Show_Edit;
            ResetItemSource(Values);
            DG.ContextMenu = null;

            //StartSearch();
        }
        private void DG_Show_FromEditor(object sender, DataGridRowEventArgs e)
        {
            if ((DG.Items[e.Row.GetIndex()] as ThisShow).NegotiationAssistandID > 0)
            { e.Row.Background = Brushes.Green; }
            else
            { e.Row.Background = Brushes.White; }
        }
        private void DG_Show_Add(object sender, MouseButtonEventArgs e)
        {
            if (DG.SelectedIndex == -1)
            { return; }
            var elem = (DG.Items[DG.SelectedIndex] as ThisShow);
            if (elem.NegotiationAssistandID > 0)
            {
                MyTools.DeleteRowsByID(G.NegotiationAssistant, false, elem.NegotiationAssistandID);
                elem.NegotiationAssistandID = 0;
            }
            else
            {
                elem.NegotiationAssistandID = MyTools.AddRowFromTable(true, G.NegotiationAssistant,
                    new KeyValuePair<int, object>(C.NegotiationAssistant.Objecte, elem.ID),
                    new KeyValuePair<int, object>(C.NegotiationAssistant.YM, DateControl_Class.SelectMonth));
            }
            DataGridRow row = (DataGridRow)DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex);
            row.Background = (DG.Items[row.GetIndex()] as ThisShow).NegotiationAssistandID > 0 ? Brushes.Green : Brushes.White;
            DG.SelectedIndex = DG.SelectedIndex + (DG.SelectedIndex != DG.Items.Count - 1 ? 1 : -1);
            text.Text = $"План: {Values.Where(x => (x as ThisShow).NegotiationAssistandID > 0).Count()}";
        }
        public class ThisShow : Deff
        {
            public ThisShow(Objecte obj)
            {
                this.obj = obj;
                result = new Dictionary<string, string>();
                result.Add("Наименование", Name);
                result.Add("Адрес", Adres);
            }

            public uint NegotiationAssistandID;
        }

        #region Доп контент

        private SelectionWell[] LoadSample(uint[] IDObj)
        {
            var querry = G.SelectionWell.QUERRY()
                .SHOW
                .WHERE
                .ID(0);
            foreach (var one in IDObj)
            { querry.OR.ARC(C.SelectionWell.Well, C.Well.Object).EQUI.BV(one); }
            querry.DO();
            SelectionWell[] result = new SelectionWell[G.SelectionWell.Rows.Count];
            for (int i = 0; i < result.Length; i++)
            { result[i] = Logic.LogicInstances.SelectionWellLogic.FirstOrDefault(G.SelectionWell.Rows.GetID(i)); }
            return result.GroupBy(x => x.ObjectID).Select(x => x.OrderBy(y => y.YMDHM).Last()).ToArray();
        }
        public abstract class Deff : ISearch
        {
            public void LoadSample(uint sampleID)
            {
                sample = Logic.LogicInstances.SampleLogic.FirstOrDefault(sampleID);
            }

            public uint ID => obj.ID;
            protected Dictionary<string, string> result;
            public Dictionary<string, string> values => result;
            public Objecte obj;
            public Client client => obj.Client;
            public Sample sample;
            public string Name => client.Detail?.FullName.StringDivision();
            public string DateLastSelect => sample != null ? MyTools.YearMonth_From_YM(sample.YM) : string.Empty;

            public string Number => obj.NumberFolder.ToString();
            public string Adres => obj.Adres.StringDivision(30);
            public string Well => obj.Wells.Count().ToString();
            public string MidMonthVolume => obj.GetMidMonthVolume(DateControl_Class.SelectYear - 1).Volume.ToString();
            uint[] Dumps => obj.Wells.GroupBy(x => x.IDUnit).Select(x => x.Key).Where(x => ListUnit.Contains(x)).ToArray();
            public string DumpPool => Dumps.Length > 0 ? Dumps.Select(x => Logic.LogicInstances.UnitLogic.FirstOrDefault(x)).Select(x => x.Name).Aggregate((a, b) => a + "," + b) : string.Empty;
        }
        public static List<uint> ListUnit;

        #endregion
    }
}