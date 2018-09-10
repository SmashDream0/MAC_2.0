using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC_2.Messages
{
    public class QuestionMessanger
           : IMessenger
    {
        public bool ShowMessage(string text)
        {
            return  System.Windows.MessageBox.Show(text, "Вопрос", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes;
        }
    }
}
