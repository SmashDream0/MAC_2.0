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
    /// Логика взаимодействия для Calc_Window.xaml
    /// </summary>
    public partial class Calc_Window : Window
    {
        public Calc_Window(bool Scan)
        {
            InitializeComponent();
            if (Scan)
            { TextError.Text += " выявленных в ходе проверки"; }
        }
    }
}
