using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    public partial class Mail_Window : Window
    {
        uint ID;
        public Mail_Window(uint ID)
        {
            InitializeComponent();
            this.ID = ID;

            Descryption_label.Text = "1)Нажмите \"Изменить пароль\", если хотите изменить пароль от вашей учетной записи\n\n2)Нажмите \"Сбросить учетную запись\", чтобы зайти из под вашей учетной записи, после сбоя питания и/или сбоя работы серверной части.";
        }

        bool Checks()
        {
            if (G.User.Rows.Get<string>(ID, C.User.Mail).Length == 0)
            {
                MessageBox.Show(this, "Почтовый ящик не указан, отправлять некуда.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            int SMTPPort;
            if (!int.TryParse(data.PrgSettings.Values[(int)data.Strings.SMTPPort].String, out SMTPPort))
            {
                MessageBox.Show(this, "Указанный порт не соответствует требованиям. Отправка невозможна", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (data.PrgSettings.Values[(int)data.Strings.MailLogin].String.Length == 0)
            {
                MessageBox.Show(this, "Логин почты отправителя не указан. Отправка невозможна", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (data.PrgSettings.Values[(int)data.Strings.MailPass].String.Length == 0)
            {
                MessageBox.Show(this, "Пароль почты отправителя не указан. Отправка невозможна", "Внимание", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }

        private void Send_button_Click(object sender, RoutedEventArgs e)
        {
            if (SetMail("Изменение пароля"))
            {
                if ((bool)(new SetNewPassWord_Window(ID)).ShowDialog())
                {
                    MessageBox.Show(this, "Пароль успешно изменен.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                    MessageBox.Show(this, "Изменение пароля отменено.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Repair_button_Click(object sender, RoutedEventArgs e)
        {
            if (SetMail("Восстановление учетной записи"))
            {
                T.User.Rows.Set(ID, C.User.IsHere, false);
                MessageBox.Show(this, "Восстановление учетной записи прошло успешно.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        bool SetMail(string Message)
        {
            if (!Checks())
            { return false; }

            var Pass = new byte[6];
            var rnd = new Random();

            for (int i = 0; i < Pass.Length; i++)
            { Pass[i] = (byte)rnd.Next(48, 58); }

            string TimeLessPass = Encoding.Default.GetString(Pass);

            try
            {
                var Smtp = new SmtpClient(data.PrgSettings.Values[(int)data.Strings.SMTPAdress].String, int.Parse(data.PrgSettings.Values[(int)data.Strings.SMTPPort].String));   //для всех 578, для гугла 25 и местной, еще есть 465

                if (data.PrgSettings.Values[(int)data.Strings.SMTPUseSSL].String.ToLower() == "true")
                { Smtp.EnableSsl = true; }
                else
                { Smtp.EnableSsl = false; }

                Smtp.Timeout = 5000;
                Smtp.Credentials = new NetworkCredential(data.PrgSettings.Values[(int)data.Strings.MailLogin].String
                                                         , data.PrgSettings.Values[(int)data.Strings.MailPass].String);

                MailAddress From = new MailAddress(data.PrgSettings.Values[(int)data.Strings.MailLogin].String, "Автоматическое сообщение"),
                            To = new MailAddress(G.User.Rows.Get<string>(ID, C.User.Mail), "Программа");
                var NewMessage = new MailMessage(From, To);
                NewMessage.Subject = Message;
                NewMessage.Body = String.Concat("Здравствуйте ", G.User.Rows.Get<string>(ID, C.User.Login), " ваш временный пароль ", TimeLessPass);
                Smtp.Send(NewMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "При отправке сообщения возникла ошибка:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            MessageBox.Show(this, "Письмо успешно отправлено, проверьте вашу почту.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);

            return (bool)new TimeLessPass_Window(TimeLessPass).ShowDialog();
        }
    }
}