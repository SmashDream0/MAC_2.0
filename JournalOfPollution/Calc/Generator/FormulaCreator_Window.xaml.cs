using AutoTable;
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
using MAC_2.Model;

namespace MAC_2.Calc
{
    /// <summary>
    /// Логика взаимодействия для FormulaCreator_Window.xaml
    /// </summary>
    public partial class FormulaCreator_Window : Window
    {
        public FormulaCreator_Window(CalculationFormula CalcFormula)
        {
            CalcFormula = Helpers.LogicHelper.CalculationFormulaLogic.FirstModel(CalcFormula.ID);
            InitializeComponent();
            this.GetSetting();
            Helper.Text = "Фактическая концентрация = \"FK\"\n" +
                "Допустимая концентрация(НОРМА) = \"DK\"\n" +
                "Тариф = \"T\"\n" +
                "Тариф загрязнения = \"TP\"\n" +
                "Объём = \"Q\"\n" +
                "Коэффициент = \"K\"";
            Formula.Text = CalcFormula.Formula;
            this.Closed += (sender, e) => { CalcFormula.SetValue(C.CalculationFormula.Formula, Formula.Text); };
        }
    }
}
