using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model.NavigationProperty
{
    public class ListNavigationProperty<ModelT, CompareT>
        : BaseNavigationProperty<ModelT>, IEnumerable<ModelT>
        where ModelT : MyTools.C_A_BaseFromAllDB
    {
        public ListNavigationProperty(CompareT target, 
            Func<ModelT, CompareT> getCheckValue)
        {
            this._target = target;
            this._getCheckValue = getCheckValue;
        }

        public class EArg
        {
            public bool CanUse { get; set; }
        }

        /// <summary>
        /// Событие проверки разрешения на добавление модели в коллекцию
        /// </summary>
        public event Action<ModelT, EArg> OnCanAdd;

        #region IEnumerator

        IEnumerator<ModelT> IEnumerable<ModelT>.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_dictionary.Values).GetEnumerator();
        }

        #endregion

        #region Поля

        private readonly CompareT _target;
        private readonly Func<ModelT, CompareT> _getCheckValue;
        private Func<CompareT, IEnumerable<ModelT>> _loadFunc = null;

        private Dictionary<uint, ModelT> _dictionary = new Dictionary<uint, ModelT>();
        private static Comparer<CompareT> _comparer = Comparer<CompareT>.Default;

        #endregion

        #region Методы

        /// <summary>
        /// Задать функцию перезагрузки элементов
        /// </summary>
        /// <param name="loadFunc"></param>
        public void SetReloadAction(Func<CompareT, IEnumerable<ModelT>> loadFunc)
        {
            this._loadFunc = loadFunc;
        }

        /// <summary>
        /// Перезагрузить элементы, если задана функция перезагрузки
        /// </summary>
        public void ReloadList()
        {
            if (this._loadFunc == null)
            { throw new Exception("Can't load cause no load function set"); }

            this.Clear();

            var items = this._loadFunc(_target);

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Добавить модель
        /// </summary>
        /// <param name="model">Модель</param>
        /// <returns>Была ли доабвлена модель</returns>
        public override bool Add(ModelT model)
        {
            if (OnCanAdd != null)
            {
                var arg = new EArg() { CanUse = true };

                OnCanAdd(model, arg);

                if (!arg.CanUse)
                { return false; }
            }

            var checkValue = _getCheckValue(model);
            
            if (_comparer.Compare(checkValue, _target) == 0)
            {
                if (_dictionary.ContainsKey(model.ID))
                { _dictionary[model.ID] = model; }
                else
                { _dictionary.Add(model.ID, model); }

                OnAddAction(model);

                return true;
            }
            else
            { return false; }
        }

        /// <summary>
        /// Очистить коллекцию моделей
        /// </summary>
        public override void Clear()
        { _dictionary.Clear(); }

        #endregion
    }
}
