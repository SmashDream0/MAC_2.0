using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Logic.Base.querior
{
    class queryTable
    {
        public queryTable(DataBase.ISTable table)
        {
            this.Table = table;

            Operations = new List<KeyValuePair<EOperation, queryTableColumn>>();
        }

        public enum EOperation { OB, CB, AND, OR, Column };
        public enum ECondition { None, Equal, More, Less }

        public DataBase.ISTable Table { get; private set; }

        public List<KeyValuePair<EOperation, queryTableColumn>> Operations { get; private set; }

        public queryTable OB()
        {
            Operations.Add(new KeyValuePair<EOperation, queryTableColumn>(EOperation.OB, null));

            return this;
        }

        public queryTable CB()
        {
            Operations.Add(new KeyValuePair<EOperation, queryTableColumn>(EOperation.CB, null));

            return this;
        }

        public queryTable Column<T>(int columnIndex, int[] columnIndexes, bool not, ECondition condition, T value)
        {
            var column = new queryTableColumn(columnIndex, columnIndexes);
            column.SetCondition(not, condition, value);

            Operations.Add(new KeyValuePair<EOperation, queryTableColumn>(EOperation.Column, column));

            return this;
        }

        public queryTable Column<T>(int columnIndex, bool not, ECondition condition, T value)
        { return Column(columnIndex, new int[0], not, condition, value); }

        public queryTable AND()
        {
            Operations.Add(new KeyValuePair<EOperation, queryTableColumn>(EOperation.AND, null));
            return this;
        }

        public queryTable OR()
        {
            Operations.Add(new KeyValuePair<EOperation, queryTableColumn>(EOperation.OR, null));
            return this;
        }

        private object makeRelationQuery(object query)
        {
            for (int i = 0; i < Table.Parent.Columns.Count; i++)
            {
                var column = Table.Parent.GetColumn(i);

                if (column.TypeCol == DataBase.Types.RIU32)
                {
                    var logic = BaseLogic.GetLogic(column.RelatedTable);

                    if (logic != null)
                    {

                    }
                }
            }

            return query;
        }

        public void MakeQuery()
        {
            object query = Table.QUERRY().SHOW;

            query = makeRelationQuery(query);

            query = MakeQuery(query);

            ((DataBase.IDo)query).DO();
        }

        public object MakeQuery(object query)
        {
            var tmpQuery = query;

            if (Operations.Any())
            {
                tmpQuery = ((DataBase.IWhere)tmpQuery).WHERE;

                foreach (var operation in Operations)
                {
                    switch (operation.Key)
                    {
                        case EOperation.AND:
                            tmpQuery = ((DataBase.IOrAnd)tmpQuery).AND;
                            break;
                        case EOperation.CB:
                            tmpQuery = ((DataBase.ICloseBracket)tmpQuery).CB();
                            break;
                        case EOperation.OB:
                            tmpQuery = ((DataBase.IOpenBracket)tmpQuery).OB();
                            break;
                        case EOperation.Column:
                            tmpQuery = operation.Value.MakeQuery(tmpQuery);
                            break;
                        case EOperation.OR:
                            tmpQuery = ((DataBase.IOrAnd)tmpQuery).OR;
                            break;
                        default:
                            throw new Exception($"Unknown operation: {operation.Key}");
                    }
                }
            }

            return tmpQuery;
        }
    }
}