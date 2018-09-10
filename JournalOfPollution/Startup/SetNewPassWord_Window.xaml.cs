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
    public partial class SetNewPassWord_Window : Window
    {
        uint ID;
        System.Windows.Forms.DialogResult DialogResult;
        public SetNewPassWord_Window(uint ID)
        {
            InitializeComponent();
            this.ID = ID;
        }

        private void Continue_button_Click(object sender, RoutedEventArgs e)
        {
            if (NewPassWord_Box.Text.Length == 0)
            {
                MessageBox.Show(this
                    , "Необходимо ввести пароль!"
                    , "Внимание"
                    , MessageBoxButton.OK
                    , MessageBoxImage.Information);
                NewPassWord_Box.Focus();
                return;
            }

            if (NewPassWord_Box.Text != RepeateNewPassWord_Box.Text)
            {
                MessageBox.Show(this
                    , "Пароли не совпадают!", "Внимание"
                    , MessageBoxButton.OK
                    , MessageBoxImage.Information);
                RepeateNewPassWord_Box.Focus();
                return;
            }

            T.User.Rows.Set(ID, C.User.Pass, NewPassWord_Box.Text);
            DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this
                , "Пароль не будет изменен. Вы уверены, что хотите оставить все как есть ?"
                , "Внимание"
                , MessageBoxButton.OK
                , MessageBoxImage.Information) == MessageBoxResult.No)
                return;

            DialogResult = System.Windows.Forms.DialogResult.No;
        }
    }
}
