using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Logic
{
    public abstract class BaseLogic<ModelT>
        where ModelT : MyTools.C_A_BaseFromAllDB
    {
        protected abstract DataBase.ISTable table { get; }
        private Cache<ModelT> _cache = new Cache<ModelT>();

        protected abstract ModelT getModel(uint id);

        protected IEnumerable<ModelT> getModels()
        { return getModels(((DataBase.table.SubTable)table).ShowRow); }

        protected IEnumerable<ModelT> getModels(IEnumerable<DataBase.IRecord> records)
        {
            var list = new List<ModelT>(records.Count());

            foreach (var record in records)
            {
                var model = _cache.Get(record.ID);

                if (model == null)
                { model = getModel(record.ID); }

                list.Add(model);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Поулчить результат исполнения запроса
        /// </summary>
        /// <param name="querry"></param>
        /// <param name="querryAction"></param>
        /// <returns></returns>
        protected IEnumerable<ModelT> getQuerryResult(string querry, Action<DataBase.ISTable> querryAction, Action<IEnumerable<ModelT>> loadAction = null)
        {
            var result = _cache.Get(querry);

            if (result != null)
            { return result; }

            querryAction(table);

            result = getModels();

            if (loadAction != null)
            { loadAction(result); }

            _cache.Add(querry, result);

            return result;
        }

        protected void addQuerry(string querry, IEnumerable<ModelT> models)
        { _cache.Add(querry, models); }

        /// <summary>
        /// Очистить кеш
        /// </summary>
        public void ClearCache()
        { _cache.Clear(); }
        
        public IEnumerable<ModelT> Find()
        {
            return getQuerryResult($"all", (table) =>
            {
                table.QUERRY().SHOW.DO();
            });
        }

        public Dictionary<uint, ModelT> GetDictionary(IEnumerable<ModelT> models)
        {
            Dictionary<uint, ModelT> dictionary = new Dictionary<uint, ModelT>();

            foreach (var model in models)
            { dictionary.Add(model.ID, model); }

            return dictionary;
        }
    }
}