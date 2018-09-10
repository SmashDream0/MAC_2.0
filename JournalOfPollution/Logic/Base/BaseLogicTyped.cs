using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Logic
{
    public abstract class BaseLogicTyped<ModelT>
        : BaseLogic
        where ModelT : MyTools.C_A_BaseFromAllDB
    {
        public BaseLogicTyped(DataBase.ITable table)
            : base(new Cache<ModelT>(), table)
        {
            this.Table.Rows.AfterAddRow += (t, id) => getModel(id);
            this._table = Table.CreateSubTable(false);
        }

        protected DataBase.ISTable _table { get; private set; }

        public new Cache<ModelT> Cache { get { return (Cache<ModelT>)base.Cache; } }

        private ModelT getModel(uint id)
        {
            var model = Cache.Get(id);

            if (model == null)
            {
                model = internalGetModel(id);

                Cache.Add(model);
            }

            return model;
        }

        protected virtual ModelT internalGetModel(uint id)
        { return (ModelT)Activator.CreateInstance(typeof(ModelT), id); }

        protected IEnumerable<ModelT> getModels(DataBase.ISTable table)
        { return getModels(((DataBase.table.SubTable)table).ShowRow); }

        protected IEnumerable<ModelT> getModels(IEnumerable<DataBase.IRecord> records)
        {
            var list = new List<ModelT>(records.Count());

            foreach (var record in records)
            {
                var model = getModel(record.ID);

                list.Add(model);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Поулчить результат исполнения запроса
        /// </summary>
        /// <param name="query"></param>
        /// <param name="querryAction"></param>
        /// <returns></returns>
        protected IEnumerable<ModelT> getQuerryResult(string query, Action<DataBase.ISTable> querryAction, Action<IEnumerable<ModelT>> loadAction = null)
        {
            return getQuerryResult(query, (table) =>
            {
                querryAction(table);

                return getModels(table);
            }, loadAction);
        }

        /// <summary>
        /// Поулчить результат исполнения запроса
        /// </summary>
        /// <param name="query"></param>
        /// <param name="querryAction"></param>
        /// <returns></returns>
        protected IEnumerable<ModelT> getQuerryResult(string query, Action<DataBase.ISTable> querryAction, out bool cacheUsed, Action<IEnumerable<ModelT>> loadAction = null)
        {
            return getQuerryResult(query, (table) =>
            {
                querryAction(table);

                return getModels(table);
            }, out cacheUsed, loadAction);
        }

        /// <summary>
        /// Поулчить результат исполнения запроса
        /// </summary>
        /// <param name="query"></param>
        /// <param name="querryAction"></param>
        /// <returns></returns>
        protected IEnumerable<ModelT> getQuerryResult(string query, Func<DataBase.ISTable, IEnumerable<ModelT>> querryAction, Action<IEnumerable<ModelT>> loadAction = null)
        {
            bool cacheUsed;
            var result = getQuerryResult(query, querryAction, out cacheUsed, loadAction);

            return result;
        }

        /// <summary>
        /// Поулчить результат исполнения запроса
        /// </summary>
        /// <param name="query"></param>
        /// <param name="querryAction"></param>
        /// <returns></returns>
        protected IEnumerable<ModelT> getQuerryResult(string query, Func<DataBase.ISTable, IEnumerable<ModelT>> querryAction, out bool cacheUsed, Action<IEnumerable<ModelT>> loadAction = null)
        {
            var result = Cache.Get(query);

            if (result != null)
            {
                cacheUsed = true;
                return result;
            }

            result = querryAction(this._table);

            Cache.Add(query, result);

            if (loadAction != null)
            { loadAction(result); }

            cacheUsed = false;
            return result;
        }

        protected void addQuerry(string query, IEnumerable<ModelT> models)
        { Cache.Add(query, models); }

        public virtual IEnumerable<ModelT> Find()
        {
            return getQuerryResult($"all", (table) =>
            {
                table.QUERRY().SHOW.DO();
            });
        }

        public virtual ModelT FirstOrDefault(uint id)
        {
            bool cacheUsed;
            var result = getQuerryResult($"id={id}", (table) =>
            {
                table.QUERRY().SHOW.WHERE.ID(id).DO();
            }, out cacheUsed).FirstOrDefault();

            if (!cacheUsed)
            { this.Cache.ClearQuerry(); }

            return result;
        }

        /// <summary>
        /// Лучше пользоваться как можно реже
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ModelT FirstModel(uint id)
        {
            var result = Cache.Get(id, false);

            if (result == null)
            { result = FirstOrDefault(id); }

            return result;
        }

        public Dictionary<uint, ModelT> GetDictionary(IEnumerable<ModelT> models)
        {
            Dictionary<uint, ModelT> dictionary = new Dictionary<uint, ModelT>();

            foreach (var model in models)
            { dictionary.Add(model.ID, model); }

            return dictionary;
        }

        protected static DataBase.IOrAndDo makeRangePeriod(DataBase.IOAOperand query, int ym, int ymFrom, int[] ymFromArray, int ymTo, int[] ymToArray)
        {
            return query
                .OB()
                    .ARC(ymFrom, ymFromArray).Less.BV(ym + 1)
                .AND
                    .OB()
                        .ARC(ymTo, ymToArray).More.BV(ym - 1)
                    .OR
                        .ARC(ymTo, ymToArray).EQUI.BV(0)
                    .CB()
                .CB();
        }
        protected static DataBase.IOrAndDo makeRangePeriod(DataBase.IOrAnd query, int ym, int ymFrom, int[] ymFromArray, int ymTo, int[] ymToArray)
        {
            return makeRangePeriod(query.AND, ym, ymFrom, ymFromArray, ymTo, ymToArray);
        }

        protected static DataBase.IOrAndDo makeRangePeriod(DataBase.IOAOperand query, int ym, int ymFrom, int ymTo)
        {
            return query
                .OB()
                    .AC(ymFrom).Less.BV(ym + 1)
                .AND
                    .OB()
                        .AC(ymTo).More.BV(ym - 1)
                    .OR
                        .AC(ymTo).EQUI.BV(0)
                    .CB()
                .CB();
        }
        protected static DataBase.IOrAndDo makeRangePeriod(DataBase.IOrAnd query, int ym, int ymFrom, int ymTo)
        {
            return makeRangePeriod(query.AND, ym, ymFrom, ymTo);
        }

        protected static DataBase.IOrAndDo makeRangePeriodTo(DataBase.IOAOperand query, int ym, int ymTo)
        {
            return query
                    .AC(ymTo).More.BV(ym + 1)
                .OR
                    .AC(ymTo).EQUI.BV(0);
        }
        protected static DataBase.IOrAndDo makeRangePeriodTo(DataBase.IOrAnd query, int ym, int ymTo)
        {
            return makeRangePeriodTo(query.AND, ym, ymTo);
        }

        protected static DataBase.IOrAndDo makeRangePeriodFrom(DataBase.IOrAnd query, int ym, int ymFrom)
        {
            return makeRangePeriodTo(query.AND, ym, ymFrom);
        }
        protected static DataBase.IOrAndDo makeRangePeriodFrom(DataBase.IOAOperand query, int ym, int ymFrom)
        {
            return query
                    .AC(ymFrom).Less.BV(ym + 1)
                .OR
                    .AC(ymFrom).EQUI.BV(0);
        }
    }
}