using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace MAC_2.Controls
{
    /// <summary>
    /// Логика взаимодействия для Choose_Page.xaml
    /// </summary>
    public partial class Choose_Page
    {
        public Choose_Page(ETypeChose TypeChoose, int Value, bool ReadOnly = false, int MinVal = int.MinValue, int MaxVal = int.MaxValue, bool OnlyOne = true)
        {
            InitializeComponent();
            this.Value = Value;
            Min = MinVal;
            Max = MaxVal;

            TB = new TextBox();
            TB.TextChanged += (sender, e) =>
            {
                DataBase.NoABC_Int_Dinamic(TB);
                if (Value != Convert.ToInt32(TB.Text))
                {
                    SetVal(Convert.ToInt32(TB.Text), true);
                }
            };
            if (ReadOnly)
                TB.IsEnabled = false;
            BTld = new Button();
            BTru = new Button();
            BTld.MaxHeight = BTru.MaxHeight = BTld.MaxWidth = BTru.MaxWidth = 30;
            if (OnlyOne)
                BTld.ToolTip = BTru.ToolTip = "При зажатии LAlt x5\nПри зажатии LShift x10\nПри зажитии LCtrl x100";
            ((IAddChild)TB).AddText(Value.ToString());
            PageG.Children.Add(TB);
            BTld.Click += (sender, e) =>
            {
                LastVal = ELastVal.Minus;
                if (!OnlyOne)
                {
                    if (Keyboard.IsKeyDown(Key.LeftAlt))
                        SetVal(-5);
                    else if (Keyboard.IsKeyDown(Key.LeftShift))
                        SetVal(-10);
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        SetVal(-100);
                    else
                        SetVal(-1);
                }
                else
                    SetVal(-1);
            };
            BTru.Click += (sender, e) =>
            {
                LastVal = ELastVal.Plus;
                if (!OnlyOne)
                {
                    if (Keyboard.IsKeyDown(Key.LeftAlt))
                        SetVal(5);
                    else if (Keyboard.IsKeyDown(Key.LeftShift))
                        SetVal(10);
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        SetVal(100);
                    else
                        SetVal(1);
                }
                else
                    SetVal(1);
            };
            PageG.Children.Add(BTld);
            PageG.Children.Add(BTru);

            switch (TypeChoose)
            {
                case ETypeChose.Horisontal:
                    for (int i = 0; i < 3; i++)
                    {
                        ColumnDefinition CD = new ColumnDefinition();
                        CD.Width = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto);
                        PageG.ColumnDefinitions.Add(CD);
                    }
                    PageG.ColumnDefinitions[1].Width = new System.Windows.GridLength(5, System.Windows.GridUnitType.Star);
                    Grid.SetColumn(TB, 1);
                    Grid.SetRow(TB, 0);

                    BTld.Content = "<";
                    Grid.SetColumn(BTld, 0);
                    Grid.SetRow(BTld, 0);
                    BTru.Content = ">";
                    Grid.SetColumn(BTru, 2);
                    Grid.SetRow(BTru, 0);
                    break;
                case ETypeChose.Vertikal:
                    for (int i = 0; i < 3; i++)
                    {
                        RowDefinition RD = new RowDefinition();
                        RD.Height = new System.Windows.GridLength(0, System.Windows.GridUnitType.Auto);
                        PageG.RowDefinitions.Add(RD);
                    }
                    PageG.RowDefinitions[1].Height = new System.Windows.GridLength(5, System.Windows.GridUnitType.Star);
                    Grid.SetColumn(TB, 0);
                    Grid.SetRow(TB, 1);

                    BTld.Content = "\\/";
                    Grid.SetColumn(BTld, 0);
                    Grid.SetRow(BTld, 2);
                    BTru.Content = "/\\";
                    Grid.SetColumn(BTru, 0);
                    Grid.SetRow(BTru, 0);
                    break;
            }
        }
        int Value;
        int Min, Max;
        Button BTru, BTld;
        public int GetValue { get { return Value; } }
        public TextBox TB;
        public ELastVal LastVal;
        public enum ELastVal { Plus, Minus }
        public enum ETypeChose { Horisontal, Vertikal }
        /// <summary>Прибавляем или кладём значение</summary>
        /// <param name="IncOrVal">Инкремент или значение</param>
        /// <param name="Val">Если true значение кладём</param>
        private void SetVal(int IncOrVal, bool Val = false)
        {
            if (!Val)
                Value += IncOrVal;
            else
                Value = IncOrVal;
            if (Value > Max)
            {
                Value = Max;
                BTru.IsEnabled = false;
            }
            else
                BTru.IsEnabled = true;
            if (Value < Min)
            {
                Value = Min;
                BTld.IsEnabled = false;
            }
            else
                BTld.IsEnabled = true;
            TB.Text = Value.ToString();
        }
    }
}