using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using MAC_2.Calc;

namespace MAC_2.Employee.Mechanisms
{
    public static class ControlHit_Class
    {
        /// <summary>Грузить все типы загрязнений</summary>
        public static void LoadHit()
        {
            G.HitModePollution.QUERRY().SHOW.DO();

            HitModePollutions = new HitModePollution[G.HitModePollution.Rows.Count];
            for(int i =0;i<HitModePollutions.Length;i++)
            { HitModePollutions[i] = new HitModePollution(G.HitModePollution.Rows.GetID(i)); }
        }

        /// <summary>Получить текущие косяки</summary>
        public static void LoadHitSelect()
        {
            G.HitSelectPollution.QUERRY()
                .SHOW
                .WHERE
                .C(C.HitSelectPollution.YM, DateControl_Class.SelectMonth)
                .DO();
            hitSelects = new HitSelectPollution[G.HitSelectPollution.Rows.Count];
            for (int i = 0; i < hitSelects.Length; i++)
            { hitSelects[i] = new HitSelectPollution(G.HitSelectPollution.Rows.GetID(i)); }
        }
        /// <summary>Протестит значения на косяки</summary>
        public static HitModePollution[] TestValues(BaseCalc_Class calc)
        {
            var last = LastHitObject(calc.Objecte.ID);
            List<HitModePollution> result = new List<HitModePollution>();
            foreach (var mode in HitModePollutions.Where(x => x.Joint && x.Formula.Length>0))
            {
                string formula = mode.Formula;
                HitSelectPollution joint = last.FirstOrDefault(x => x.IDHitModePollution == mode.ID);
                //formula = formula.Replace(Const_Class.Multiplier, mode.Multi(joint == null ? 0 : joint.Number).ToString());
                //formula = formula.Replace(Const_Class.Summa, calc.Answer.Sum(x=>x.Value.Summ).ToString());
                formula = formula.Replace(Const_Class.Limit, AdditionnTable.ListPeriod.Last(x => x.YM <= DateControl_Class.SelectMonth).MinLimits.ToString());

                if (mode.PollutionID > 0)
                {
                    if (calc.Value.FirstOrDefault(x => x.Pollution.ID == mode.PollutionID) != null)
                    {
                        BaseCalc_Class.SetConst(ref formula, calc.Value.First(x => x.Pollution.ID == mode.PollutionID));
                        if (Math_Class.Get_Comparison_Calc(formula))
                        { result.Add(mode); }
                    }
                }
                else
                {
                    foreach (var one in calc.Value)
                    {
                        BaseCalc_Class.SetConst(ref formula, one);
                        if (Math_Class.Get_Comparison_Calc(formula))
                        { result.Add(mode); }
                    }
                }
            }
            return result.ToArray();
        }


        /// <summary>Последние попадения</summary>
        private static HitSelectPollution[] LastHitObject(uint IDObj)
        {
            G.HitSelectPollution.QUERRY()
                .SHOW
                .WHERE
                    .C(C.HitSelectPollution.Object, IDObj)
                .AND
                    .AC(C.HitSelectPollution.YM).More.BV(DateControl_Class.SelectMonth - MaxMonthControl - 1)
                .DO();
            HitSelectPollution[] Hits = new HitSelectPollution[G.HitSelectPollution.Rows.Count];
            for (int i = 0; i < Hits.Length; i++)
            { Hits[i] = new HitSelectPollution(G.HitSelectPollution.Rows.GetID(i)); }
            return Hits.GroupBy(x => x.IDHitModePollution).Select(x => x.OrderBy(y => y.YM).Last()).ToArray();
        }
        private static HitSelectPollution[] hitSelects;
        private static HitModePollution[] HitModePollutions;
        static int MaxMonthControl = HitModePollutions==null?0: HitModePollutions.Where(x => x.MonthStartTack == 0).Max(x => x.MothControl);
    }
    public class HitModePollution : MyTools.C_A_BaseFromAllDB
    {
        public HitModePollution(uint ID, bool CanEdit = true) : base(G.HitModePollution, ID, CanEdit)
        { }
        /// <summary>Месяц начала отсчёта контроля</summary>
        public int MonthStartTack => T.HitModePollution.Rows.Get<int>(ID, C.HitModePollution.MonthStartTack);
        /// <summary>Колличество месяцев контроля</summary>
        public int MothControl => T.HitModePollution.Rows.Get<int>(ID, C.HitModePollution.MonthTrack);
        /// <summary>Формула</summary>
        public string Formula => T.HitModePollution.Rows.Get<string>(ID, C.HitModePollution.Formula);
        /// <summary>Косяк (если false то поощерение)</summary>
        public bool Joint => T.HitModePollution.Rows.Get<bool>(ID, C.HitModePollution.Joint);
        /// <summary>Множители</summary>
        public double Multi(int Number)
        {
            var result = T.HitModePollution.Rows.Get<string>(ID, C.HitModePollution.Multiplier).Replace(',', '.').Split('|');
            if (result.Length < Number)
            { return result.Last().TryParseDouble(); }
            return result[Number].TryParseDouble();
        }

        /// <summary>ID показателя загрязнения</summary>
        public uint PollutionID => T.HitModePollution.Rows.Get_UnShow<uint>(ID, C.HitModePollution.Pollution);
    }
    public class HitSelectPollution : MyTools.C_A_BaseFromAllDB
    {
        public HitSelectPollution(uint ID, bool CanEdit = true) : base(G.HitSelectPollution, ID, CanEdit)
        { }
        /// <summary>Колличество попадений</summary>
        public int Number => T.HitSelectPollution.Rows.Get<int>(base.ID, C.HitSelectPollution.Number);
        /// <summary>Месяц попадения</summary>
        public int YM => T.HitSelectPollution.Rows.Get<int>(base.ID, C.HitSelectPollution.YM);
        /// <summary>ID объекта</summary>
        public uint IDObj => T.HitSelectPollution.Rows.Get_UnShow<uint>(base.ID, C.HitSelectPollution.Object);
        /// <summary>ID тип загрязнения</summary>
        public uint IDHitModePollution => T.HitSelectPollution.Rows.Get_UnShow<uint>(base.ID, C.HitSelectPollution.HitModePollution);
    }
}