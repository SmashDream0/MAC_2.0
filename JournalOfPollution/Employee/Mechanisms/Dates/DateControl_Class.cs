using AutoTable;
using MAC_2.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using MAC_2.Model;

namespace MAC_2.Employee.Mechanisms
{
    public static class DateControl_Class
    {
        public static void Selectors(WrapPanel WP)
        {
            MassPeriod = Helpers.LogicHelper.PeiodLogic.Find().ToArray();

            Drawning(WP);
        }

        public static event Action OnPeriodChange;

        private static Model.Period[] MassPeriod;

        private static ComboBox CB_Month;
        /// <summary>отрисовать</summary>
        private static void Drawning(WrapPanel WP)
        {
            WP.Children.Clear();

            CB_Month = new ComboBox();
            CB_Month.FillComboBoxMonth();
            CB_Month.ToolTip = "Период";
            CB_Month.MinWidth = CB_Month.MaxWidth = 100;
            CB_Month.Background = Brushes.LightSeaGreen;
            TextBox TBYear = new TextBox();
            CB_Month.FontSize = TBYear.FontSize = 14;

            TBYear.TextChanged += (sender, e) =>
              { DataBase.NoABC_Int_Dinamic(TBYear); };

            TBYear.KeyDown += (sender, e) =>
              {
                  if (e.Key == System.Windows.Input.Key.Enter)
                  {
                      var year = TBYear.Text.TryParseInt();
                      SelectMonth = year * 12 + MyTools.M_From_YM(SelectMonth);
                      CB_Month.Text = MyTools.Month_From_M_C_R(MyTools.M_From_YM(SelectMonth));
                      TBYear.Text = SelectYear.ToString();
                  }
              };

            Button BTNext = new Button();
            BTNext.Content = ">";

            Button BTBack = new Button();
            BTBack.Content = "<";

            BTNext.Click += (sender, e) =>
            {
                CB_Month.SelectionChanged -= CB_SelectionChanged;
                SelectMonth++;
                CB_Month.SelectedIndex = MyTools.M_From_YM(SelectMonth) - 1;
                TBYear.Text = SelectYear.ToString();
                CB_Month.SelectionChanged += CB_SelectionChanged;
            };

            BTBack.Click += (sender, e) =>
            {
                CB_Month.SelectionChanged -= CB_SelectionChanged;
                SelectMonth--;
                CB_Month.SelectedIndex = MyTools.M_From_YM(SelectMonth) - 1;
                TBYear.Text = SelectYear.ToString();
                CB_Month.SelectionChanged += CB_SelectionChanged;
            };

            CB_Month.SelectedIndex = MyTools.M_From_YM(SelectMonth) - 1;
            CB_Month.SelectionChanged += CB_SelectionChanged;
            TBYear.Text = SelectYear.ToString();
            WP.Children.Add(BTBack);
            WP.Children.Add(CB_Month);
            WP.Children.Add(TBYear);
            WP.Children.Add(BTNext);
        }

        private static void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectMonth = SelectYear * 12 + CB_Month.SelectedIndex + 1;
        }

        private static void Start()
        {
            PollutionBase_Class.LoadSample();

            if (OnPeriodChange != null)
            { OnPeriodChange(); }
        }
        /// <summary>Выбранный месяц</summary>
        public static int SelectMonth
        {
            get
            {
                int result = Helpers.PeriodHelper.YM;

                if (result == 0)
                { result = MyTools.YM_From_Y_M(DateTime.Now.Year, DateTime.Now.Month); }

                return result;
            }
            set
            {
                Helpers.PeriodHelper.YM = value;
                Start();
            }
        }
        /// <summary>Выбранный год</summary>
        public static int SelectYear => MyTools.Y_From_YM(SelectMonth);
        /// <summary>получить тариф на текущий месяц</summary>
        public static Model.Period SelectPeriod => MassPeriod.FirstOrDefault(x => x.YM == SelectMonth);
        /// <summary>Является ли выбранный месяц текущим</summary>
        public static bool ThisMonth => SelectMonth == MyTools.GetNowDate(MyTools.EInputDate.YM);
        /// <summary>Является ли выбранный месяц текущим в рамках дней</summary>
        public static bool ThisMonthDay => SelectMonth >= MyTools.GetNowDate(MyTools.EInputDate.YM) - 1 && DateTime.Now.Day < 22;
        /// <summary>Проверка на закрытие</summary>
        public static bool ControlMonth()
        {
            if (DateTime.Now.Day >= 22)
            {
                List<fromMessage> mess = new List<fromMessage>();

                {
                    var dictionary = new Dictionary<uint, fromMessage>();

                    var selectionWells = Helpers.LogicHelper.SelectionWellLogic.Find((int)MyTools.GetNowDate(MyTools.EInputDate.YM), 0);

                    foreach (var selectionWell in selectionWells)
                    {
                        var objecte = selectionWell.Objecte;

                        if (!dictionary.ContainsKey(objecte.ID))
                        {
                            mess.Add(new fromMessage(selectionWell));
                        }
                    }
                }
                //PollutionBase_Class.LoadSample(false, IDObjMass.ToArray());
                //PollutionBase_Class.GetValueMass(false, IDObjMass.ToArray());
                if (mess.Count > 0)
                {
                    CustomMessage_Window CS_W = new CustomMessage_Window(new CustomMessage_Window.SettingWindow(Title: "Сообщение", CanResize: true));
                    var query = G.Volume.QUERRY().SHOW
                        .WHERE
                        .ID(0);
                    foreach (var one in mess)
                    { query.OR.C(C.Volume.Sample, one.Sample.ID); }
                    query.DO();
                    var count = G.Volume.Rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        uint SampleID = G.Volume.Rows.Get_UnShow<uint>(i, C.Volume.Sample);
                        mess.First(x => x.Sample.ID == SampleID).SetVolume(G.Volume.Rows.GetID(i));
                    }
                    CS_W.MessageText.Text = "Списки отборов не имеющих статуса";
                    if (mess.Where(x => x.Volume == 0).Count() > 0)
                    {
                        TextBlock text = new TextBlock();
                        text.Text = "Объём нулевой";
                        text.Background = Brushes.LightBlue;
                        CS_W.ShowControl.SetRowFromGrid(MyTools.GL_Auto);
                        CS_W.ShowControl.SetFromGrid(text, Column: 0);
                        foreach (var one in mess.Where(x => x.Volume == 0))
                        {
                            CheckBox CB = new CheckBox();
                            TextBlock tb = new TextBlock();
                            tb.Background = Brushes.LightBlue;
                            tb.Text = one.Objecte.Client.Detail.FullName + " - " + one.Objecte.Adres + " папка №" + one.Objecte.NumberFolder + " от " + MyTools.YearMonth_From_YM(one.Sample.YM);
                            CB.Click += (sender, e) =>
                             { one.Status = (bool)CB.IsChecked ? data.EStatus.NotVolume : data.EStatus.None; };
                            CB.Content = tb;
                            CS_W.ShowControl.SetRowFromGrid(MyTools.GL_Auto);
                            CS_W.ShowControl.SetFromGrid(CB);
                        }
                    }
                    if (mess.Where(x => x.Volume > 0).Count() > 0)
                    {
                        TextBlock text = new TextBlock();
                        text.Text = "Сумма меньше лимита";
                        text.Background = Brushes.LightCoral;
                        CS_W.ShowControl.SetRowFromGrid(MyTools.GL_Auto);
                        CS_W.ShowControl.SetFromGrid(text, Column: 0);
                        foreach (var one in mess.Where(x => x.Volume > 0))
                        {
                            if (one.Calc())
                            { continue; }
                            CheckBox CB = new CheckBox();
                            TextBlock tb = new TextBlock();
                            tb.Background = Brushes.LightCoral;
                            tb.Text = $"{one.Objecte.Client.Detail.FullName} - {one.Objecte.Adres} папка №{one.Objecte.NumberFolder} {one.Summs}";
                            CB.Content = tb;
                            CB.Click += (sender, e) =>
                            { one.Status = (bool)CB.IsChecked ? data.EStatus.NotLimit : data.EStatus.None; };
                            CS_W.ShowControl.SetRowFromGrid(MyTools.GL_Auto);
                            CS_W.ShowControl.SetFromGrid(CB, Column: 0);
                        }
                    }
                    CS_W.ShowDialog();
                }
                return ThisMonth;
            }
            return true;
        }
        class fromMessage
        {
            public fromMessage(SelectionWell selectionWell)
            {
                this.SelectionWell = selectionWell;
                volume = new List<Volume>();
                _644 = null;
                _621 = null;
            }
            public SelectionWell SelectionWell
            { get; private set; }

            public Sample Sample => SelectionWell.Sample;
            public Objecte Objecte => SelectionWell.Objecte;

            public data.EStatus Status;
            public void SetVolume(uint ID)
            {
                volume.Add(Helpers.LogicHelper.VolumeLogic.FirstModel(ID));
            }
            List<Volume> volume;
            public double Volume => volume.Count > 0 ? volume.Max(x => x.Value) : 0;

            public bool Calc()
            {
                _644 = new Calc_644(SelectionWell.Sample, SelectionWell.Objecte, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644")));
                _644.Calc();
                _621 = new Calc_621(SelectionWell.Sample, SelectionWell.Objecte, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621")));
                _621.Calc();
                return ((_644.Answer == null || _644.Answer.Length == 0 ? 0 : _644.Answer.Max(x => x.Value.SummNDS))
                    >
                    (_621.Answer == null || _621.Answer.Length == 0 ? 0 : _621.Answer.Max(x => x.Value.SummNDS))
                    ?
                    (_644.Answer == null || _644.Answer.Length == 0 ? 0 : _644.Answer.Max(x => x.Value.SummNDS))
                    :
                    (_621.Answer == null || _621.Answer.Length == 0 ? 0 : _621.Answer.Max(x => x.Value.SummNDS)))
                    > AdditionnTable.GetPeriod.MinLimits;
            }
            Calc_644 _644;
            Calc_621 _621;
            public string Summs
            {
                get
                {
                    string result = (_644.Answer == null || _644.Answer.Length == 0 ? string.Empty : $" 644 - {_644.Answer.Max(x => x.Value.SummNDS).ToMoney()}");
                    result += (_621.Answer == null || _621.Answer.Length == 0 ? string.Empty : $" 621 - {_621.Answer.Max(x => x.Value.SummNDS).ToMoney()}");
                    return result;
                }
            }
        }
    }
}