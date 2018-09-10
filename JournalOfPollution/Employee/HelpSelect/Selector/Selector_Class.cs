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

namespace MAC_2.Employee.HelpSelect.Selector
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
            { ResetItemSource(Values.Where(x => (x as NegotiationAssistantSearch).SampleID == 0).ToList()); }

            text.Text = $"План: {Values.Count} В обработке:{DG.Items.Count}";
        }
        ContextMenu ContextMenuDG;
        /// <summary>Помошник</summary>
        public void LoadSelects()
        {
            ShowSelect.IsEnabled = true;
            DG.Columns[6].Visibility = Visibility.Hidden;
            DG.Columns[7].Visibility = Visibility.Visible;
            DG.Columns[8].Visibility = Visibility.Visible;

            var negotiationAssistants = Helpers.LogicHelper.NegotiationAssistantLogic.Find(DateControl_Class.SelectMonth);

            this.ClearItemSource();

            Values = negotiationAssistants.Select(x => (ISearch)new NegotiationAssistantSearch(x)).ToList();

            this.ResetItemSource(Values);

            DG.MouseDoubleClick -= DG_Show_Add;
            DG.MouseDoubleClick += DG_Show_Edit;
            DG.LoadingRow -= DG_Show_FromEditor;

            DG.ContextMenu = ContextMenuDG;

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
                    SG_W = new SearchGrid_Window(AdditionnTable.GetWorkerDT(data.ETypeTemplate.ActSelection), new C_SettingSearchDataGrid(DefDeleg: true, IDSelect: (DG.SelectedItem as NegotiationAssistantSearch).WorkerID));
                    SG_W.ShowDialog();

                    if (SG_W.SelectID > 0)
                    {
                        var selectedValue = ((NegotiationAssistantSearch)DG.SelectedValue);

                        selectedValue.WorkerID = SG_W.SelectID;

                        var worker = Helpers.LogicHelper.WorkerLogic.FirstModel(SG_W.SelectID);

                        selectedValue.Add(worker);
                    }

                    DG.Items.Refresh();
                }
            }
            if (DG.SelectedIndex > -1 && DG.CurrentCell.Column.DisplayIndex != 8)
            {
                new SampleAdd_Window((DG.SelectedItem as NegotiationAssistantSearch).ID).ShowDialog();
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
                var negotiationAssistant = DG.SelectedItem as NegotiationAssistantSearch; 
                print = new ActSelect_Print_Class(negotiationAssistant);
                Print();
            };
            ContextMenuDG.Items.Add(MI_Act);

            MenuItem MI_Letter = new MenuItem();
            MI_Letter.Header = "Письмо-уведомление";
            MI_Letter.Click += (sender, e) =>
            {
                var el = DG.SelectedItem as NegotiationAssistantSearch;
                print = new Letter_Print_Class(el);
                Print();
            };
            ContextMenuDG.Items.Add(MI_Letter);
        }

        /// <summary>Редактор</summary>
        public void LoadEditor()
        {
            ShowSelect.IsEnabled = false;
            DG.Columns[6].Visibility = Visibility.Visible;
            DG.Columns[7].Visibility = Visibility.Hidden;
            DG.Columns[8].Visibility = Visibility.Hidden;

            ClearItemSource();

            var objects = Helpers.LogicHelper.ObjecteLogic.Find(DateControl_Class.SelectMonth, true);

            {
                var dictionary = new Dictionary<uint, ThisShow>();

                foreach (var obj in objects)
                {
                    var thisShow = new ThisShow(obj);

                    Values.Add(thisShow);

                    dictionary.Add(obj.ID, thisShow);
                }

                var negotiationAssistants = Helpers.LogicHelper.NegotiationAssistantLogic.Find(DateControl_Class.SelectMonth);

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
            if ((DG.Items[e.Row.GetIndex()] as IThisShow).NegotiationAssistandID > 0)
            { e.Row.Background = Brushes.Green; }
            else
            { e.Row.Background = Brushes.White; }
        }
        private void DG_Show_Add(object sender, MouseButtonEventArgs e)
        {
            if (DG.SelectedIndex == -1)
            { return; }
            var elem = (DG.Items[DG.SelectedIndex] as IThisShow);
            if (elem.NegotiationAssistandID > 0)
            {
                MyTools.DeleteRowsByID(G.NegotiationAssistant, false, elem.NegotiationAssistandID);
                elem.NegotiationAssistandID = 0;
            }
            else
            {
                if ((bool)G.NegotiationAssistant.QUERRY().EXIST
                    .WHERE
                    .AC(C.NegotiationAssistant.Objecte).EQUI.BV(elem.ID)
                    .AND
                    .AC(C.NegotiationAssistant.YM).EQUI.BV(DateControl_Class.SelectMonth).DO()[0].Value
                    )
                {
                    throw new System.Exception($"Negotiation assistand allready exist:\r\nobject id = {elem.ID}, ym = {DateControl_Class.SelectMonth}");
                }
                else
                {
                    elem.NegotiationAssistandID = MyTools.AddRowFromTable(true, G.NegotiationAssistant,
                      new KeyValuePair<int, object>(C.NegotiationAssistant.Objecte, elem.ID),
                      new KeyValuePair<int, object>(C.NegotiationAssistant.YM, DateControl_Class.SelectMonth));
                }
            }

            DataGridRow row = (DataGridRow)DG.ItemContainerGenerator.ContainerFromIndex(DG.SelectedIndex);
            row.Background = (DG.Items[row.GetIndex()] as IThisShow).NegotiationAssistandID > 0 ? Brushes.Green : Brushes.White;
            DG.SelectedIndex = DG.SelectedIndex + (DG.SelectedIndex != DG.Items.Count - 1 ? 1 : -1);
            text.Text = $"План: {Values.Where(x => (x as IThisShow).NegotiationAssistandID > 0).Count()}";
        }
        public class ThisShow
            : Deff, Selector.IThisShow
        {
            public ThisShow(Objecte obj)
            {
                this.obj = obj;
                result = new Dictionary<string, string>();
                result.Add("Наименование", Name);
                result.Add("Адрес", Adres);
            }

            public uint NegotiationAssistandID { get; set; }
        }

        #region Доп контент

        private SelectionWell[] LoadSample(uint[] IDObj)
        {
            var query = G.SelectionWell.QUERRY()
                .SHOW
                .WHERE
                .ID(0);
            foreach (var one in IDObj)
            { query.OR.ARC(C.SelectionWell.Well, C.Well.Object).EQUI.BV(one); }
            query.DO();
            SelectionWell[] result = new SelectionWell[G.SelectionWell.Rows.Count];
            for (int i = 0; i < result.Length; i++)
            { result[i] = Helpers.LogicHelper.SelectionWellLogic.FirstModel(G.SelectionWell.Rows.GetID(i)); }
            return result.GroupBy(x => x.ObjectID).Select(x => x.OrderBy(y => y.YMDHM).Last()).ToArray();
        }
        public abstract class Deff : ISearch
        {
            public void LoadSample(uint sampleID)
            {
                sample = Helpers.LogicHelper.SampleLogic.FirstModel(sampleID);
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
            uint[] Dumps => obj.Wells.GroupBy(x => x.UnitID).Select(x => x.Key).Where(x => ListUnit.Contains(x)).ToArray();
            public string DumpPool => Dumps.Length > 0 ? Dumps.Select(x => Helpers.LogicHelper.UnitLogic.FirstModel(x)).Select(x => x.Name).Aggregate((a, b) => a + "," + b) : string.Empty;
        }
        public static List<uint> ListUnit;

        #endregion
    }
}