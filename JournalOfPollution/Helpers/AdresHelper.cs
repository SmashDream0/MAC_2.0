using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Helpers
{
    public static class AdresHelper
    {

        public static string CutAdr(string adr)
        {
            return adr.CutAdres(false)
                .ToLower()
                .Replace("дом", string.Empty)
                .Replace("ул.", string.Empty)
                .Replace("№", string.Empty)
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);
        }


        public static bool ComparisonAdres(string Adres1, string Adres2)
        {
            Adres1 = CutAdr(Adres1);
            Adres2 = CutAdr(Adres2);

            if (Adres1.Contains(Adres2) || Adres2.Contains(Adres1))
            { return true; }

            return false;
        }
    }
}
