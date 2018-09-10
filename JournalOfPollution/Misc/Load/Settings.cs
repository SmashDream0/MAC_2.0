using System;
using System.Windows.Forms;

namespace MAC_2
{
    public static partial class Misc
    {
        /// <summary>Загрузка файла настроек</summary>
        public static void Prepare(string FilePath = null)
        {
            //MessageBox.Show("Начинаю грузить настройки");
            var Values = new int[Enum.GetNames(typeof(data.Strings)).Length];

            Values[(int)data.Strings.LastUser] =
            Values[(int)data.Strings.SqlPort] =
            Values[(int)data.Strings.UseSQL] =
            Values[(int)data.Strings.SMTPPort] =
            Values[(int)data.Strings.IMAPPort] =
            Values[(int)data.Strings.Changes] = 5;
            Values[(int)data.Strings.SMTPPort] = 5;

            Values[(int)data.Strings.SMTPAdress] =
            Values[(int)data.Strings.IMAPAdress] =
            Values[(int)data.Strings.MailIP] =
            Values[(int)data.Strings.MailLogin] =
            Values[(int)data.Strings.MailPass] = 50;

            Values[(int)data.Strings.DATABASE] =
            Values[(int)data.Strings.SqlPassword] =
            Values[(int)data.Strings.SqlLogin] = 30;

            Values[(int)data.Strings.SqlIp] =
            Values[(int)data.Strings.SqlIp1] =
            Values[(int)data.Strings.SqlIp2] =
            Values[(int)data.Strings.SqlIpLast] = 15;

            Values[(int)data.Strings.UseErorLog] =
            Values[(int)data.Strings.UseActionLog] =
            Values[(int)data.Strings.UseSimpleLog] = 5;

            data.PrgSettings = new Settings((FilePath == null ? Application.StartupPath + "\\App\\" : FilePath), "Settings.dll", Enum.GetNames(typeof(data.Forms)).Length, Values);

            //data.PrgSettings.Load();
        }
    }
}
