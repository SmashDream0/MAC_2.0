using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using AutoTable.Employee.Mechanisms.Forms;
using MAC_2.Model;
using System.Windows;
using MAC_2.Model.NavigationProperty;

namespace MAC_2.Model
{
    public class SelectionWell : MyTools.C_A_BaseFromAllDB
    {
        public SelectionWell(uint ID, bool CanEdit = true) : base(G.SelectionWell, ID, CanEdit)
        {
            ValueSelections = new ListNavigationPropertyTyped<Model.ValueSelection>(this, (model) => model.SelectionWellID);
            ValueSelections.OnAdd += (model) =>
            { model.Add(this); };

            {
                HashSet<uint> pollutionHashSet = new HashSet<uint>();

                ValueSelections.OnCanAdd += (model, e) =>
                {
                    if (pollutionHashSet.Contains(model.PollutionID))
                    { e.CanUse = false; }
                    else
                    { pollutionHashSet.Add(model.PollutionID); }
                };
            }
        }

        public Objecte Objecte
        { get; private set; }

        public bool Add(Objecte objecte)
        {
            if (ObjectID == objecte.ID)
            {
                Objecte = objecte;
                return true;
            }
            else
            { return false; }
        }

        public Sample Sample
        { get; private set; }

        public bool Add(Sample sample)
        {
            if (SampleID == sample.ID)
            {
                Sample = sample;
                return true;
            }
            else
            { return false; }
        }

        public Well Well
        { get; private set; }

        public bool Add(Well well)
        {
            if (WellID == well.ID)
            {
                Well = well;
                return true;
            }
            else
            { return false; }
        }
        #region Заполняем структуру SelectionWell, до конца не ясно, разобраться.Тут лежит структура и заполняется структура таблицы selectionWell, Mac_Test базы
        /*
         * Шо тут происходит, а происходит следующие. У нас имеется такой класс, как Т. Там храниться настройки и общая инфа объектов
         * Например в T.SelectionWell - это наша структура, которую мы ниже потом и используем. В структуре у нас имеется: 
         * 1 Well, 2 Sample, 3 Number, 4 DateTime, это все есть в БД софта, лезем и смотрим напрямую - Mac_Test(если в будущем не смениться, но стандартно mac)
         * выбираем таблицу selectionwell, там все есть. 
         */
        /// <summary>ID отбора</summary>
        public uint SampleID => T.SelectionWell.Rows.Get_UnShow<uint>(ID, C.SelectionWell.Sample);
        /// <summary>ID колодца</summary>
        public uint WellID => T.SelectionWell.Rows.Get_UnShow<uint>(ID, C.SelectionWell.Well);
        /// <summary>ID объекта</summary>
        public uint ObjectID => T.SelectionWell.Rows.Get_UnShow<uint>(ID, C.SelectionWell.Well, C.Well.Object);
        /// <summary>ID клиента</summary>
        public uint ClientID => T.SelectionWell.Rows.Get_UnShow<uint>(ID, C.SelectionWell.Well, C.Well.Object, C.Objecte.Client);
        /// <summary>Дата время в минутах</summary>
        public int YMDHM => T.SelectionWell.Rows.Get<int>(ID, C.SelectionWell.DateTime);
        /// <summary>Номер</summary>
        public int Number
        {
            get => T.SelectionWell.Rows.Get<int>(ID, C.SelectionWell.Number);
            set => T.SelectionWell.Rows.Set(ID, C.SelectionWell.Number, value);
        }
        /// <summary>Номер по формату</summary>
        public string FormatNumber => $"{Number}-С-{MyTools.YearMonth_From_YM(Sample.YM, MyTools.EDateTimeTypes.DivisionSymbol, "/")}";

        public ListNavigationPropertyTyped<ValueSelection> ValueSelections { get; private set; }
        #endregion

        public bool Add(ValueSelection valueSelection)
        {
            return ValueSelections.Add(valueSelection);
        }

        public override string ToString()
        {
            return Well.Number.ToString();
        }
    }
}
