using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoTable;
using MAC_2.Model;

namespace MAC_2.Employee.HelpSelect
{
    /// <summary>
    /// Логика взаимодействия для SampleAdd_Window.xaml
    /// </summary>
    public partial class SampleAdd_Window : Window
    {
        public SampleAdd_Window(uint negotiationAssistantID)
        {
            InitializeComponent();

            negotiationAssistant = Helpers.LogicHelper.NegotiationAssistantLogic.FirstModel(negotiationAssistantID);

            if (negotiationAssistant.WorkerID == 0)
            {
                MessageBox.Show("Не выбран пробоотборщик!");
                this.Activated += (sender, e) => Close();
            }

            WH = new MyTools.C_MinMaxWidthHeight(MinWidth: 80);
            font = new MyTools.S_FontControl(18);
            EditNegotiationAssistant.SetRowFromGrid(MyTools.GL_Auto);
            EditNegotiationAssistant.SetFromGrid(negotiationAssistant.GetEditor(new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical, Scope: true, Color: Brushes.LightSkyBlue),
                new MyTools.C_DefColumn(C.NegotiationAssistant.Objecte, WH, false, font: font),
                new MyTools.C_DefColumn(C.NegotiationAssistant.Sample, WH, false, font: font),
                new MyTools.C_DefColumn(C.NegotiationAssistant.Worker, WH, font: font),
                new MyTools.C_DefColumn(C.NegotiationAssistant.YMD, WH, font: font)
                ), 0);
            LoadWell();
        }

        private void LoadWell()
        {
            var wps = new List<WrapPanel>();

            if (negotiationAssistant.GetIDValue(C.NegotiationAssistant.Sample) == 0)
            {
                foreach (var well in negotiationAssistant.Objecte.Wells)
                {
                    var selectionWellVM = new SelectionWellViewModel(negotiationAssistant, well);

                    WrapPanel wp = new WrapPanel();
                    wps.Add(wp);

                    wp.Margin = new Thickness(5);
                    wp.Name = $"wp{well.Number}";

                    TextBlock Sample = new TextBlock();
                    Sample.Margin = new Thickness(2);
                    Sample.Padding = new Thickness(3);
                    Sample.FontSize = 18;
                    Sample.Text = $"Введите номер отбора для колодца \"{well.PresentNumber}\"";

                    TextBox tbxSampleNumber = new TextBox();
                    tbxSampleNumber.Margin = new Thickness(2);
                    tbxSampleNumber.Padding = new Thickness(3);
                    tbxSampleNumber.TextAlignment = TextAlignment.Left;
                    tbxSampleNumber.MinWidth = 50;
                    tbxSampleNumber.TextChanged += (sender, e) => DataBase.NoABC_Int_Dinamic(tbxSampleNumber);
                    tbxSampleNumber.BorderThickness = new Thickness(2);
                    tbxSampleNumber.KeyDown += (sender, e) =>
                    {
                        if (e.Key == Key.Enter)
                        {
                            SetSample(selectionWellVM, tbxSampleNumber);
                        }
                    };

                    UpdateControlColor(tbxSampleNumber, 0);

                    Button btSetSample = new Button();
                    btSetSample.Margin = new Thickness(2);
                    btSetSample.Padding = new Thickness(3);
                    btSetSample.Content = "Занести номер";
                    btSetSample.Click += (sender, e) =>
                    {
                        SetSample(selectionWellVM, tbxSampleNumber);
                    };

                    wp.Children.Add(Sample);
                    wp.Children.Add(tbxSampleNumber);
                    wp.Children.Add(btSetSample);

                    EditNegotiationAssistant.SetRowFromGrid(MyTools.GL_Auto);
                    EditNegotiationAssistant.SetFromGrid(wp);
                }
            }
            else
            { EditorSample(); }

            Button bt = new Button();
            EditNegotiationAssistant.SetRowFromGrid(MyTools.GL_Auto);
            EditNegotiationAssistant.SetFromGrid(bt);
            bt.Content = "Подтвердить выбор колодцев";
            bt.Click += (sender, e) =>
            {
                if (negotiationAssistant.SampleID == 0)
                {
                    MessageBox.Show("Колодцы не заполнены");
                    return;
                }

                foreach (var wp in wps)
                { wp.Children.Clear(); }

                EditNegotiationAssistant.KillControl(bt);
                //EditNegotiationAssistant.KillControl(new Objecte(negotiationAssistant.IDObjecte, true).Wells.Select(x => $"wp{x.Number}").ToArray());
                EditNegotiationAssistant.RowDefinitions.RemoveRange(1, EditNegotiationAssistant.RowDefinitions.Count - 1);
                EditorSample();
            };
        }

        private bool SetSample(SelectionWellViewModel swVM, TextBox tb)
        {
            int newNumber;
            bool result = false;

            if (String.IsNullOrEmpty(tb.Text))
            { tb.Text = (LastNumber + 1).ToString(); }

            if (int.TryParse(tb.Text, out newNumber))
            {
                if (SetSelectionWell(swVM, newNumber))
                {
                    result = true;
                    tb.IsEnabled = false;
                }
                else
                { newNumber = 0; }
            }
            else
            {
                MessageBox.Show("Значение не является номером!");
                newNumber = 0;
            }

            UpdateControlColor(tb, newNumber);

            return result;
        }

        private bool SetSelectionWell(SelectionWellViewModel swVM, int number)
        {
            if (!IsUsedNumber(number, negotiationAssistant.SampleID))
            {
                swVM.Number = number;
                return true;
            }
            else
            {
                MessageBox.Show("Номер отбора уже занят!");
                return false;
            }
        }

        private void EditorSample()
        {
            EditNegotiationAssistant.SetRowFromGrid(MyTools.GL_Auto);
            Sample sample = Helpers.LogicHelper.SampleLogic.FirstModel(negotiationAssistant.SampleID);
            EditNegotiationAssistant.SetFromGrid(sample.GetEditor(new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical, Scope: true, Color: Brushes.LightSeaGreen),
                new MyTools.C_DefColumn(C.Sample.Representative, WH, font: font,
                Click: (sender, e) =>
                {
                    G.Representative.QUERRY().SHOW.WHERE.C(C.Representative.Client, negotiationAssistant.GetIDValue(C.NegotiationAssistant.Objecte, C.Objecte.Client)).DO();
                    List<C_ColumnFromSearch> column = new List<C_ColumnFromSearch>();
                    column.Add(new C_ColumnFromSearch(C.Representative.FIO));
                    column.Add(new C_ColumnFromSearch(C.Representative.Post));
                    SearchGrid_Window search = new SearchGrid_Window(G.Representative, null, column, new C_SettingSearchDataGrid(DefDeleg: true));
                    MenuItem miAdd = new MenuItem();
                    miAdd.Header = "Добавить представителя";
                    miAdd.Click += (sender1, e1) =>
                    {
                        var clientID = negotiationAssistant.Objecte.ClientID;

                        G.Client.QUERRY().SHOW.WHERE.ID(clientID).DO();

                        var addForm = new Edit_Form(G.Representative, 500, false);
                        addForm.LoadData();
                        addForm.controls[C.Representative.Client].Default = clientID;
                        addForm.controls[C.Representative.Client].obj.Enabled = false;
                        addForm.ShowDialog();

                        search.SelectID = addForm.ID;
                        search.Close();
                    };
                    search.ThisMenu.Items.Add(miAdd);
                    search.ShowDialog();

                    if (search.SelectID > 0)
                    { sample.SetValue(C.Sample.Representative, search.SelectID); }
                })));
            G.SelectionWell.QUERRY().SHOW.WHERE.C(C.SelectionWell.Sample, negotiationAssistant.SampleID).DO();
            uint[] SelectionWells = new uint[G.SelectionWell.Rows.Count];
            for (int i = 0; i < SelectionWells.Length; i++)
            { SelectionWells[i] = G.SelectionWell.Rows.GetID(i); }
            foreach (var one in SelectionWells.Select(x => Helpers.LogicHelper.SelectionWellLogic.FirstModel(x)))
            {
                EditNegotiationAssistant.SetRowFromGrid(MyTools.GL_Auto);
                EditNegotiationAssistant.SetFromGrid(one.GetEditor(new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical, Scope: true, Color: Brushes.LightSkyBlue),
                    new MyTools.C_DefColumn(C.SelectionWell.Well, WH, false, font: font),
                    new MyTools.C_DefColumn(C.SelectionWell.Number, WH, data.User<bool>(C.User.CanRedact), font: font),
                    new MyTools.C_DefColumn(C.SelectionWell.DateTime, WH, font: font)));
            }
        }

        /// <summary>
        /// Последний использованый номер отбора
        /// </summary>
        public int LastNumber
        {
            get
            {
                int max = int.MinValue;

                G.SelectionWell.QUERRY()
                .SHOW
                .WHERE
                .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(DateControl_Class.SelectMonth)
                .DO();

                int count = G.SelectionWell.Rows.Count;

                for (int i = 0; i < count; i++)
                {
                    if (G.SelectionWell.Rows.Get<int>(i, C.SelectionWell.Number) > max)
                    { max = G.SelectionWell.Rows.Get<int>(i, C.SelectionWell.Number); }
                }

                return max;
            }
        }

        /// <summary>
        /// Был ли использован номер отбора в текущем периоде
        /// </summary>
        /// <param name="number">Номер</param>
        /// <param name="excludeSelectionWellID">Исключить Sample</param>
        /// <returns></returns>
        private static bool IsUsedNumber(int number, uint excludeSelectionWellID = 0)
        {
            bool exist;

            if (excludeSelectionWellID == 0)
            {
                exist = (bool)G.SelectionWell.QUERRY()
                     .EXIST
                     .WHERE
                     .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(DateControl_Class.SelectMonth)
                     .AND.C(C.SelectionWell.Number, number)
                     .DO()[0].Value;
            }
            else
            {
                exist = (bool)G.SelectionWell.QUERRY()
                     .EXIST
                     .WHERE
                     .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(DateControl_Class.SelectMonth)
                     .AND.C(C.SelectionWell.Number, number)
                     .AND.NOT.C(C.SelectionWell.Sample, excludeSelectionWellID)
                     .DO()[0].Value;
            }

                return exist;
        }

        private static bool CheckNumber(int number)
        {
            return number > 0;
        }

        private void UpdateControlColor(System.Windows.Controls.Control control, int number)
        {
            if (CheckNumber(number))
            { control.Background = new SolidColorBrush(Colors.Green); }
            else
            { control.Background = new SolidColorBrush(Colors.Red); }
        }

        MyTools.S_FontControl font;
        MyTools.C_MinMaxWidthHeight WH;
        NegotiationAssistant negotiationAssistant;
        private void Cancle_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}