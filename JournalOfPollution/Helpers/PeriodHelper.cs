using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2.Helpers
{
    public static class PeriodHelper
    {
        static PeriodHelper()
        {
            updatePeriod();
        }

        public static event Action OnPeriodChange;

        /// <summary>
        /// Настройки текущего периода
        /// </summary>
        public static Period CurrentPeriod
        {
            get;
            private set;
        }

        /// <summary>
        /// Настройки следующего периода
        /// </summary>

        public static Period NextPeriod
        {
            get;
            private set;
        }

        /// <summary>
        /// Настройки текущего месяца и настройки следующего месяца разные
        /// </summary>
        public static bool DifferentPeriods
        { get => CurrentPeriod.ID != NextPeriod.ID; }

        /// <summary>
        /// Номер текущего периода
        /// </summary>
        public static int YM
        {
            get => data.User<int>(C.User.CPeriod);
            set
            {
                if (YM != value)
                {
                    data.User<int>(C.User.CPeriod, value);

                    updatePeriod();

                    if (OnPeriodChange != null)
                    { OnPeriodChange(); }
                }
            }
        }

        private static void updatePeriod()
        {
            CurrentPeriod = Helpers.LogicHelper.PeiodLogic.FirstOrDefault(YM);
            NextPeriod = Helpers.LogicHelper.PeiodLogic.FirstOrDefault(YM + 1);
        }
    }
}
