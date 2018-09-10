using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAC_2.Messages
{
    public class ErrorMessanger
        : IMessenger
    {
        public bool ShowMessage(string text)
        {
            System.Windows.MessageBox.Show(text, "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            return false;
        }
    }
}
