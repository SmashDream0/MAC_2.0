using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Logic
{
    public class PollutionLogic
        : BaseLogicTyped<Pollution>
    {
        public PollutionLogic() : base(T.Pollution)
        { }

        protected override Pollution internalGetModel(uint id)
        { return new Pollution(id); }

        /// <summary>
        /// Получить список показателей с сортировкой по полю Number
        /// </summary>
        /// <returns></returns>
        public new IEnumerable<Pollution> Find()
        {
            return getQuerryResult($"all", (table) =>
            {
                table.QUERRY()
                       .SHOWL(C.Pollution.Number)
                       .WHERE
                       .AC(C.Pollution.Show).EQUI.BV(true)
                       //на случай если лезут лишние загрязнения!!!
                       //    .OB()
                       //        .C(C.Pollution.YMFrom,0)
                       //    .OR
                       //        .AC(C.Pollution.YMFrom).Less.BV(data.User<int>(C.User.CPeriod)+1)
                       //    .CB()
                       //.AND
                       //    .OB()
                       //        .C(C.Pollution.YMTo, 0)
                       //    .OR
                       //        .AC(C.Pollution.YMTo).More.BV(data.User<int>(C.User.CPeriod) - 1)
                       //    .CB()
                       .DO();

                var result = getModels(table).ToList();

                int i = 0;
                foreach (var pollution in result)
                {
                    pollution.Index = i;
                    pollution.BindName = i.ToString();
                    i++;
                }

                result.Sort((it1, it2) =>
                {
                    var resultOrder = it1.Number.CompareTo(it2.Number);

                    if (resultOrder == 0)
                    { resultOrder = it1.ID.CompareTo(it2.ID); }

                    return resultOrder;
                });

                return result.ToArray();
            });
        }
    }
}
