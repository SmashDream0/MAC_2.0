using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MAC_2
{
    public class Program
    {
        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        /// <param name="args">Аргументы командной строки</param>
        [STAThread]
        public static void Main(string[] args)
        {
            initialize();
            setArgs(args);
            execFileCommands();

            try
            {
                new Startup_Window().ShowDialog();
            }
            catch (Exception ex)
            { MAC_2.Messages.StaticMessager.Error(ex); }
        }

        private static void initialize()
        {
            Misc.Prepare();

            Loggers = new Logger.Main(ProgramPath + "\\Log");

            Loggers.Action.Enable = data.GetBooleanSettings(data.Strings.UseActionLog);
            Loggers.Error.Enable = data.GetBooleanSettings(data.Strings.UseErorLog);
            Loggers.Log.Enable = data.GetBooleanSettings(data.Strings.UseSimpleLog);
        }

        private static void setArgs(string[] Args)
        {
            for (int i = 0; i < Args.Length; i++)
            {
                var cmd = Args[i].ToLower();

                if (cmd.Length > data.CMD.AlowToChange.Length && cmd.IndexOf(data.CMD.AlowToChange, 0, data.CMD.AlowToChange.Length) > -1)
                {
                    if (!bool.TryParse(cmd.Substring(data.CMD.AlowToChange.Length), out data.AllowModify))
                    { data.AllowModify = false; }
                }
                else if (cmd.Length > data.CMD.SettingsFile.Length && cmd.IndexOf(data.CMD.SettingsFile, 0, data.CMD.SettingsFile.Length) > -1)
                { data.StName = cmd.Substring(data.CMD.SettingsFile.Length); }
                else if (cmd.Length > data.CMD.SetIncrem.Length && cmd.IndexOf(data.CMD.SetIncrem, 0, data.CMD.SetIncrem.Length) == 0)
                {
                    if (!int.TryParse(cmd.Substring(data.CMD.SetIncrem.Length), out data.Increm))
                    { data.Increm = -1; }
                }
                else if (cmd.Length > data.CMD.DeleteMe.Length && cmd.IndexOf(data.CMD.DeleteMe, 0, data.CMD.DeleteMe.Length) > -1)
                {
                    if (!bool.TryParse(cmd.Substring(data.CMD.DeleteMe.Length), out data.DeleteConf))
                    { data.DeleteConf = false; }
                }
            }
        }

        private static void execFileCommands()
        {
            if (File.Exists(Application.StartupPath + "\\commands.txt"))
            {
                using (var sr = new StreamReader(Application.StartupPath + "\\commands.txt"))
                {
                    var FArgs = new List<string>();

                    while (!sr.EndOfStream)
                    { FArgs.Add(sr.ReadLine()); }

                    setArgs(FArgs.ToArray());
                }
            }

            if (data.DeleteConf && File.Exists(Application.StartupPath + "\\commands.txt"))
            { File.Delete(Application.StartupPath + "\\commands.txt"); }
        }

        /// <summary>
        /// Средства логгирования
        /// </summary>
        public static Logger.Main Loggers { get; private set; }

        /// <summary>
        /// Путь до папки с исполняемым файлом
        /// </summary>
        public static readonly string ProgramPath = Application.StartupPath;
    }
}
