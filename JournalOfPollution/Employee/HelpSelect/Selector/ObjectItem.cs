using MAC_2.Model;
using System.Collections.Generic;

namespace MAC_2.Employee.HelpSelect.Selector
{
    public class ObjectItem : SearchItem
    {
        public ObjectItem(Objecte obj)
        {
            this.Objecte = obj;
            result = new Dictionary<string, string>();
            result.Add("Наименование", Name);
            result.Add("Адрес", Adres);
        }
    }
}
