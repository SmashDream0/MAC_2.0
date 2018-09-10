using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MAC_2.EmployeeWindow.Admin
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class About_Window : Window
    {
        public About_Window()
        {
            InitializeComponent();
        }

        private void Descryption_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void AboutForm_Load(object sender, RoutedEventArgs e)
        {            
            Descryption.Text = "Средство учета размещенных материалов.\r\n" +
                               "Версия " + Assembly.GetExecutingAssembly().GetName().Version +
                               ", МУП г.Астрахани \"Астрводоканал\", Отдел ИТиТ, Круглов Павел\r\n" +
                               ((DataBase.RemoteType)data.PrgSettings.Values[(int)data.Strings.UseSQL].Int).ToString() + " - " + data.PrgSettings.Values[(int)data.Strings.DATABASE].String;

            switch ((DataBase.RemoteType)data.PrgSettings.Values[(int)data.Strings.UseSQL].Int)
            {
                case DataBase.RemoteType.MySQL:
                    Descryption.Text += "\r\nip: " + data.PrgSettings.Values[(int)data.Strings.SqlIp].String + ";  login: " + data.PrgSettings.Values[(int)data.Strings.SqlLogin].String;
                    break;
            }
        }

        private void label3_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://dev.mysql.com/downloads/connector/net/");
        }

        private void label2_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://imapx.codeplex.com/");
        }

        private void label1_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://npoi.codeplex.com/");
        }

        private void Descryptions_button_Click(object sender, RoutedEventArgs e)
        {
            var FilePath = Environment.CurrentDirectory + "\\Инструкции\\";
            {
                string[] Files;
                if (Directory.Exists(FilePath) && (Files = Directory.GetFiles(FilePath)).Length > 0)
                {
                    if (Files.Length == 1)
                    { System.Diagnostics.Process.Start(Files[0]); }
                    else
                    { System.Diagnostics.Process.Start(FilePath); }
                }
                else
                {
                    MessageBox.Show(this
                        , "Инструкции не найдены"
                        , "Внимание"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Information);
                }
            }
        }

        private void DocChanges_button_Click(object sender, RoutedEventArgs e)
        {
            Startup_Window.CheckDocChanges();
        }

        private void Lable1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Lable1.Foreground = Brushes.Green;
        }

        private void Lable1_MouseLeave(object sender, MouseEventArgs e)
        {
            Lable1.Foreground = Brushes.Blue;
        }

        private void Lable2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Lable1.Foreground = Brushes.Green;
        }

        private void Lable2_MouseLeave(object sender, MouseEventArgs e)
        {
            Lable1.Foreground = Brushes.Blue;
        }

        private void Lable3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Lable1.Foreground = Brushes.Green;
        }

        private void Lable3_MouseLeave(object sender, MouseEventArgs e)
        {
            Lable1.Foreground = Brushes.Blue;
        }

    }
}
