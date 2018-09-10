using AutoTable;
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

namespace MAC_2.Calc
{
    /// <summary>
    /// Логика взаимодействия для GeneratorCalc_Window.xaml
    /// </summary>
    public partial class GeneratorCalc_Window : Window
    {
        public GeneratorCalc_Window()
        {
            InitializeComponent();
            this.GetSetting();
            GG_C = new GeneratorCalc_Class(Pollution_TC, GeneratorCalc_Class.EViewType.Pollution);
        }
        GeneratorCalc_Class GG_C;

        private void Pollusion_Click(object sender, RoutedEventArgs e)
        { GG_C = new GeneratorCalc_Class(Pollution_TC, GeneratorCalc_Class.EViewType.Pollution); }

        private void Group_Click(object sender, RoutedEventArgs e)
        { GG_C = new GeneratorCalc_Class(Pollution_TC, GeneratorCalc_Class.EViewType.Group); }

        private void Coefficient_Click(object sender, RoutedEventArgs e)
        { GG_C.LoadCoefficient();    }
    }
}