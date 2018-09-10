using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2
{
    public class Cache<ModelT>: BaseCache
        where ModelT: MyTools.C_A_BaseFromAllDB
    {
        private static Dictionary<uint, KeyValuePair<DateTime, ModelT>> _dictionaryModel = new Dictionary<uint, KeyValuePair<DateTime, ModelT>>();
        private static Dictionary<string, KeyValuePair<DateTime, IEnumerable<ModelT>>> _dictionaryQuerry = new Dictionary<string, KeyValuePair<DateTime, IEnumerable<ModelT>>>();

        /// <summary>
        /// Время жизни кеша в милисекундах
        /// </summary>
        public static int CacheVileMiliseconds = 30 * 10000 * 1000;

        public void Add(ModelT model)
        {
            if (!_dictionaryModel.ContainsKey(model.ID))
            { _dictionaryModel.Add(model.ID, new KeyValuePair<DateTime, ModelT>(DateTime.Now, model)); }
        }

        public ModelT Get(uint id, bool useTiming = true)
        {
            if (_dictionaryModel.ContainsKey(id))
            {
                var result = _dictionaryModel[id];

                if (!useTiming || CheckCache(result.Key))
                { return result.Value; }
                else
                { _dictionaryModel.Remove(id); }
            }

            return null;
        }

        public void Add(string query, IEnumerable<ModelT> models)
        {
            if (_dictionaryQuerry.ContainsKey(query))
            {
                var result = _dictionaryQuerry[query];

                if (!CheckCache(result.Key))
                {
                    _dictionaryQuerry[query] = new KeyValuePair<DateTime, IEnumerable<ModelT>>(DateTime.Now, models);
                }
            }
            else
            {
                _dictionaryQuerry.Add(query, new KeyValuePair<DateTime, IEnumerable<ModelT>>(DateTime.Now, models));
            }
        }

        public IEnumerable<ModelT> Get(string query)
        {
            if (_dictionaryQuerry.ContainsKey(query))
            {
                var result = _dictionaryQuerry[query];

                if (CheckCache(result.Key))
                { return result.Value; }
                else
                { _dictionaryQuerry.Remove(query); }
            }

            return null;
        }

        private static bool CheckCache(DateTime timeStamp)
        {
            var ts = new TimeSpan(CacheVileMiliseconds);

            return DateTime.Now.Ticks - timeStamp.Ticks <= CacheVileMiliseconds;
        }

        public override void ClearModel()
        {
            _dictionaryModel.Clear();
        }
        public override void ClearQuerry()
        {
            _dictionaryQuerry.Clear();
        }
    }
}
