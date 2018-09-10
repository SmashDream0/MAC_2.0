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
using MAC_2.Employee.Mechanisms;
using MAC_2.Calc;
using MAC_2.Model;

namespace MAC_2.Employee.EditSample
{
    /// <summary>
    /// Логика взаимодействия для EditValue.xaml
    /// </summary>
    public partial class EditValue_Window : Window
    {
        public EditValue_Window(SelectionWell selectionWell)
        {
            InitializeComponent();
            this.GetSetting();
            this.selectionWell = selectionWell;
            this.obj = selectionWell.Objecte;
            this._values = selectionWell.ValueSelections.ToArray();
            this.CanEdit = data.User<bool>(C.User.CanRedact);
            SFE = new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical, true, -1, true, false, true);
            DrawObject();
            ShowAct.IsEnabled = CanEdit;

            DrawColumns();
            DrawValue();

            LoadAct();
        }

        bool CanEdit;
        Objecte obj;
        SelectionWell selectionWell;
        ValueSelection[] _values;
        MyTools.C_SettingFromRowEdit SFE;

        #region Данные объекта
        private void DrawObject()
        {
            ObjectGrid.ColumnDefinitions.Clear();
            ObjectGrid.RowDefinitions.Clear();
            ObjectGrid.Children.Clear();
            var MM_WH = new MyTools.C_MinMaxWidthHeight(250, Wrap: true, MinMaxName: 140);

            ObjectGrid.SetRowFromGrid(MyTools.GL_Auto);
            ObjectGrid.SetFromGrid(this.selectionWell.Well.GetEditor
                (
                SFE,
                new MyTools.C_DefColumn(C.Well.Number, MM_WH, false)
                ));
            ObjectGrid.SetRowFromGrid(MyTools.GL_Auto);
            ObjectGrid.SetFromGrid(this.selectionWell.Well.GetEditor
                (
                SFE,
                new MyTools.C_DefColumn(C.SelectionWell.DateTime, MM_WH, false)
                ));

            ObjectGrid.SetRowFromGrid(MyTools.GL_Auto);
            ObjectGrid.SetFromGrid(obj.Client.Detail.GetEditor
                (
                SFE,
                new MyTools.C_DefColumn(C.DetailsClient.FullName, MM_WH, false)
                ));
            ObjectGrid.SetRowFromGrid(MyTools.GL_Auto);
            ObjectGrid.SetFromGrid(obj.GetEditor
                (
                SFE,
                new MyTools.C_DefColumn(C.Objecte.AdresFact, MM_WH, false),
                new MyTools.C_DefColumn(C.Objecte.NumberFolder, MM_WH, false)
                ));

            //нужно пихнуть объёмы
        }
        #endregion

        #region Значения

        List<column> columns;

        struct column
        {
            public column(string name, string header, bool value = false, bool readOnly = true)
            {
                this.bind = new Binding(name);

                if (value)
                { this.bind.Path = new PropertyPath("values[" + name + "].Value"); }

                //this.bind.Mode = BindingMode.OneWay;
                this.bind.Mode = readOnly ? BindingMode.OneWay : BindingMode.TwoWay;
                this.Header = header;
                this.ReadOnly = readOnly;
            }

            public Binding bind;
            public string Header;
            bool ReadOnly;
        }

        private void DrawColumns()
        {
            dgValues.Columns.Clear();

            var pollutions = Helpers.LogicHelper.PollutionLogic.Find();

            {
                var column = new DataGridTextColumn();
                column.Header = "Наименование";
                column.Binding = new Binding("Name");

                dgValues.Columns.Add(column);
            }

            foreach (var pollution in pollutions)
            {
                var column = new DataGridTextColumn();
                column.Header = pollution.CurtName.SymbolConverter();

                {
                    var propertyPath = $"Values[{pollution.BindName}].Value";

                    column.Binding = new Binding(propertyPath); ;
                }

                dgValues.Columns.Add(column);
            }

            dgValues.FrozenColumnCount = 1;
        }

        private void DrawValue()
        {
            var showList = new List<IThisWell>();

            var pollutions = Helpers.LogicHelper.PollutionLogic.Find();
            var defaultValues = getDefaultValues();
            var values = pollutions.Select(x => new MiddleValue(x.CurtName)).ToArray();

            {
                var calc = new Calc_644(_values, obj, PollutionBase_Class.AllResolution.First(x => x.CurtName == "644"));

                var sample = selectionWell.Sample;

                foreach (var selectionWell in sample.SelectionWells)
                {
                    {
                        var selectionWellViewModel = new ThisWell(selectionWell, pollutions.Select(x => new SelectionValueInternal(selectionWell, x)).ToArray());

                        foreach (var selectionValue in selectionWell.ValueSelections)
                        {
                            var pollutionIndex = selectionValue.Pollution.Index;

                            selectionWellViewModel.Values[pollutionIndex] = new SelectionValueInternal(selectionValue);
                        }

                        showList.Add(selectionWellViewModel);
                    }

                    {
                        var declaration = selectionWell.Well.Declaration;
                        var declarationViewModel = new ThisCalc("Декларация", pollutions.Select(x => new DeclarationValueInternal(declaration, x)).ToArray());

                        if (declaration != null)
                        {

                            foreach (var declarationValue in declaration.DeclarationValues)
                            {
                                if (declarationValue.PollutionID > 0)
                                {
                                    declarationViewModel.Values[declarationValue.Pollution.Index] = new DeclarationValueInternal(declarationValue);
                                }
                            }
                        }

                        showList.Add(declarationViewModel);
                    }

                    {
                        var selectionViewModel = new ThisCalc("Выбран", defaultValues.ToArray());

                        foreach (var selectionValue in calc.ValueFromResolution(selectionWell.ValueSelections))
                        {
                            var pollutionIndex = selectionValue.Pollution.Index;
                            var value = Math.Round(selectionValue.Value, selectionValue.Pollution.Round);

                            selectionViewModel.Values[pollutionIndex] = new ValueCalc(selectionValue.Pollution.CurtName, value);
                            values[pollutionIndex].Add(value);
                        }

                        showList.Add(selectionViewModel);
                    }
                }
            }

            {
                var middleViewModel = new ThisCalc("Среднее", values);

                showList.Add(middleViewModel);
            }


            dgValues.ItemsSource = showList.ToArray();
        }

        private static IValue[] getDefaultValues()
        {
            var pollutions = Helpers.LogicHelper.PollutionLogic.Find();

            var result = pollutions.Select(x => new ValueCalc(x.CurtName)).ToArray();

            return result;
        }

        class ThisWell : IThisWell
        {
            public ThisWell(SelectionWell selectionWell, IValue[] Values)
            {
                this._selectionWell = selectionWell;
                this.Values = Values;
            }
            public uint WellID => _selectionWell.WellID;
            public readonly SelectionWell _selectionWell;
            public string Name => T.Well.Rows.Get<string>(_selectionWell.WellID, C.Well.TypeWell, C.TypeWell.CurtName)
                + ' ' + T.Well.Rows.Get<string>(_selectionWell.WellID, C.Well.Number);
            public IValue[] Values { get; }
        }

        class ThisCalc : IThisWell
        {
            public ThisCalc(string name, IValue[] values)
            {
                this.Name = name;
                Values = values;
            }
            public string Name { get; }
            public IValue[] Values { get; }
        }

        interface IThisWell
        {
            string Name { get; }
            IValue[] Values { get; }
        }

        #endregion

        #region Акты

        private void LoadAct()
        {
            ShowAct.Children.Clear();
            ShowAct.RowDefinitions.Clear();
            ShowAct.ColumnDefinitions.Clear();
            ShowAct.SetRowFromGrid(MyTools.GL_Auto);
            Button add = new Button { Content = "Добавить акт" };

            add.Click += Act_Click;

            ShowAct.SetFromGrid(add);

            var normDocs = Helpers.LogicHelper.NormDocLogic.Find(selectionWell.SampleID);

            foreach(var normDoc in normDocs)
            {
                ShowAct.SetRowFromGrid(MyTools.GL_Auto);
                
                ShowAct.SetFromGrid(normDoc.GetEditor(new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical)));
            }
        }

        void Act_Click(object sender, RoutedEventArgs e)
        {
            var _querry = G.Volume.QUERRY()
            .SHOW
            .WHERE
            .C(C.Volume.Sample, selectionWell.SampleID);
            _querry.DO();

            if (G.Volume.Rows.Count > 0)
            {
                SearchGrid_Window sg = new SearchGrid_Window(G.Volume, null, null, new C_SettingSearchDataGrid(DefDeleg: true));
                sg.ShowDialog();

                if (sg.SelectID > 0)
                {
                    var table = T.NormDoc.CreateSubTable();

                    table.QUERRY().SHOW.WHERE.ARC(C.NormDoc.Volume, C.Volume.Sample).EQUI.BV(selectionWell.SampleID).DO();

                    MyTools.AddRowFromTable(table, new KeyValuePair<int, object>(C.NormDoc.Volume, sg.SelectID));
                }

                LoadAct();
            }
            else
            { MessageBox.Show("Объёмы не указаны"); }
        }

        #endregion
    }
}