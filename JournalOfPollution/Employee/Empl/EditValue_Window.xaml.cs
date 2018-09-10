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

namespace MAC_2.Employee.Empl
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

            var pollutions = Logic.LogicInstances.PollutionLogic.Find();

            {
                var column = new DataGridTextColumn();
                column.Header = "Наименование";
                column.Binding = new Binding("Name");
                column.IsReadOnly = true;

                dgValues.Columns.Add(column);
            }

            foreach (var pollution in pollutions)
            {
                var column = new DataGridTextColumn();
                column.Header = pollution.CurtName.SymbolConverter();

                {
                    var bind = new Binding($"Values[{pollution.BindName}].Value");
                    bind.StringFormat = "0.#################";

                    column.Binding = bind;
                }

                dgValues.Columns.Add(column);
            }

            dgValues.FrozenColumnCount = 1;
        }

        private void DrawValue()
        {
            var showList = new List<IThisWell>();
            var defaultValues = getDefaultValues();

            var sample = selectionWell.Sample;
            var pollutions = Logic.LogicInstances.PollutionLogic.Find();

            var calc = new Calc_644(sample, obj, PollutionBase_Class.AllResolution.First(x => x.CurtName == "644"));

            var middles = pollutions.Select(x => new MiddleValue(x.CurtName, x.Index)).ToArray();

            foreach (var selectionWell in sample.SelectionWells)
            {
                {
                    var selectionWellViewModel = new SelectionWellViewModel(selectionWell.Well.ShortName, defaultValues.ToArray());

                    foreach (var selectionValue in selectionWell.ValueSelections)
                    {
                        selectionWellViewModel.Values[selectionValue.Pollution.Index].Value = selectionValue.Value;
                    }

                    showList.Add(selectionWellViewModel);
                }

                {
                    var declarationViewModel = new NamedValues("Декларация", defaultValues.ToArray());

                    var declaration = selectionWell.Well.Declaration;

                    foreach (var declarationValue in declaration.DeclarationValues)
                    {
                        declarationViewModel.Values[declarationValue.Pollution.Index] = new NamedValue(declarationValue.Pollution.CurtName, declarationValue.Pollution.GetRounded(declarationValue.To));
                    }

                    showList.Add(declarationViewModel);
                }

                {
                    var selectedValues = new NamedValues("Выбран", defaultValues.ToArray());

                    var values = calc.ValueFromResolution(selectionWell.ValueSelections);

                    foreach (var value in values)
                    {
                        var pollutionIndex = value.Pollution.Index;
                        var resultValue = value.Pollution.GetRounded(value.Value);

                        selectedValues.Values[pollutionIndex].Value = resultValue;
                    }

                    foreach (var middle in middles)
                    {
                        var pollutionIndex = middle.Index;

                        middles[pollutionIndex].AddValue(selectedValues.Values[pollutionIndex]);

                    }

                    showList.Add(selectedValues);
                }
            }

            {
                var middleValues = new NamedValues("Среднее", middles);

                showList.Add(middleValues);
            }

            dgValues.ItemsSource = showList.ToArray();
        }

        private static IValue[] getDefaultValues()
        {
            var pollutions = Logic.LogicInstances.PollutionLogic.Find();

            var result = pollutions.Select(x => new NamedValue(x.CurtName)).ToArray();

            return result;
        }

        class SelectionWellViewModel : IThisWell
        {
            public SelectionWellViewModel(string name, IValue[] Values)
            {
                this._name = name;
                this.Values = Values;
            }

            string _name;
            public string Name => _name;
            public IValue[] Values { get; }
        }

        class NamedValues : IThisWell
        {
            public NamedValues(string name, IValue[] values)
            {
                this._name = name;
                Values = values;
            }

            string _name;
            public string Name => _name;
            public IValue[] Values { get; }
        }

        class NamedValue : IValue
        {
            public NamedValue(string name, decimal value)
            {
                this.Value = value;
                this.Name = name;
            }

            public NamedValue(string name)
            {
                this.Value = null;
                this.Name = name;
            }

            public virtual decimal? Value { get; set; }
            public string Name { get; private set; }

            public override string ToString()
            {
                return $"{Name} = {Value}";
            }
        }

        class MiddleValue : NamedValue
        {
            public MiddleValue(string name, int index)
                :base(name)
            {
                Index = index;
            }

            public int Index
            { get; private set; }

            public decimal Summ
            { get; private set; }

            public int Count
            { get { return values.Where(x => x.Value > 0).Count(); } }

            public override decimal? Value
            {
                get
                {
                    if (Count > 0)
                    { return values.Sum(x => x.Value ?? 0) / Count; }
                    else
                    { return null; }
                }
                set
                { }
            }

            List<IValue> values = new List<IValue>();

            public void AddValue(IValue value)
            {
                values.Add(value);
            }
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
            //var samples = Values.Select(x => Logic.LogicInstances.SampleLogic.FirstOrDefault(x.SampleID)).ToArray();\
            var samples = selectionWell.Sample.SelectionWells.ToArray();
            add.Click += (sender, e) =>
              {
                  var _querry = G.Volume.QUERRY()
                  .SHOW
                  .WHERE
                  .C(C.Volume.Sample, samples.First().ID);
                  for (int i = 1; i < samples.Length; i++)
                  { _querry.OR.C(C.Volume.Sample, samples[i].ID); }
                  _querry.DO();
                  SearchGrid_Window sg = new SearchGrid_Window(G.Volume, null, null, new C_SettingSearchDataGrid(DefDeleg: true));
                  sg.ShowDialog();
                  if (sg.SelectID > 0)
                  { MyTools.AddRowFromTable(G.NormDoc, new KeyValuePair<int, object>(C.NormDoc.Volume, sg.SelectID)); }
                  LoadAct();
              };
            ShowAct.SetFromGrid(add);
            if (samples.Any())
            {
                var querry = G.NormDoc.QUERRY()
                    .SHOW
                    .WHERE
                    .ARC(C.NormDoc.Volume, C.Volume.Sample, C.Sample.YM).EQUI.BV(DateControl_Class.SelectMonth);
                querry.AND.OB().ARC(C.NormDoc.Volume, C.Volume.Sample).EQUI.BV(samples.First().ID);
                for (int i = 1; i < samples.Length; i++)
                { querry.OR.ARC(C.NormDoc.Volume, C.Volume.Sample).EQUI.BV(samples[i].ID); }
                querry.DO();

                List<NormDoc> normDocs = new List<NormDoc>();
                int count = G.NormDoc.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    ShowAct.SetRowFromGrid(MyTools.GL_Auto);
                    normDocs.Add(Logic.LogicInstances.NormDocLogic.FirstOrDefault(G.NormDoc.Rows.GetID(i)));
                    ShowAct.SetFromGrid(normDocs.Last().GetEditor(new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical)));
                }
            }
        }

        #endregion
    }
}