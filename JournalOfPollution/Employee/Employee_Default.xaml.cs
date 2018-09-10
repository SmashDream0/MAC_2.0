using AutoTable;
using MAC_2.Calc;
using MAC_2.Employee;
using MAC_2.Employee.HelpSelect;
using MAC_2.Employee.Mechanisms;
using MAC_2.Employee.Mechanisms.LoadVolume;
using MAC_2.Employee.Windows;
using MAC_2.PrintForm;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MAC_2.EmployeeWindow
{
    /// <summary>
    /// Логика взаимодействия для Employee_Window.xaml
    /// </summary>
    public partial class Employee_Window : Window
    {
        public Employee_Window()
        {
            BaseStart.Start();
            InitializeComponent();
            DateControl_Class.ControlMonth();
            this.GetSetting();
            LoadMenu();
            ElementControl();
            InstructionsMessage_Class.LoadInstructions(ThisMenu, data.ETypeInstruction.DefWindow);

            SC = new View_Class(ShowGrid, SearchText);

            DateControl_Class.Selectors(SelectorDate);

            DateControl_Class.OnPeriodChange += Action;
            ShowGrid.MouseDoubleClick += (sender, e) => Action();

            Action();
        }
        /// <summary>Обновление окна</summary>
        private void Action()
        {
            Helpers.LogicHelper.ClearCacheAll();

            ElementControl();
            SC.CreateColumn();
            SC.DrawColumns();
            SC.ColumnsSelector(Columns);
        }
        SearchGrid_Window SG;
        View_Class SC;
        /// <summary>Контроль над элементами</summary>
        private void ElementControl()
        { /*Negotiation.IsEnabled = DateControl_Class.ThisMonth;*/ }
        private void Adres_Click(object sender, RoutedEventArgs e)
        { G.Adres.GetAutoForm().ShowDialog(); }

        private void Workers_Click(object sender, RoutedEventArgs e)
        {
            G.Worker.QUERRY().SHOW.DO();
            G.Worker.GetAutoForm().ShowDialog();
        }

        #region Поисковик ОБЪЕКТОВ

        private void ClientsObjects_Click(object sender, RoutedEventArgs e)
        {
            var objs = Helpers.LogicHelper.ObjecteLogic.Find(DateControl_Class.SelectMonth).ToArray();

            foreach (var objecte in objs)
            { objecte.InitializeColumns(); }

            SG = new SearchGrid_Window(objs
                                     , new C_SettingSearchDataGrid(ThisDelegate: MouseDoubleClick
                                                                 , ColorConditions: new ColorCondition(column.DateClose
                                                                                                     , (text) => text.TryParseInt() > 0 && text.TryParseInt() <= DateControl_Class.SelectMonth
                                                                                                     , Brushes.Red)
                                     , DopText: $"Последний номер папки: {G.Objecte.QUERRY().GET.C(C.Objecte.NumberFolder).Max(C.Objecte.NumberFolder).By(C.Objecte.NumberFolder).DO()[0].Value}"));

            {
                var MiAdd = new MenuItem();
                MiAdd.Header = "Добавить";
                MiAdd.Click += (senderAdd, eAdd) =>
                  {
                      G.Client.QUERRY().SHOW.WHERE.AC(C.Client.INN).EQUI.BV("000").DO();
                      uint clientID;

                      if (G.Client.Rows.Count == 0)
                      {
                          clientID = MyTools.AddRowFromTable(G.Client, new KeyValuePair<int, object>(C.Client.INN, "000"), new KeyValuePair<int, object>(C.Client.YMFrom, DateControl_Class.SelectMonth - 1));
                      }
                      else
                      { clientID = G.Client.Rows.GetID(0); }

                      SG.SelectWindowSearth(new Client_Window(clientID));

                      SG.ReSet(Helpers.LogicHelper.ClientsLogic.Find(DateControl_Class.SelectMonth).ToArray());
                  };
                SG.ThisMenu.Items.Add(MiAdd);
            }

            {
                var btShowAll = new Button { Content = "Показать все" };
                btShowAll.Click += (senderS, eS) =>
                  {
                      SG.ReSet(Helpers.LogicHelper.ClientsLogic.Find(DateControl_Class.SelectMonth).ToArray());
                      btShowAll.IsEnabled = false;
                  };
                SG.ThisMenu.Items.Add(btShowAll);
            }

            this.SelectWindowSearth(SG);
        }

        public struct column
        {
            public const string Name = "Наименование";
            public const string Adres = "Адрес";
            public const string INN = "ИНН";
            public const string NumberFolder = "Номер папки";
            public const string DateClose = "Дата закрытия";
        }

        #endregion

        private void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SG.SelectID != 0)
            {
                var clientID = T.Objecte.Rows.Get_UnShow<uint>(SG.SelectID, C.Objecte.Client);

                SG.SelectWindowSearth(new Client_Window(clientID));
            }
        }

        private void PollutionsNorms_Click(object sender, RoutedEventArgs e)
        {
            this.SelectWindowSearth(new Norm_Window());

            Helpers.LogicHelper.ClearQuerryCacheAll();

            Action();
        }

        /// <summary>Загрузка дополнительного меню</summary>
        private void LoadMenu()
        {
            if (data.User<uint>(C.User.UType) != (uint)data.UType.MainWork)
            {
                Sync.IsEnabled =
                Loader.IsEnabled = false;
            }
            #region администратор
            if (data.User<uint>(C.User.UType) == (uint)data.UType.Admin)
            {
                #region Редактор формул
                MenuItem MI_Formul = new MenuItem();
                MI_Formul.Header = "Редактор формул";
                MI_Formul.Background = Brushes.LightGreen;
                MI_Formul.Click += (senrer, e) => { this.SelectWindowSearth(new GeneratorCalc_Window()); };
                Reference.Items.Add(MI_Formul);
                #endregion                
                Sync.IsEnabled = Loader.IsEnabled = true;
            }
            #endregion
        }

        private void Negotiation_Click(object sender, RoutedEventArgs e)
        { this.SelectWindowSearth(new Selector_Window(), Action); }

        private void NumberSelect_Click(object sender, RoutedEventArgs e)
        { SC.Filter(View_Class.EFilter.Number); }

        private void Summ621_Click(object sender, RoutedEventArgs e)
        { SC.Filter(View_Class.EFilter.Summ621); }

        private void Summ644_Click(object sender, RoutedEventArgs e)
        { SC.Filter(View_Class.EFilter.Summ644); }

        #region загрузка из файлов

        LoadExcelBook_Window load;
        private void LoadVolume_Click(object sender, RoutedEventArgs e)
        {
            load = new LoadExcelBook_Window(new LoadVolumes(), null);
            load.ShowDialog();
            if (load.NewIsLoad)
            {
                PollutionBase_Class.LoadSample();
                SC.DrawColumns();
            }
        }

        private void LoadFromJCalc_Click(object sender, RoutedEventArgs e)
        {
            load = new LoadExcelBook_Window(new SyncFromCalc(), null);
            load.ShowDialog();
            if (load.NewIsLoad)
            {
                PollutionBase_Class.LoadSample();
                SC.DrawColumns();
            }
        }

        #endregion

        private void Reports_Click(object sender, RoutedEventArgs e)
        { MyTools.OpenFolder($"{Directory.GetCurrentDirectory()}\\Отчёты", true, true); }
        BasePrint print;
        private void Registry_Click(object sender, RoutedEventArgs e)
        {
            print = new Registry_Print_Class();
            print.Start();
        }

        private void DeclairWell_Click(object sender, RoutedEventArgs e)
        {
            List<C_ColumnFromSearch> columns = new List<C_ColumnFromSearch>
            {
                new C_ColumnFromSearch(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.Object, C.Objecte.Client, C.Client.INN),
                new C_ColumnFromSearch(C.DeclarationValue.Declaration, (obj,id)=>{ return AllClients.ClientAtWell(id).Detail.FullName; }),
                new C_ColumnFromSearch(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.Object, C.Objecte.AdresFact),
                new C_ColumnFromSearch(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.TypeWell, C.TypeWell.CurtName),
                new C_ColumnFromSearch(C.DeclarationValue.Declaration, C.Declaration.Well, C.Well.Number),
                new C_ColumnFromSearch(C.DeclarationValue.Declaration,(obj,ID)=>{return MyTools.YearMonth_From_YM(obj.TryParseInt()); }, C.Declaration.YM),
                new C_ColumnFromSearch(C.DeclarationValue.Pollution),
                new C_ColumnFromSearch(C.DeclarationValue.From),
                new C_ColumnFromSearch(C.DeclarationValue.To)
            };
            SG = new SearchGrid_Window(G.DeclarationValue,
                new C_ColumnGetID(C.DeclarationValue.Declaration,C.Declaration.Well),
                columns, 
                new C_SettingSearchDataGrid(true, true, false, AdderButton: false),true);
            SG.ShowDialog();
        }

        private void LoadActs_Click(object sender, RoutedEventArgs e)
        {
            load = new LoadExcelBook_Window(new LoadActs(), null);
            load.ShowDialog();
        }

        private void Period_Click(object sender, RoutedEventArgs e)
        {
            SG = new SearchGrid_Window(G.Period, null, null, new C_SettingSearchDataGrid(table:G.Period), true);
            SG.ShowDialog();

            AdditionnTable.LoadPeriod();
        }

        private void Journal_Click(object sender, RoutedEventArgs e)
        {
            print = new Journal_Print_Class();
            print.Start();
        }
    }
}