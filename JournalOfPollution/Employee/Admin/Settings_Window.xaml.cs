using System;
using System.Drawing;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace MAC_2.EmployeeWindow.Admin
{
    /// <summary>
    /// Логика взаимодействия для Settings_Form.xaml
    /// </summary>
    public partial class Settings_Form : Window
    {
        /*bool ShowAgain = true;
        bool CanDo = true;*/

        //bool ItWasSql;

        public System.Drawing.Point Location { get; private set; }

        public Settings_Form()
        {
            InitializeComponent();

            var nms = Enum.GetNames(typeof(DataBase.RemoteType));

            for (int i = 0; i < nms.Length; i++)
            { DataSource_combo.Items.Add(nms[i]); }
        }

        private void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValue(SqlPass_label, SqlPass_Box) ||
                !CheckValue(SqlIp1_label, SqlIp1_Box) ||
                !CheckValue(SqlIp2_label, SqlIp2_Box) ||
                !CheckValue(SqlIpLast_label, SqlIpLast_Box) ||
                !CheckValue(SqlPort_label, SqlPort_Box) ||
                !CheckValue(SqlBdName_label, SqlBdName_Box) ||
                !CheckValue(eMaleAdress_label, eMaleAdress_Box) ||
                !CheckValue(eMaleLogin_label, eMaleLogin_Box) ||
                !CheckValue(eMalePass_label, eMalePass_Box) ||
                !CheckValue(Imap_Adress_label, ImapAdress_Box) ||
                !CheckValue(SmtpAdress_label, SmtpAdress_Box) ||
                !CheckIP(SqlIp1_label, SqlIp1_Box.Text) ||
                !CheckIP(SqlIp2_label, SqlIp2_Box.Text) ||
                SqlIp_label.Text.Length > 0 && !CheckIP(label4, SqlIp_label.Text)) return;

            if (DataSource_combo.SelectedIndex != data.PrgSettings.Values[(int)data.Strings.UseSQL].Int)
            { MessageBox.Show("Некоторые настройки вступят в силу, после перезапуска программы"); }

            data.PrgSettings.Values[(int)data.Strings.SqlIp1].String = SqlIp1_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SqlIp2].String = SqlIp2_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SqlIpLast].String = SqlIpLast_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SqlIp].String = SqlIp_label.Text;
            data.PrgSettings.Values[(int)data.Strings.SqlLogin].String = SqlLogin_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SqlPort].String = SqlPort_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SqlPassword].String = SqlPass_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.DATABASE].String = SqlBdName_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.MailIP].String = eMaleAdress_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.MailLogin].String = eMaleLogin_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.MailPass].String = eMalePass_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SMTPAdress].String = SmtpAdress_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.IMAPAdress].String = ImapAdress_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SMTPPort].String = eMaleSmtpPort_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.IMAPPort].String = eMaleImapPort_Box.Text;
            data.PrgSettings.Values[(int)data.Strings.SMTPUseSSL].Bool = (bool)eMaleUseSmtpSSL_Check.IsChecked;
            data.PrgSettings.Values[(int)data.Strings.UseSQL].Int = DataSource_combo.SelectedIndex;

            data.SetSettings(data.Strings.UseErorLog, cbUseErrorLog.IsChecked.Value);
            data.SetSettings(data.Strings.UseSimpleLog, cbUseSimpleLog.IsChecked.Value);
            data.SetSettings(data.Strings.UseActionLog, cbUseActionLog.IsChecked.Value);

            this.Close();
        }

        static bool CheckValue(TextBlock label, TextBox textbox)
        {
            if (textbox.Text.Length == 0)
            {
                MessageBox.Show($"Значение в поле \"{label.Text}\" не может быть пустым");
                textbox.Focus();
                return false;
            }
            return true;
        }

        static bool CheckIP(TextBlock label, string textbox)
        {
            if (textbox == "localhost")
                return true;

            IPAddress ip;
            if (!IPAddress.TryParse(textbox, out ip))
            {
                MessageBox.Show(label.Parent.ToString()
                    , $"Значение в поле \"{label.Text}\" не является ip-адресом."
                    , MessageBoxButton.OK
                    , MessageBoxImage.Exclamation
                    , MessageBoxResult.OK);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ExitBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Settings_Window_Load(object sender, RoutedEventArgs e)
        {
            SqlIp_label.Text = data.PrgSettings.Values[(int)data.Strings.SqlIp].String;

            SqlLogin_Box.Text = data.PrgSettings.Values[(int)data.Strings.SqlLogin].String;
            SqlPort_Box.Text = data.PrgSettings.Values[(int)data.Strings.SqlPort].String;
            SqlPass_Box.Text = data.PrgSettings.Values[(int)data.Strings.SqlPassword].String;
            SqlBdName_Box.Text = data.PrgSettings.Values[(int)data.Strings.DATABASE].String;
            eMaleAdress_Box.Text = data.PrgSettings.Values[(int)data.Strings.MailIP].String;
            eMaleLogin_Box.Text = data.PrgSettings.Values[(int)data.Strings.MailLogin].String;
            eMalePass_Box.Text = data.PrgSettings.Values[(int)data.Strings.MailPass].String;
            SmtpAdress_Box.Text = data.PrgSettings.Values[(int)data.Strings.SMTPAdress].String;
            ImapAdress_Box.Text = data.PrgSettings.Values[(int)data.Strings.IMAPAdress].String;
            eMaleSmtpPort_Box.Text = data.PrgSettings.Values[(int)data.Strings.SMTPPort].String;
            eMaleImapPort_Box.Text = data.PrgSettings.Values[(int)data.Strings.IMAPPort].String;
            eMaleUseSmtpSSL_Check.IsChecked = Convert.ToBoolean(data.PrgSettings.Values[(int)data.Strings.SMTPUseSSL].Bool);
            SqlIp1_Box.Text = data.PrgSettings.Values[(int)data.Strings.SqlIp1].String;
            SqlIp2_Box.Text = data.PrgSettings.Values[(int)data.Strings.SqlIp2].String;
            SqlIpLast_Box.Text = data.PrgSettings.Values[(int)data.Strings.SqlIpLast].String;
            DataSource_combo.SelectedIndex = data.PrgSettings.Values[(int)data.Strings.UseSQL].Int;

            cbUseErrorLog.IsChecked = data.GetBooleanSettings(data.Strings.UseErorLog);
            cbUseSimpleLog.IsChecked = data.GetBooleanSettings(data.Strings.UseSimpleLog);
            cbUseActionLog.IsChecked = data.GetBooleanSettings(data.Strings.UseActionLog);

            this.Location = data.PrgSettings.Forms[(int)data.Forms.Settings].Location;
        }

        private void Settings_Form_FormClosed(object sender, EventArgs e)
        {
            data.PrgSettings.Forms[(int)data.Forms.Settings].Set(this);

            data.PrgSettings.Save();
        }

        private void EmalePort_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataBase.NoABC_Int_Dinamic(sender as System.Windows.Forms.Control);
        }

        private void UseIp1_button_Click(object sender, RoutedEventArgs e)
        {
            SqlIp_label.Text = data.PrgSettings.Values[(int)data.Strings.SqlIp1].String;
        }

        private void UseIp2_button_Click(object sender, RoutedEventArgs e)
        {
            SqlIp_label.Text = data.PrgSettings.Values[(int)data.Strings.SqlIp2].String;
        }

        private void UseIp3_button_Click(object sender, RoutedEventArgs e)
        {
            SqlIp_label.Text = data.PrgSettings.Values[(int)data.Strings.SqlIpLast].String;
        }

        private void SqlPort_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataBase.NoABC_Int_Dinamic(SqlIpLast_Box);
        }

        private void eMaleImapPort_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataBase.NoABC_Int_Dinamic(eMaleSmtpPort_Box);
        }
    }
}
