using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.Mechanisms.LoadVolume
{
    internal struct Columns
    {
        public const string name = "наименование предприятия";
        public const string inn = "инн";
        public const string volume = "объем, м3";
        public const string adres = "место отбора";
        public const string acount = "л/с";

        public const string volold = "объём по старому тарифу, м3";
        public const string volnew = "объём по новому тарифу, м3";
    }
}
