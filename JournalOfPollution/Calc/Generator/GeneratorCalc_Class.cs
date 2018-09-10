using AutoTable;
using MAC_2.Employee.Mechanisms;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MAC_2.Model;

namespace MAC_2.Calc
{
    public class GeneratorCalc_Class
    {
        public GeneratorCalc_Class(TabControl TC, EViewType ViewType)
        {
            this.TC = TC;
            LoadTC(ViewType);
        }
        public enum EViewType { Pollution, Group }
        TabControl TC;
        MyTools.C_SettingFromRowEdit SFE;
        public void LoadCoefficient()
        {
            TC.Items.Clear();
            PollutionBase_Class.LoadCoefficient(true);
            TabItem TI = new TabItem();
            TI.Header = "Коэффициенты";
            TC.Items.Add(TI);
            TC.SelectedIndex = 0;
            Grid grid = new Grid();
            grid.SetColumnsFromGrid(2, MyTools.GL_Auto);
            TI.Content =MyTools.VerticalHorisontalScroll(grid);

            var list = Helpers.LogicHelper.CoefficientLogic.Find();
            foreach (var one in list)
            {
                grid.SetRowFromGrid(MyTools.GL_Auto);
                grid.SetFromGrid(one.GetEditor
                    (
                    SFE,
                    new MyTools.C_DefColumn(C.Coefficient.Pollution, size),
                    new MyTools.C_DefColumn(C.Coefficient.Resolution, size),
                    new MyTools.C_DefColumn(C.Coefficient.Compare, size),
                    new MyTools.C_DefColumn(C.Coefficient.YMFrom, size),
                    new MyTools.C_DefColumn(C.Coefficient.YMTo, size)
                    ), Column: 0);
                WrapPanel WP = new WrapPanel();
                WP.Orientation = Orientation.Vertical;
                grid.SetFromGrid(WP);
                foreach (var val in one.CoefficientValues)
                {
                    List<MyTools.C_DefColumn> columns = new List<MyTools.C_DefColumn>();
                    if (one.Compare)
                    {
                        var Poll = PollutionBase_Class.AllPolutions.FirstOrDefault(x => x.ID == one.PollutionID);
                        if (Poll != null)
                        {
                            if (Poll.HasRange)
                            { columns.Add(new MyTools.C_DefColumn(C.CoefficientValue.From, size, true, Poll.Round)); }
                            columns.Add(new MyTools.C_DefColumn(C.CoefficientValue.To, size, true, Poll.Round));
                        }
                    }
                    columns.Add(new MyTools.C_DefColumn(C.CoefficientValue.Value, size));
                    WP.Children.Add(val.GetEditor(SFE, columns.ToArray()));
                }
                Button AddVal = new Button();
                AddVal.Content = "Добавить";
                AddVal.Click += (sender, e) =>
                  {
                      MyTools.AddRowFromTable(G.CoefficientValue, new KeyValuePair<int, object>(C.CoefficientValue.Coefficient, one.ID));
                      LoadCoefficient();
                  };
                WP.Children.Add(AddVal);
            }
            grid.SetRowFromGrid(MyTools.GL_Auto);
            Button Add = new Button();
            Add.Content = "Добавить";
            Add.Click += (sender, e) =>
            {
                MyTools.AddRowFromTable(G.Coefficient, new KeyValuePair<int, object>(C.Coefficient.Compare, false));
                LoadCoefficient();
            };
            grid.SetFromGrid(Add,Column:0);
        }
        private void LoadTC(EViewType ViewType)
        {
            size = new MyTools.C_MinMaxWidthHeight(180, Wrap: true, MinMaxName: 120);
            SFE = new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical, true, -1, true, false, true);
            TC.Items.Clear();

            switch (ViewType)
            {
                case EViewType.Pollution:
                    {
                        var pollutions = Helpers.LogicHelper.PollutionLogic.Find();

                        foreach (var pollution in pollutions)
                        {
                            TabItem TI = new TabItem();
                            TI.Header = pollution.CurtName;
                            TI.Content = MyTools.VerticalHorisontalScroll_From_Control(LoadBox(pollution));
                            TC.Items.Add(TI);
                        }
                        break;
                    }
                case EViewType.Group:
                    {
                        var norms = Helpers.LogicHelper.ResolutionClarifyLogic.Find(DateControl_Class.SelectMonth);

                        foreach (var one in PollutionBase_Class.AllCalculationFormul.GroupBy(x => x.ResolutionClarifyID))
                        {
                            TabItem TI = new TabItem();
                            TI.Header = PollutionBase_Class.AllResolution.First(x => x.ListResolutionClarify.Count(y => y.ID == one.Key) > 0).CurtName;
                            TI.Content = MyTools.VerticalHorisontalScroll_From_Control(LoadBox(one.ToArray()));
                            TC.Items.Add(TI);
                        }
                        break;
                    }
            }
        }
        MyTools.C_MinMaxWidthHeight size;
        private WrapPanel LoadBox(Pollution pollution)
        {
            WrapPanel WP = new WrapPanel();
            foreach (var one in PollutionBase_Class.AllCalculationFormul.Where(x => x.PollutionID == pollution.ID).Select(x => Helpers.LogicHelper.CalculationFormulaLogic.FirstModel(x.ID)))
            {              
                WP.Children.Add(one.GetEditor
                    (
                    SFE,
                    new MyTools.C_DefColumn(C.CalculationFormula.Pollution, size, false),
                    new MyTools.C_DefColumn(C.CalculationFormula.ResolutionClarify, size),
                    new MyTools.C_DefColumn(C.CalculationFormula.YM, size),
                    new MyTools.C_DefColumn(C.CalculationFormula.Number, size),
                    new MyTools.C_DefColumn(C.CalculationFormula.GettingValueFormula, size),
                    new MyTools.C_DefColumn(C.CalculationFormula.Formula, size, false, -1, new System.Windows.Input.MouseButtonEventHandler((sender, e) => { new FormulaCreator_Window(one).ShowDialog(); })),
                    new MyTools.C_DefColumn(C.CalculationFormula.GettingValueLink, size),
                    new MyTools.C_DefColumn(C.CalculationFormula.Label, size)

                    ));
                Rectangle rec = new Rectangle();
                rec.Stroke = Brushes.Aqua;
                (WP.Children[WP.Children.Count-1] as Grid).SetFromGrid(rec,0,0,int.MaxValue,int.MaxValue);
            }
            Button BT = new Button();
            BT.Content = "Добавить";
            BT.Click += (sender, e) =>
              {
                  selectPollution = TC.SelectedIndex;
                  G.CalculationFormula.QUERRY()
                  .ADD
                  .C(C.CalculationFormula.Pollution, pollution.ID)
                  .DO();
                  LoadTC(EViewType.Pollution);
                  TC.SelectedIndex=selectPollution;
              };
            WP.Children.Add(BT);
            return WP;
        }
        int selectPollution;
        private TabControl LoadBox(CalculationFormula[] formuls)
        {
            TabControl result = new TabControl();
            foreach (var one in formuls.GroupBy(x => x.Number))
            {
                TabItem TI = new TabItem();
                TI.Header = one.First().Number;
                WrapPanel WP = new WrapPanel();
                TI.Content = WP;
                foreach (var form in one)
                {
                    WP.Children.Add(form.GetEditor
                        (
                        SFE,
                        new MyTools.C_DefColumn(C.CalculationFormula.Pollution, size, false),
                        new MyTools.C_DefColumn(C.CalculationFormula.ResolutionClarify, size),
                        new MyTools.C_DefColumn(C.CalculationFormula.YM, size),
                        new MyTools.C_DefColumn(C.CalculationFormula.Number, size),
                        new MyTools.C_DefColumn(C.CalculationFormula.GettingValueFormula, size),
                        new MyTools.C_DefColumn(C.CalculationFormula.Formula, size, false, -1, new System.Windows.Input.MouseButtonEventHandler((sender, e) => { new FormulaCreator_Window(form).ShowDialog(); })),
                        new MyTools.C_DefColumn(C.CalculationFormula.GettingValueLink, size)
                        ));
                    Rectangle rec = new Rectangle();
                    rec.Stroke = Brushes.Aqua;
                    (WP.Children[WP.Children.Count - 1] as Grid).SetFromGrid(rec, 0, 0, int.MaxValue, int.MaxValue);
                }
                result.Items.Add(TI);
            }
            return result;
        }
    }
}