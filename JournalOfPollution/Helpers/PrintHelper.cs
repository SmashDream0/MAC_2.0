using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Helpers
{
    public static class PrintHelper
    {
        public static string KillZero(this decimal Value)
        {
            int kill = 0;
            string result = Value.ToString();
            foreach (var one in result.Reverse())
            {
                if (one == ',')
                {
                    kill++;
                    break;
                }
                else if (one == '0')
                { kill++; }
                else
                { break; }
            }
            return result.Contains(',') ? result.Substring(0, result.Length - kill) : result;
        }

        /// <summary>Отрезает от адреса: индекс, область, город</summary>
        public static string CutAdres(this string adres, bool full)
        {
            if (adres == null || adres.Length == 0)
            { return ""; }

            if (full)
            { adres = adres.StringDivision(28); }
            else
            {
                if (adres[0] == '4')
                { adres = adres.Substring(adres.IndexOf(',') + 1); }

                if (adres.Substring(0, 14).Contains("Астраханская"))
                { adres = adres.Substring(adres.IndexOf(',') + 1); }

                if (adres.Contains("Астрахань"))
                { adres = adres.Substring(adres.IndexOf(',') + 1); }
            }
            return adres;
        }

        public static string CutAdress(string adres, bool full)
        {
            if (adres == null || adres.Length == 0)
            { return ""; }

            if (full)
            { adres = adres.StringDivision(28); }
            else
            {
                if (adres[0] == '4')
                { adres = adres.Substring(adres.IndexOf(',') + 1); }

                if (adres.Substring(0, 14).Contains("Астраханская"))
                { adres = adres.Substring(adres.IndexOf(',') + 1); }

                if (adres.Contains("Астрахань"))
                { adres = adres.Substring(adres.IndexOf(',') + 1); }
            }
            return adres;
        }
    }
}