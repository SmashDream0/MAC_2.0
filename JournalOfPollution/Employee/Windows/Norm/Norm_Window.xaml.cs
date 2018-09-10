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

namespace MAC_2.Employee.Windows
{
    /// <summary>
    /// Логика взаимодействия для Norm_Window.xaml
    /// </summary>
    public partial class Norm_Window : Window
    {
        public Norm_Window()
        {
            InitializeComponent();
            this.GetSetting();
            N_C = new NormViewModel(Norms);
        }
        NormViewModel N_C;
    }
}