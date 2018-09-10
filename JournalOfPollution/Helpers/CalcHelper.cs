using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formulator;

namespace MAC_2.Helpers
{
    public static class CalcHelper
    {
        public static Formula GetFormula(string text)
        {
            //text = text.ToLower();

            Formula formula;

            //if (_formulsDictionary.ContainsKey(text))
            //{
            //    formula = _formulsDictionary[text].Clone();
            //}
            //else
            //{
                formula = new Formula();
                formula.FormulaText = text;

                //_formulsDictionary.Add(text, formula);
            //}

            return formula;
        }

        //private static Dictionary<string, Formula> _formulsDictionary = new Dictionary<string, Formula>();
    }
}
