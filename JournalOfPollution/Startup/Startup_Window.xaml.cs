using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using MAC_2.EmployeeWindow;

namespace MAC_2
{
    public partial class Startup_Window : Window
    {
        public Startup_Window()
        {
            InitializeComponent();

            this.CanUse_timer = new System.Windows.Forms.Timer();
            this.CanUse_timer.Enabled = false;
            this.CanUse_timer.Tick += CanUse_timer_Tick;
            this.CanUse_timer.Interval = 10 * 1000;

            this.Timer_timer = new System.Windows.Forms.Timer();
            this.Timer_timer.Enabled = false;
            this.Timer_timer.Tick += Timer_timer_Tick;

            Try_Again:;
            var Return = string.Empty;

            data.T1 = new DataBase(data.PrgSettings.Values[(int)data.Strings.DATABASE].String, Encoding.GetEncoding(1251));
            data.T1.AllowModify = data.AllowModify;

            data.T1.ErrorConnection = (bd, ex) =>
            {
                if (MessageBox.Show("При попытке подключения к источнику данных возникла ошибка:\n" + ex.Message + "\nПопробовать подключиться сново?\nВ противном случае программа закроется", "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                { return true; }
                else
                {
                    var S = System.Diagnostics.Process.GetProcessesByName(System.AppDomain.CurrentDomain.FriendlyName);

                    for (int i = 0; i < S.Length; i++)
                    { S[i].Kill(); }

                    return false;
                }
            };

            switch (data.DataSourceType)
            {
                case DataBase.RemoteType.MFT:
                    try
                    {
                        if (!data.T1.UseMFT(System.AppDomain.CurrentDomain.FriendlyName))
                        { Return += "База данных не обнаружена, либо данные повреждены\n"; }
                    }
                    catch (Exception ex)
                    { Return += ex.Message.ToString() + "\n"; }
                    break;
                case DataBase.RemoteType.MySQL:
                    Return = string.Empty;
                    for (int i = (int)data.Strings.SqlIp; i < (int)data.Strings.SqlIpLast + 1; i++)  //проверяю ip-шники
                    {
                        try
                        {
                            if (data.T1.UseMySql(data.PrgSettings.Values[i].String
                                               , data.PrgSettings.Values[(int)data.Strings.SqlLogin].String
                                               , data.PrgSettings.Values[(int)data.Strings.SqlPassword].String
                                               , data.PrgSettings.Values[(int)data.Strings.SqlPort].Int))
                            {
                                if (i != (int)data.Strings.SqlIp)
                                { data.PrgSettings.Values[(int)data.Strings.SqlIp].String = data.PrgSettings.Values[i].String; }

                                Return = string.Empty;
                                break;
                            }
                            else
                            { Return += i + ")Не верный логин/пароль, либо сервер не отвечает\n"; }
                        }
                        catch (Exception ex)
                        { Return += i + ")" + ex.Message.ToString() + "\n"; }
                    }

                    if (Return.Length > 0)
                    { Return = "Варианты mysql адресов:\n" + Return; }
                    break;
                case DataBase.RemoteType.Local: break;
                default:
                    throw new Exception("не известный тип источника данных " + data.PrgSettings.Values[(int)data.Strings.UseSQL].String);
            }

            if (Return.Length > 0)
            {
                switch (MessageBox.Show("При подключении к источнику данных возникла ошибка:\n" + Return + "Попробовать подключиться еще раз ?\nВ случае отмены будет запущен оффлайновый режим.", "Ошибка", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation))
                {
                    case MessageBoxResult.Cancel:
                        {
                            data.PrgSettings.Values[(int)data.Strings.UseSQL].Int = (int)DataBase.RemoteType.Local;
                            data.T1.UseLocal();
                            break;
                        }
                    case MessageBoxResult.No:
                        {
                            this.Close();
                            return;
                        }
                    case MessageBoxResult.OK:
                        { goto Try_Again; }
                }
            }
           
            if (!(bool)new StartupLogo_Window(Misc.DataBaseLoadFT).ShowDialog())
            {this.Close(); }            
        }
        
        Window Show;
        System.Windows.Forms.Timer CanUse_timer;
        System.Windows.Forms.Timer Timer_timer;
        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if (UserNames_combo.SelectedIndex < 0)  //если пользователь не выбран
            {
                System.Windows.MessageBox.Show(this
                    , "Необходимо выбрать пользователя"
                    , "Внимание"
                    , MessageBoxButton.OK
                    , MessageBoxImage.Exclamation);
                UserNames_combo.IsDropDownOpen = true;
                UserNames_combo.Focus();
                return;
            }

            var UserID = G.User.Rows.GetID(UserNames_combo.SelectedIndex);  //записываю ID текущего пользователя

            if (data.T1.type == DataBase.RemoteType.MySQL && !T.User.Rows.Get<bool>(UserID, C.User.Enabled))
            {
                var Cause = T.User.Rows.Get<string>(UserID, C.User.Cause);
                if (Cause.Length > 0)
                {
                    System.Windows.MessageBox.Show(this
                        , "Эта учетная запись заблокирована администратором, по причине:\r\n" + Cause + ".\r\n Её использование сейчас невозможно.", "Внимание"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Exclamation);
                }
                else
                {
                    System.Windows.MessageBox.Show(this
                        , "Эта учетная запись заблокирована администратором. Её использование сейчас невозможно."
                        , "Внимание"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Exclamation);
                }
                return;
            }

            //пароль=================================================
            if (UserPass_Box.Password != "пуыефде" && Environment.UserName != "Asup10")    //секретный пароль
            //пароль=================================================
            {
                if (T.User.Rows.Get<string>(UserID, C.User.Pass) != UserPass_Box.Password)   //стандартный пароль
                {
                    System.Windows.MessageBox.Show(this
                        , "Пароль неверный!\nЕсли вы забыли ваш пароль, то попробуйте сменить его. "
                                        + "Для этого нажмите левой кнопкой мышки на слова:\n\"Проблема с учетной записью ?\"\nПод полем ввода пароля"
                                        , "Внимание"
                                        , MessageBoxButton.OK
                                        , MessageBoxImage.Exclamation);
                    UserPass_Box.Password = string.Empty;
                    UserPass_Box.Focus();
                    return;
                }

                if (data.T1.type == DataBase.RemoteType.MySQL)
                {
                    if (T.User.Rows.Get<bool>(UserID, C.User.IsHere))   //проверяю залогинился пользователь до этого или нет
                    {
                        System.Windows.MessageBox.Show(this
                            , "Эта учетная запись сейчас используется.\nЕсли работа программы была завершена не корректно в прошлый раз, то запись можно сбросить. "
                                            + "Для этого нажмите левой кнопкой мышки на слова:\n\"Проблема с учетной записью ?\"\nПод полем ввода пароля"
                                            , "Внимание"
                                            , MessageBoxButton.OK
                                            , MessageBoxImage.Exclamation);
                        UserPass_Box.Focus();
                        return;
                    }
                    T.User.Rows.Set(UserID, C.User.IsHere, true);
                }

                T.User.Rows.Set(UserID, C.User.PCUser, Environment.UserName);
                T.User.Rows.Set(UserID, C.User.PCName, Environment.MachineName);
                T.User.Rows.Set(UserID, C.User.PrgVer, Assembly.GetExecutingAssembly().GetName().Version);
            }            

            this.Visibility = System.Windows.Visibility.Hidden;
            if ((bool)new StartupLogo_Window(Misc.DataBaseLoad).ShowDialog())
            {
                data.UserID = UserID;  //записываю id текущего пользователя

                Show = Misc.SelectForm();

                if (Show != null)
                {                    
                    if (Show.IsInitialized)//PresentationSource.FromVisual(Show) != null && !PresentationSource.FromVisual(Show).IsDisposed)
                    {
                        sText = Show.Title = data.User<string>(C.User.Login);
                        CanUse_timer.Enabled = data.T1.type == DataBase.RemoteType.MySQL;

                        //Проверяю наличие изменений
                        if (data.PrgSettings.Values[(int)data.Strings.Changes].Int != Misc.Number && CheckDocChanges())
                        {
                            System.Windows.MessageBox.Show(this
                                , "Открылся документ со списком изменений в новой версии программы, пожалуйста ознакомтесь с изменениями прежде чем начать работу."
                                , "Внимание"
                                , MessageBoxButton.OK
                                , MessageBoxImage.Exclamation);
                            data.PrgSettings.Values[(int)data.Strings.Changes].Int = Misc.Number;
                        }
                    }

                    Show.ShowDialog();              
                } 
            }
            this.Close();
        }

        public static bool CheckDocChanges()
        {
            var FilePath = System.Windows.Forms.Application.StartupPath + "\\Новое в версии\\";

            if (Directory.Exists(FilePath))
            {
                string[] Files;

                if ((Files = Directory.GetFiles(FilePath, Misc.Number.ToString() + ".*")).Length == 1)
                {
                    System.Diagnostics.Process.Start(Files[0]);
                    return true;
                }
            }

            return false;
        }


        private void UserPass_Box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)    //если ткнул ентер, то
            {
                Start_Button.Focus();

                Start_Button_Click(null, null);    //делаю вид, что нажали "Вход"

                e.Handled = true;
            }
        }

        private void UserNames_combo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)    //если ткнул ентер, то
            {
                UserPass_Box.Focus();    //делаю вид, что нажали "TAB"
                e.Handled = true;
            }
        }

        private void UserNames_combo_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            UserPass_Box.Password = string.Empty; //если сменил логин, то старый пароль стерается

            if (UserNames_combo.SelectedIndex > -1)
            { Mail_Label.Visibility = System.Windows.Visibility.Visible; }
            else
            { Mail_Label.Visibility = System.Windows.Visibility.Hidden; }
        }

        private void Mail_Label_Click(object sender, MouseButtonEventArgs e)
        {
            if (UserNames_combo.SelectedIndex > -1)
            { new Mail_Window(G.User.Rows.GetID(UserNames_combo.SelectedIndex)).ShowDialog(); }
        }

        private void Startup_Form_FormClosed(object sender, EventArgs e)
        {
            if (data.UserID > 0)
            {
                if (data.T1.type == DataBase.RemoteType.MySQL)
                {
                    //если пользователь залогинился, то разлогиванию его <_^
                    while (data.User<bool>(C.User.IsHere))
                    { data.User<bool>(C.User.IsHere, false); }
                }

                data.PrgSettings.Save(); //сохраняю настройки и аривидерчи
                Show.Close();
            }                        
        }

        private void Version_Strip_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show(this
                , System.Windows.Forms.Application.ProductName + "=" + Assembly.GetExecutingAssembly().GetName().Version + "\n" + DataBase.GetVersion()
                , "Версия"
                , MessageBoxButton.OK
                ,  MessageBoxImage.Information);
        }


        /*
        * тута вот механизм "выгоняния" пользователя из программы.
        * Принцип:
        * 1.Проверяю метку "выгоняния"(значение колонки C.User.Enabled текущего пользователя), если она не активирована, то вывожу сообщение. После того как сообщение выведено, пойдет отсчет - 1 минута
        * 2.По истечении минуты, если метка все еще установлена на закрытие, программа закрывается.
        */
        /// <summary>Указание на то, такой статус таймера CanUse_timer</summary>
        bool OneMinute = false;
        /// <summary>Длина периода таймера CanUse_timer в режиме сканирования значение C.User.Enabled</summary>
        const int StandartIndervalCanUse = 10 * 1000;
        /// <summary>Длина периода таймера CanUse_timer в режиме ожидания закрытия</summary>
        const int CloseIndervalCanUse = MaxCount * 1000;

        /// <summary>Кол-во секунд таймера</summary>
        const byte MaxCount = 60;
        /// <summary>Сохраненный текст заголовка окна интерфейса</summary>
        string sText = null;
        /// <summary>Текущее кол-во секунд таймера</summary>
        byte Count = 0;

        private void CanUse_timer_Tick(object sender, EventArgs e)
        {
            CanUse_timer.Enabled = false;

            if (OneMinute)
            {
                if (T.User.Rows.Get<bool>(data.UserID, C.User.Enabled))
                {
                    new Thread(() =>
                    {
                        
                        System.Windows.MessageBox.Show("Использование этой учетной записи вновь разрешено. Работу можно продолжить."
                            , "Внимание"
                            , MessageBoxButton.OK
                            , MessageBoxImage.Exclamation);
                        
                    }).Start();

                    Timer_timer.Enabled = false;

                    Show.Title = sText;
                    OneMinute = false;
                    CanUse_timer.Interval = StandartIndervalCanUse;
                    CanUse_timer.Enabled = true;
                }
                else
                {
                    Timer_timer.Enabled = false;
                    this.Close();
                    System.Windows.Forms.Application.ExitThread();
                }
            }
            else if (!T.User.Rows.Get<bool>(data.UserID, C.User.Enabled))
            {
                Count = MaxCount - 1;
                OneMinute = true;
                CanUse_timer.Interval = CloseIndervalCanUse;
                var Cause = T.User.Rows.Get<string>(data.UserID, C.User.Cause);

                if (Cause.Length == 0)
                { Cause = " не указана."; }
                else
                { Cause = $":\r\n\"{Cause}\".\r\n"; }

                Timer_timer.Interval = 10 * 100;
                Timer_timer.Enabled = true;

                new Thread(() =>
                {                    
                    System.Windows.MessageBox.Show("Использование этой учетной записи запрещено, причина" + Cause + " Пожалуйста закончите вашу работу и выйдите из программы, в противном случае программа завершит свою работу через одну минуту."
                        , "Внимание"
                        , MessageBoxButton.OK
                        , MessageBoxImage.Exclamation);
                }).Start();
            }
            T.User.Rows.Set<bool>(data.UserID, C.User.IsHere, true);
            CanUse_timer.Enabled = true;
        }

        private void Timer_timer_Tick(object sender, EventArgs e)
        {
            if (Count > 15)
            { Show.Title = sText + ". Осталось " + Count.ToString() + " сек."; }
            else
            { Show.Title = sText + ". Внимание! Осталось " + Count.ToString() + " сек."; }
            Count--;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            G.User.QUERRY().SHOW.WHERE.NOT.AC(C.User.UType).EQUI.BV((uint)0).DO();
            if (G.User.Rows.Count == 0)
            {
                G.User.QUERRY()
                    .ADD
                        .C(C.User.Login, "Администратор")
                        .C(C.User.UType, (uint)data.UType.Admin)
                        .C(C.User.IsHere, DataBase.AutoStatus.UnUse)
                        .C(C.User.Enabled, true)
                    .DO();
            }
            int SelectingUser = -1;
            var UserID = data.PrgSettings.Values[(int)data.Strings.LastUser].UInt;

            for (int i = 0; i < G.User.Rows.Count; i++)
            {
                if (G.User.Rows.Get_Row(i).ID == UserID)
                { SelectingUser = i; }

                UserNames_combo.Items.Add(G.User.Rows.Get<string>(i, C.User.Login) + "(" + G.User.Rows.Get<string>(i, C.User.UType) + ")");
            }
            UserNames_combo.SelectedIndex = SelectingUser;
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}