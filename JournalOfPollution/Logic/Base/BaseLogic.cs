using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Logic
{
    public abstract class BaseLogic
    {
        public BaseLogic(BaseCache cache, DataBase.ITable table)
        {
            this.Table = table;
            this.Cache = cache;

            //addLogic(this);
        }

        public BaseCache Cache { get; private set; }
        public DataBase.ITable Table { get; private set; }

        private static Dictionary<DataBase.ITable, BaseLogic> _logics = new Dictionary<DataBase.ITable, BaseLogic>();

        private static void addLogic(BaseLogic logic)
        {
            if (!_logics.ContainsKey(logic.Table))
            { _logics.Add(logic.Table, logic); }
        }

        public static BaseLogic GetLogic(DataBase.ITable table)
        {
            if (_logics.ContainsKey(table))
            { return _logics[table]; }
            else
            { return null; }
        }
    }
}
