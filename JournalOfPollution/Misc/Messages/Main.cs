using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAC_2.Messages;
using MAC_2;

namespace MAC_2.Messages
{

    public interface IMessenger
    {
        bool ShowMessage(string text);
    }

    public static class StaticMessager
    {
        static StaticMessager()
        {
            ErrorMessage = new ErrorMessanger();
            InformationMessage = new InformationMessanger();
            QuestionMessage = new QuestionMessanger();
        }

        public static void Error(Exception ex)
        {
            Program.Loggers.Error.Add(ex);

            if (ErrorMessage != null)
            { ErrorMessage.ShowMessage(ex.Message + "\r\n" + ex.ToString()); }
        }

        public static void Error(string txt)
        {
            Program.Loggers.Log.Add(() => txt);

            if (ErrorMessage != null)
            { ErrorMessage.ShowMessage(txt); }
        }

        public static bool Question(string txt)
        {
            Program.Loggers.Log.Add(() => "user question: " + txt);

            if (QuestionMessage != null)
            { return QuestionMessage.ShowMessage(txt); }

            return false;
        }

        public static bool Information(string txt)
        {
            Program.Loggers.Log.Add(() => "user information: " + txt);

            if (QuestionMessage != null)
            { return InformationMessage.ShowMessage(txt); }

            return false;
        }

        public static IMessenger ErrorMessage { get; set; }
        public static IMessenger QuestionMessage { get; set; }
        public static IMessenger InformationMessage { get; set; }
    }
}