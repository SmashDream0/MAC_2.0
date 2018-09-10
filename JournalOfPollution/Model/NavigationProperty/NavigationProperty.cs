using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model.NavigationProperty
{
    public class NavigationProperty<ModelT, CompareT>
        : BaseNavigationProperty<ModelT>
        where ModelT : MyTools.C_A_BaseFromAllDB
    {
        public NavigationProperty(CompareT compareValue, Func<ModelT, CompareT> getCompareValue)
        {
            _compareValue = compareValue;
            _getCompareValue = getCompareValue;
        }

        public ModelT Model
        { get; private set; }

        private readonly CompareT _compareValue;

        private readonly Func<ModelT, CompareT> _getCompareValue;

        private static Comparer<CompareT> _comparer = Comparer<CompareT>.Default;

        /// <summary>
        /// Присвоить модель
        /// </summary>
        /// <param name="model">Модель</param>
        /// <returns></returns>
        public override bool Add(ModelT model)
        {
            var compareValue = _getCompareValue(model);

            if (_comparer.Compare(compareValue, _compareValue) == 0)
            {
                Model = model;

                return true;
            }
            else
            { return false; }
        }

        /// <summary>
        /// Очистить модель
        /// </summary>
        public override void Clear()
        {
            Model = null;
        }
    }
}
