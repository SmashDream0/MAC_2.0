using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model.NavigationProperty
{
    public abstract class BaseNavigationProperty<ModelT>
    {
        /// <summary>
        /// Событие добавления модели
        /// </summary>
        public event Action<ModelT> OnAdd;

        /// <summary>
        /// Добавить модель
        /// </summary>
        /// <param name="model">Модель</param>
        /// <returns>Была ли доабвлена модель</returns>
        public abstract bool Add(ModelT model);

        /// <summary>
        /// Очистить
        /// </summary>
        public abstract void Clear();

        protected void OnAddAction(ModelT model)
        {
            if (OnAdd != null)
            { OnAdd(model); }
        }
    }
}
