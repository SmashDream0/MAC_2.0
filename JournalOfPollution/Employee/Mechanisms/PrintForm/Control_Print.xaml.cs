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

namespace MAC_2.PrintForm
{
    /// <summary>
    /// Логика взаимодействия для ControlPrint.xaml
    /// </summary>
    public partial class Control_Print : Window
    {
        public Control_Print()
        {
            InitializeComponent();
        }

        private void ControlPrint_Loaded(object sender, RoutedEventArgs e)
        {
            ControlPrint.Width = Elems.DesiredSize.Width + 50;
            ControlPrint.Height = Elems.DesiredSize.Height + 65;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}
