using AutoTable;
using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MAC_2.Model;

namespace MAC_2.Employee.Windows
{
    public class Client_Class
    {
        public Client_Class(TabControl TB, uint clientID)
        {
            this.ThisTC = TB;
            this.clientID = clientID;
            WH = new MyTools.C_MinMaxWidthHeight(MinWidth: 80, HorAlig: HorizontalAlignment.Left, Wrap: true, MinMaxName: 250);
            SFE = new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Vertical, true, -1, true, false, true);
            DrawClient();
            DrawObject();
        }

        uint clientID;
        MyTools.C_SettingFromRowEdit SFE;
        MyTools.C_MinMaxWidthHeight WH;
        TabControl ThisTC;
        Client client;

        struct Column
        {
            public static MyTools.C_DefColumn[] Client =
                {
                new MyTools.C_DefColumn(C.Client.INN,EnterInput:true),
                new MyTools.C_DefColumn(C.Client.CanSelect),
                new MyTools.C_DefColumn(C.Client.MustDeclair),
                new MyTools.C_DefColumn(C.Client.TypeClient),
                new MyTools.C_DefColumn(C.Client.YMFrom ),
                new MyTools.C_DefColumn(C.Client.YMTo)
                };
            public static MyTools.C_DefColumn[] DetailsClient =
                {
                new MyTools.C_DefColumn(C.DetailsClient.FullName),
                new MyTools.C_DefColumn(C.DetailsClient.AdresP),
                new MyTools.C_DefColumn(C.DetailsClient.AdresUr),
                new MyTools.C_DefColumn(C.DetailsClient.NameInDative),
                new MyTools.C_DefColumn(C.DetailsClient.YM)
                };
            public static MyTools.C_DefColumn[] DetailsObject =
                {
                new MyTools.C_DefColumn(C.DetailsObject.AddName),
                new MyTools.C_DefColumn(C.DetailsObject.YM)
                };
            public static MyTools.C_DefColumn[] Objecte =
                {
                new MyTools.C_DefColumn(C.Objecte.AdresFact),
                new MyTools.C_DefColumn(C.Objecte.TypeSample),
                new MyTools.C_DefColumn(C.Objecte.NumberFolder,EnterInput:true),
                new MyTools.C_DefColumn(C.Objecte.Separate),
                new MyTools.C_DefColumn(C.Objecte.Account1C),
                new MyTools.C_DefColumn(C.Objecte.YMFrom),
                new MyTools.C_DefColumn(C.Objecte.YMTo)
                };
            public static MyTools.C_DefColumn[] ObjectFromResolution =
                {
                new MyTools.C_DefColumn(C.ObjectFromResolution.Resolution),
                new MyTools.C_DefColumn(C.ObjectFromResolution.Application),
                new MyTools.C_DefColumn(C.ObjectFromResolution.Reason)
                };
            public static MyTools.C_DefColumn[] Well =
                {
                new MyTools.C_DefColumn(C.Well.TypeWell),
                new MyTools.C_DefColumn(C.Well.Unit),
                new MyTools.C_DefColumn(C.Well.Number,EnterInput:true),
                new MyTools.C_DefColumn(C.Well.YMFrom),
                new MyTools.C_DefColumn(C.Well.YMTo)
                };
            public static MyTools.C_DefColumn[] Declaration =
                {
                new MyTools.C_DefColumn(C.Declaration.Name),
                new MyTools.C_DefColumn(C.Declaration.YM)
                };
            public static MyTools.C_DefColumn[] MidMonthVolume =
                {
                new MyTools.C_DefColumn(C.MidMonthVolume.Year),
                new MyTools.C_DefColumn(C.MidMonthVolume.Volume)
                };
        }

        private void DrawClient()
        {
            //client = Helpers.LogicHelper.ClientsLogic.FirstOrDefault(DateControl_Class.SelectMonth, clientID);
            client = Helpers.LogicHelper.ClientsLogic.FirstModel(clientID);

            if (client != null)
            {
                ThisTC.Items.Clear();
                TabItem TI = new TabItem();
                ThisTC.Items.Add(TI);
                TI.Header = "Клиент";
                Grid Gr = new Grid();
                TI.Content = Gr;
                Gr.SetRowFromGrid(MyTools.GL_Auto);
                Gr.SetFromGrid(client.GetEditor(SFE, WH, Column.Client));
                Gr.SetRowFromGrid(MyTools.GL_Auto);
                Rectangle R = new Rectangle();
                R.Fill = Brushes.LightBlue;
                Gr.SetFromGrid(R, RowSpan: 2);
                TextBlock text = new TextBlock();
                text.Text = "Реквизиты клиента";
                text.FontSize = 20;
                Gr.SetFromGrid(text);
                Gr.SetRowFromGrid(MyTools.GL_Auto);
                TabControl TC = new TabControl();
                TC.Margin = new Thickness(2);
                Gr.SetFromGrid(TC);

                {
                    var details = client.GetDetailsAll();

                    foreach (var one in details)
                    {
                        TabItem Ti = new TabItem();
                        Ti.Header = $"Действует с - {(one.YM > 0 ? MyTools.YearMonth_From_YM(one.YM) : "Начала программы")}";
                        Ti.Content = one.GetEditor(SFE, WH, Column.DetailsClient);
                        TC.Items.Add(Ti);
                        CopyRow(Ti, one, new KeyValuePair<int, object>(C.DetailsClient.YM, DateControl_Class.SelectMonth));
                        DeleteRow(Ti, G.DetailsClient, one.ID);
                    }

                    AdderMenu(text, G.DetailsClient,
                        new KeyValuePair<int, object>(C.DetailsClient.Client, client.ID),
                        new KeyValuePair<int, object>(C.DetailsClient.YM, DateControl_Class.SelectMonth - 1));
                    TC.SelectedIndex = TC.Items.Count - 1;
                }
            }
        }

        #region объект

        private void DrawObject()
        {
            TabItem TI = new TabItem { Header = "Объекты" };
            //ThisTC.Items.Add(TI);
            TI.Add(ThisTC);

            Grid grid = new Grid { Background = Brushes.LightSteelBlue };

            TabControl TC = new TabControl { Margin = new Thickness(2) };

            TextBlock text = new TextBlock
            {
                Text = "Объекты",
                FontSize = 20
            };

            grid.SetRowFromGrid(MyTools.GL_Auto);
            grid.SetFromGrid(text);
            grid.SetRowFromGrid(new GridLength(1, GridUnitType.Star));
            grid.SetFromGrid(TC.VerticalHorisontalScroll_From_Control());
            TI.Content = grid;

            foreach (var objecte in client.Objects)
            {
                TabItem Ti = new TabItem { Header = $"Папка - {objecte.NumberFolder}" };

                WrapPanel wp = new WrapPanel { Orientation = Orientation.Vertical };
                Ti.Content = wp;
                TC.Items.Add(Ti);
                wp.Children.Add(objecte.GetEditor(SFE, WH, Column.Objecte));

                wp.Children.Add(DetailObj(objecte));
                wp.Children.Add(MidMonthVolume(objecte));
                wp.Children.Add(ObjFromRes(objecte));
                wp.Children.Add(WellObj(objecte));
            }

            AdderMenuWithDefaultKeys(text, G.Objecte
                , new KeyValuePair<int, object>[]
                { new KeyValuePair<int, object>(C.Objecte.NumberFolder, default(int)) }
                , new KeyValuePair<int, object>[]
                { new KeyValuePair<int, object>(C.Objecte.Client, clientID) }
                , new KeyValuePair<int, object>(C.Objecte.Client, client.ID),
                  new KeyValuePair<int, object>(C.Objecte.YMFrom, DateControl_Class.SelectMonth - 1));
        }

        /// <summary>Реквизиты объекта</summary>
        private WrapPanel DetailObj(Objecte obj)
        {
            TabControl TC = new TabControl();
            TC.Margin = new Thickness(2);
            WrapPanel result = new WrapPanel();
            result.Orientation = Orientation.Vertical;
            result.VerticalAlignment = VerticalAlignment.Top;
            result.Background = Brushes.LightSalmon;

            TextBlock text = new TextBlock();
            text.Text = "Реквизиты объекта";
            text.FontSize = 20;
            result.Children.Add(text);

            foreach (var one in obj.Details)
            {
                TabItem Ti = new TabItem();

                CopyRow(Ti, one, new KeyValuePair<int, object>(C.DetailsObject.YM, DateControl_Class.SelectMonth - 1));
                DeleteRow(Ti, G.DetailsObject, one.ID);
                Ti.Header = $"Действует с - {(one.YM > 0 ? MyTools.YearMonth_From_YM(one.YM) : "Начала программы")}";

                WrapPanel wp = new WrapPanel();
                wp.Orientation = Orientation.Vertical;
                wp.Children.Add(one.GetEditor(SFE, WH, Column.DetailsObject));

                if (obj.Separate)
                {
                    wp.Children.Add(one.GetEditor(SFE, WH,
                      new MyTools.C_DefColumn(C.DetailsObject.LegalAdresReference),
                      new MyTools.C_DefColumn(C.DetailsObject.MailAdresReference)));
                }

                Ti.Content = wp;
                TC.Items.Add(Ti);
            }

            TC.SelectedIndex = TC.Items.Count - 1;
            AdderMenu(text, G.DetailsObject,
                new KeyValuePair<int, object>(C.DetailsObject.Object, obj.ID),
                new KeyValuePair<int, object>(C.DetailsObject.YM, DateControl_Class.SelectMonth - 1));

            result.Children.Add(TC);

            return result;
        }

        private WrapPanel MidMonthVolume(Objecte obj)
        {
            WrapPanel result = new WrapPanel();
            result.Orientation = Orientation.Vertical;
            result.Background = Brushes.LightCyan;
            TextBlock text = new TextBlock();
            result.Children.Add(text);
            text.Text = "Среднемесячный объём";
            text.FontSize = 20;
            G.MidMonthVolume.QUERRY()
                .SHOW
                .WHERE
                .C(C.MidMonthVolume.Objecte, obj.ID)
                .DO();
            for (int i = 0; i < G.MidMonthVolume.Rows.Count; i++)
            {
                var mmv = new MidMonthVolume(G.MidMonthVolume.Rows.GetID(i), G.MidMonthVolume.Rows.Get<int>(i, C.MidMonthVolume.Year) >= DateTime.Now.Year - 1);
                result.Children.Add(mmv.GetEditor(SFE, WH, Column.MidMonthVolume));
            }
            AdderMenu(text, G.MidMonthVolume,
                new KeyValuePair<int, object>(C.MidMonthVolume.Objecte, obj.ID),
                new KeyValuePair<int, object>(C.MidMonthVolume.Year, DateControl_Class.SelectYear));
            return result;
        }

        private WrapPanel ObjFromRes(Objecte obj)
        {
            WrapPanel result = new WrapPanel();
            result.Orientation = Orientation.Vertical;
            result.Background = Brushes.LightYellow;
            TextBlock text = new TextBlock();
            result.Children.Add(text);
            text.Text = "Отношение к постановлениям";
            text.FontSize = 20;
            //if (obj.OFR == null)
            //{ obj.OFR = AdditionnTable.LoadOFRAtObjecte(obj.ID).ToList(); }
            foreach (var one in obj.OFR)
            { result.Children.Add(one.GetEditor(SFE, WH, Column.ObjectFromResolution)); }
            AdderMenu(text, G.ObjectFromResolution, new KeyValuePair<int, object>(C.ObjectFromResolution.Object, obj.ID));
            return result;
        }

        #region Колодец

        /// <summary>Колодцы</summary>
        private Grid WellObj(Objecte obj)
        {
            TabControl TC = new TabControl();
            TC.Margin = new Thickness(2);
            Grid result = new Grid();
            result.Background = Brushes.LightSkyBlue;
            TextBlock text = new TextBlock();
            text.Text = "Колодцы";
            text.FontSize = 20;
            AdderMenu(text, G.Well,
                    new KeyValuePair<int, object>(C.Well.Object, obj.ID),
                    new KeyValuePair<int, object>(C.Well.YMFrom, DateControl_Class.SelectMonth - 1));
            result.SetRowFromGrid();
            result.SetFromGrid(text);
            foreach (var one in obj.Wells)
            {
                TabItem Ti = new TabItem();
                CopyRow(Ti, one, new KeyValuePair<int, object>(C.Well.YMFrom, DateControl_Class.SelectMonth - 1));
                DeleteRow(Ti, G.Well, one.ID);
                Ti.Header = $"№ {one.Number}";
                WrapPanel wpwell = new WrapPanel();
                wpwell.Orientation = Orientation.Vertical;
                Ti.Content = wpwell;
                wpwell.Children.Add(one.GetEditor(SFE, WH, Column.Well));
                TC.Items.Add(Ti);
                wpwell.Children.Add(DeclairWell(one));
            }
            result.SetRowFromGrid();
            result.SetFromGrid(TC);
            return result;
        }

        /// <summary>Декларация</summary>
        private WrapPanel DeclairWell(Well well)
        {
            WrapPanel result = new WrapPanel();
            result.Orientation = Orientation.Vertical;
            result.Background = Brushes.LightSteelBlue;
            TextBlock text = new TextBlock();
            text.Text = "Декларации";
            text.FontSize = 20;
            result.Children.Add(text);
            TabControl TC = new TabControl();
            result.Children.Add(TC);
            TC.Margin = new Thickness(2);

            var declarations = Helpers.LogicHelper.DeclarationLogic.Find(well.ID);
            foreach (var one in declarations)
            {
                TabItem Ti = new TabItem();
                MenuItem addVal = new MenuItem { Header = "Добавить значение (загрязнение) декларации" };
                Grid grid = new Grid();
                CopyRowDeclair(Ti, one);
                Ti.ContextMenu.Items.Add(addVal);
                DeleteRow(Ti, G.Declaration, one.ID);
                addVal.Click += (sender, e) =>
                {
                    G.DeclarationValue.QUERRY()
                    .ADD
                    .C(C.DeclarationValue.Declaration, one.ID)
                    .DO();
                    DeclVal(grid, one);
                };
                Ti.Header = $"{one.Name} от {MyTools.YearMonth_From_YM(one.YM)}";
                DeclVal(grid, one);
                Ti.Content = grid;
                TC.Items.Add(Ti);
            }
            TC.SelectedIndex = TC.Items.Count - 1;
            AdderMenu(text, G.Declaration,
                new KeyValuePair<int, object>(C.Declaration.Well, well.ID),
                new KeyValuePair<int, object>(C.Declaration.YM, DateControl_Class.SelectMonth - 1));
            return result;
        }

        /// <summary>Значения декларации</summary>
        private void DeclVal(Grid grid, Declaration decl)
        {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();
            grid.SetRowFromGrid(MyTools.GL_Auto);
            var wh = new MyTools.C_MinMaxWidthHeight(HorAlig: HorizontalAlignment.Left, Wrap: true, MinMaxName: 80, MinWidth: 80);
            grid.SetFromGrid(decl.GetEditor(SFE, wh, Column.Declaration));
            decl = Helpers.LogicHelper.DeclarationLogic.FirstModel(decl.ID);
            MyTools.C_SettingFromRowEdit _sfe = new MyTools.C_SettingFromRowEdit(MyTools.EPosition.Horisontal, ShowName: false, ScopeColumn: true);
            foreach (var val in decl.DeclarationValues.OrderBy(x => Helpers.LogicHelper.PollutionLogic.FirstModel(x.PollutionID).Number).ToArray())
            {
                grid.SetRowFromGrid();
                List<MyTools.C_DefColumn> columns = new List<MyTools.C_DefColumn>();
                columns.Add(new MyTools.C_DefColumn(C.DeclarationValue.Pollution, wh, val.PollutionID > 0 ? false : true, font: new MyTools.S_FontControl(20, Color: val.PollutionID > 0 ? Brushes.LightCoral : Brushes.LightGreen, SymbolReplace: true)));
                int round = T.Pollution.Rows.Get<int>(val.PollutionID, C.Pollution.Round);
                if (T.Pollution.Rows.Get<bool>(val.PollutionID, C.Pollution.HasRange))
                { columns.Add(new MyTools.C_DefColumn(C.DeclarationValue.From, wh, true, round)); }
                columns.Add(new MyTools.C_DefColumn(C.DeclarationValue.To, wh, true, round));
                grid.SetFromGrid(val.GetEditor(_sfe, columns.ToArray()));
            }
        }

        #endregion

        #endregion

        int selectThisTC;

        private void AdderMenuWithDefaultKeys(TextBlock tb, DataBase.ISTable table
            , KeyValuePair<int, object>[] keys
            , KeyValuePair<int, object>[] updateKeys
            , params KeyValuePair<int, object>[] values)
        {
            MenuItem MI = new MenuItem();
            MI.Header = $"Добавить \"{table.Parent.AlterName}\"";
            var t = tb;
            MI.Click += (sender, e) =>
            {
                selectThisTC = ThisTC.SelectedIndex;
                try
                {
                    var tempTable = table.Parent.CreateSubTable(false);

                    if (keys.Any())
                    {
                        {
                            var query = tempTable.QUERRY().SHOW.WHERE;

                            foreach (var value in keys)
                            {
                                if (value.Key < 0)
                                { var tmp = query.ID((uint)value.Value).AND; }
                                else
                                { var tmp = query.C(value.Key, value.Value).AND; }
                            }

                            ((DataBase.IDo)query).DO();
                        }
                    }

                    if (tempTable.Rows.Count > 0)
                    {
                        foreach (var updateKey in updateKeys)
                        { tempTable.Rows.Set(0, updateKey.Key, updateKey.Value); }

                        table.Rows.Add(tempTable.Rows.GetID(0));
                    }
                    else
                    { MyTools.AddRowFromTable(table, values); }
                }
                catch
                {
                    MessageBox.Show("Вы пытаетесь добавить дубликат записи!");
                    return;
                }

                var sc = new MyTools.C_SelectControl(t);

                DrawClient();
                DrawObject();
                sc.ReSelect(t);

                ThisTC.SelectedIndex = selectThisTC;
            };

            tb.ContextMenu = new ContextMenu();
            tb.ContextMenu.Items.Add(MI);
        }
        private void AdderMenu(TextBlock tb, DataBase.ISTable Table, params KeyValuePair<int, object>[] Values)
        { AdderMenuWithDefaultKeys(tb, Table, new KeyValuePair<int, object>[0], new KeyValuePair<int, object>[0], Values); }

        private void DeleteRow(TabItem TI, DataBase.ISTable Table, uint ID)
        {
            if (TI.ContextMenu == null)
            { TI.ContextMenu = new ContextMenu(); }
            MenuItem Delete = new MenuItem();
            TI.ContextMenu.Items.Add(Delete);
            Delete.Header = "Удалить запись";
            Delete.Click += (sender, e) =>
              {
                  selectThisTC = ThisTC.SelectedIndex;
                  if (MyTools.DeleteRowsByID(Table, true, ID))
                  {
                      DrawClient();
                      DrawObject();
                      ThisTC.SelectedIndex = selectThisTC;
                  }
              };
        }

        private void CopyRow(TabItem TI, MyTools.C_A_BaseFromAllDB row, params KeyValuePair<int, object>[] values)
        {
            if (TI.ContextMenu == null)
            { TI.ContextMenu = new ContextMenu(); }
            MenuItem Copy = new MenuItem();
            TI.ContextMenu.Items.Add(Copy);
            Copy.Header = "Копировать";
            Copy.Click += (sender, e) =>
            {
                selectThisTC = ThisTC.SelectedIndex;
                try
                { MyTools.CopyElements(row, values); }
                catch
                {
                    MessageBox.Show("Не удалось скопировать запись!");
                    return;
                }
                DrawClient();
                DrawObject();
                ThisTC.SelectedIndex = selectThisTC;
            };
        }

        private void CopyRowDeclair(TabItem TI, Declaration row)
        {
            if (TI.ContextMenu == null)
            { TI.ContextMenu = new ContextMenu(); }
            MenuItem Copy = new MenuItem();
            TI.ContextMenu.Items.Add(Copy);
            Copy.Header = "Копировать декларацию";
            Copy.Click += (sender, e) =>
            {
                selectThisTC = ThisTC.SelectedIndex;
                try
                {
                    uint NewIDDec = MyTools.CopyElements(row, new KeyValuePair<int, object>(C.Declaration.YM, DateControl_Class.SelectMonth));
                    foreach (var one in row.DeclarationValues)
                    { MyTools.CopyElements(one, new KeyValuePair<int, object>(C.DeclarationValue.Declaration, NewIDDec)); }
                }
                catch
                {
                    MessageBox.Show("Не удалось скопировать запись!");
                    return;
                }
                DrawClient();
                DrawObject();
                ThisTC.SelectedIndex = selectThisTC;
            };
        }
    }
}