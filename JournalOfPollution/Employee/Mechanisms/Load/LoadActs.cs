using AutoTable;
using MAC_2.Calc;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using MAC_2.Model;

namespace MAC_2.Employee.Mechanisms
{
    public class LoadActs : MyTools.C_A_BaseLoad_Excel
    {
        public LoadActs()
        {
            DG.CanUserDeleteRows =
            DG.CanUserAddRows =
            DG.AutoGenerateColumns = false;
            DG.LoadingRow += (sender, e) =>
              {
                  var obj = (e.Row.Item as show);
                  if (obj.VolumeID > 0)//всё ок
                  { e.Row.Background = Brushes.Green; }
                  else if (obj.number > 0)//не нашёлся объём
                  { e.Row.Background = Brushes.LightGreen; }
                  else if (obj.client != null)//есть попадение по клиенту
                  { e.Row.Background = Brushes.Yellow; }
                  else if (obj.client == null)//вообще ничего не нашлось
                  { e.Row.Background = Brushes.Red; }
                  else
                  { e.Row.Background = Brushes.White; }
              };
        }

        public override bool StartLoad()
        {
            G.NormDoc.QUERRY()
                .SHOW
                .WHERE
                .ARC(C.NormDoc.Volume, C.Volume.Sample, C.Sample.YM).EQUI.BV(YM - 1)
                .DO();
            NormDoc[] normDocs = new NormDoc[G.NormDoc.Rows.Count];
            for (int i = 0; i < normDocs.Length; i++)
            { normDocs[i] = new NormDoc(G.NormDoc.Rows.GetID(i)); }
            show[] _temp = shows.ToArray();
            load_sw load = new load_sw(_temp.Length, new Func<int, bool>(
                IndexRow =>
                {
                    show temp = _temp[IndexRow];
                    if (temp.VolumeID > 0)
                    {
                        if (normDocs.FirstOrDefault(x => temp.act == x.Act && temp.invoces == x.Invoces && temp.score == x.Score) == null)
                        {
                            MyTools.AddRowFromTable(G.NormDoc,
                             new KeyValuePair<int, object>(C.NormDoc.Act, temp.act),
                             new KeyValuePair<int, object>(C.NormDoc.Score, temp.score),
                             new KeyValuePair<int, object>(C.NormDoc.Invoces, temp.invoces),
                             new KeyValuePair<int, object>(C.NormDoc.Volume, temp.VolumeID),
                             new KeyValuePair<int, object>(C.NormDoc.Date, temp._date),
                             new KeyValuePair<int, object>(C.NormDoc.Summ, temp.summ),
                             new KeyValuePair<int, object>(C.NormDoc.Resolution, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains(temp.type_calc)).ID)
                             );
                        }

                        shows.Remove(temp);
                    }

                    return true;
                }));
            _progress = new Progress_Form(load);
            _progress.ShowDialog();
            DG.ItemsSource = null;
            DG.ItemsSource = shows;
            return true;
        }

        #region колонки и обработка

        struct col
        {
            public const string ls = "лс";
            public const string name = "наименование предприятия";
            public const string inn = "инн";
            public const string date = "дата";
            public const string type_calc = "вид расчета";
            public const string score = "номер счета";
            public const string invoces = "номер счет-фактуры";
            public const string act = "номер акта";
            public const string summ = "сумма";
            public const string adres = "место отбора";
        }

        protected override void GetColumn()
        {
            column = new string[]
            {
                col.ls,
                col.name,
                col.inn,
                col.date,
                col.type_calc,
                col.score,
                col.invoces,
                col.act,
                col.summ,
                col.adres
            };
        }

        protected override void GetDGColumn()
        {
            MyTools.C_Setting_DGColumn setting = new MyTools.C_Setting_DGColumn();
            List<MyTools.C_DGColumn_Bind> list = new List<MyTools.C_DGColumn_Bind>
            {
                new MyTools.C_DGColumn_Bind("номер отбора", MyTools.E_TypeColumnDG.Text, "number",setting),
                new MyTools.C_DGColumn_Bind(col.ls, MyTools.E_TypeColumnDG.Text, "ls",setting),
                new MyTools.C_DGColumn_Bind(col.name, MyTools.E_TypeColumnDG.Text, "name",setting),
                new MyTools.C_DGColumn_Bind(col.inn, MyTools.E_TypeColumnDG.Text, "inn",setting),
                new MyTools.C_DGColumn_Bind(col.date, MyTools.E_TypeColumnDG.Text, "date",setting),
                new MyTools.C_DGColumn_Bind(col.type_calc, MyTools.E_TypeColumnDG.Text, "type_calc",setting),
                new MyTools.C_DGColumn_Bind(col.score, MyTools.E_TypeColumnDG.Text, "score",setting),
                new MyTools.C_DGColumn_Bind(col.invoces, MyTools.E_TypeColumnDG.Text, "invoces",setting),
                new MyTools.C_DGColumn_Bind(col.act, MyTools.E_TypeColumnDG.Text, "act",setting),
                new MyTools.C_DGColumn_Bind(col.summ, MyTools.E_TypeColumnDG.Text, "summ",setting),
                new MyTools.C_DGColumn_Bind(col.adres, MyTools.E_TypeColumnDG.Text, "adres",setting),
            };
            ColumnsFromDG = list.ToArray();
        }

        #endregion

        protected override void LoadMenu()
        {

        }
        int YM;
        protected override void Start()
        {
            string dat = SearthCellFromMark("Дата начала: ", false).StringCellValue;
            YM = (int)MyTools.StringDATE_In_intDate(dat.Substring(dat.IndexOf(':') + 1), MyTools.EInputDate.YMDHMS, MyTools.EInputDate.YM);
            LoadSelection();
            shows = new List<show>();
            progressOpenFile openFile = new progressOpenFile(sheet.LastRowNum - StartRow, new Func<int, bool>(
                RowIndex =>
                {
                    IRow row = sheet.GetRow(RowIndex);
                    calc[] _calc = null;
                    Dictionary<string, object> value = new Dictionary<string, object>();
                    foreach (var one in ColumnsBook)
                    {
                        string val = row.GetCell(one.Value).ToString();
                        if (one.Key == col.ls)
                        {
                            if (!val.Contains('\''))
                            { throw new Exception($"Колонка {col.ls} не содержит символ: \'"); }
                            val = val.Replace("\'", "");
                            _calc = calcs.Where(x => x.objecte.Accounts.Contains(val)).ToArray();
                        }
                        if (_calc.Length == 0 && one.Key == col.inn)
                        { _calc = calcs.Select(x => new { client = x.client, calc = x }).Where(x => x.client.INN == val).Select(x => x.calc).ToArray(); }

                        value.Add(one.Key, val);
                    }
                    shows.Add(new show(_calc, value));
                    return true;
                }));
            _progress = new Progress_Form(openFile);
            _progress.ShowDialog();
            DG.ItemsSource = shows;
        }
        calc[] calcs;
        private void LoadSelection()
        {
            G.SelectionWell.QUERRY()
                .SHOW
                .WHERE
                .ARC(C.SelectionWell.Sample, C.Sample.YM).EQUI.BV(YM - 1)
                .DO();
            calcs = new calc[G.SelectionWell.Rows.Count];
            load_sw openFile = new load_sw(calcs.Length, new Func<int, bool>(
                IndexRow =>
                {
                    calcs[IndexRow] = new calc(G.SelectionWell.Rows.GetID(IndexRow));
                    return true;
                }));
            _progress = new Progress_Form(openFile);
            _progress.ShowDialog();
        }
        protected class load_sw : Progress_Form.AObject
        {
            public load_sw(int length, Func<int, bool> deleg) : base(true)
            {
                _MaxCount = length;
                this.deleg = deleg;
            }
            Func<int, bool> deleg;
            protected override bool Do()
            {
                for (int i = 0; i < _MaxCount; i++)
                {
                    Action(i);
                    if (!(bool)deleg?.Invoke(i))
                    { return false; }
                }
                return true;
            }
        }

        List<show> shows;
        class show
        {
            public show(calc[] calc, Dictionary<string, object> values)
            {
                foreach (var one in values)
                {
                    switch (one.Key)
                    {
                        case col.ls:
                            {
                                ls = one.Value.ToString();
                                break;
                            }
                        case col.name:
                            {
                                name = one.Value.ToString();
                                break;
                            }
                        case col.inn:
                            {
                                inn = one.Value.ToString();
                                break;
                            }
                        case col.date:
                            {
                                _date = (int)MyTools.StringDATE_In_intDate(one.Value.ToString(), MyTools.EInputDate.YMDHMS, MyTools.EInputDate.YMD);
                                break;
                            }
                        case col.type_calc:
                            {
                                type_calc = one.Value.ToString().ToLower() == "нв" ? "644" : one.Value.ToString().ToLower() == "пдк" ? "621" : "не найдено";
                                break;
                            }
                        case col.score:
                            {
                                score = one.Value.ToString();
                                break;
                            }
                        case col.invoces:
                            {
                                invoces = one.Value.ToString();
                                break;
                            }
                        case col.act:
                            {
                                act = one.Value.ToString();
                                break;
                            }
                        case col.summ:
                            {
                                summ = one.Value.TryParseDecimal();
                                break;
                            }
                        case col.adres:
                            {
                                adres = one.Value.ToString();
                                break;
                            }
                    }
                }
                if (calc.Length > 0)
                {
                    if (calc.Length == 1)
                    { this.calc = calc.First(); }
                    else
                    {
                        switch (type_calc)
                        {
                            case "621":
                                {
                                    this.calc = calc.FirstOrDefault(x => x.calc_621.Answer != null && x.calc_621.Answer.Select(y => y.Value.SummNDS).Contains(summ));
                                    break;
                                }
                            case "644":
                                {
                                    this.calc = calc.FirstOrDefault(x => x.calc_644.Answer != null && x.calc_644.Answer.Select(y => y.Value.SummNDS).Contains(summ));
                                    break;
                                }
                        }
                    }
                    if (this.calc == null)
                    { client = calc.First().client; }
                    else
                    {
                        //алимпик не подходит по сумме, которая тут считает не правильно
                        switch (type_calc)
                        {
                            case "621":
                                {
                                    if (this.calc.calc_621.TotalSumNDS == summ)
                                    {
                                        var volume = this.calc.volume.FirstOrDefault(x => x.PeriodID == Helpers.PeriodHelper.CurrentPeriod.ID);
                                        VolumeID = (volume == null ? 0 : volume.ID);
                                    }
                                    break;
                                }
                            case "644":
                                {
                                    if (this.calc.calc_644.TotalSumNDS == summ)
                                    {
                                        var volume = this.calc.volume.FirstOrDefault(x => x.PeriodID == Helpers.PeriodHelper.CurrentPeriod.ID);
                                        VolumeID = (volume == null ? 0 : volume.ID);
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            
            public uint VolumeID { get; private set; }
            public Client client;
            public calc calc;

            public int number => calc != null ? calc.sw.Number : 0;
            public string ls { get; }
            public string name { get; }
            public string inn { get; }
            public int _date;
            public string date => MyTools.YearMonthDay_From_YMD(_date);
            public string type_calc { get; }
            public string score { get; }
            public string invoces { get; }
            public string act { get; }
            public decimal summ { get; }
            public string adres { get; }
        }

        class calc
        {
            public calc(uint selectionID)
            {
                sw = Helpers.LogicHelper.SelectionWellLogic.FirstModel(selectionID);
                client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.ClientID);
                objecte = Helpers.LogicHelper.ObjecteLogic.FirstModel(sw.ObjectID);

                calc_621 = new Calc_621(sw.Sample, objecte, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("621")));
                calc_621.Calc();
                calc_644 = new Calc_644(sw.Sample, objecte, PollutionBase_Class.AllResolution.First(x => x.CurtName.Contains("644")));
                calc_644.Calc();
                volume = sw.Sample.Volumes.ToArray();
            }
            public Volume[] volume;
            public SelectionWell sw;
            public Objecte objecte;
            public Client client;
            public Calc_621 calc_621;
            public Calc_644 calc_644;
        }
    }
}