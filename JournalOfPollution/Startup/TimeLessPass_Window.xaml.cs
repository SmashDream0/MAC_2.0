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

namespace MAC_2
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class TimeLessPass_Window : Window
    {
        string Pass;
        byte Nums = 0;
        public TimeLessPass_Window(string Pass)
        {
            InitializeComponent();
            this.Pass = Pass;
        }
        System.Windows.Forms.DialogResult DialogResult;
        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void Continue_button_Click(object sender, RoutedEventArgs e)
        {
            if (Pass == Pass_Box.Text)
            {
                DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
            else
            {
                if (Nums > 1)
                {
                    MessageBox.Show(this
                        , "Пароль не верный, попыток неосталось"
                        , "Внимание"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Exclamation);
                    DialogResult = System.Windows.Forms.DialogResult.No;
                }
                else
                {
                    MessageBox.Show(this
                        , "Пароль не верный, осталось попыток: " + (3 - ++Nums)
                        , "Внимание"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Exclamation);
                }
            }
        }
    }
}
