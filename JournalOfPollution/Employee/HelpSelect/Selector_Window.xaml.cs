using MAC_2.Employee.Mechanisms;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoTable;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading;
using MAC_2.Model;
using MAC_2.Employee.HelpSelect.Selector;

namespace MAC_2.Employee.HelpSelect
{
    /// <summary>
    /// Логика взаимодействия для Selector_Window.xaml
    /// </summary>
    public partial class Selector_Window : Window
    {
        public Selector_Window()
        {
            InitializeComponent();
            this.GetSetting();
            SS = new Selector_Class(DG_Show, WP, ShowSelect, Text);
            LoadThisMenu();
        }
        private void LoadThisMenu()
        {
            if (data.User<uint>(C.User.UType) != (uint)data.UType.MainWork)
            { Units.IsEnabled = Add.IsEnabled = (data.User<uint>(C.User.UType) == (uint)data.UType.Admin); }
        }
        Selector_Class SS;

        private void Units_Click(object sender, RoutedEventArgs e)
        {
            if (Units.Items.Count == 0)
            {                
                var units = Helpers.LogicHelper.UnitLogic.Find();

                foreach (var unit in units)
                {
                    CheckBox CB = new CheckBox();
                    CB.Content = unit.Name;
                    if (unit.Name.ToLower().Contains("поск-2"))
                    {
                        Selector_Class.ListUnit.Add(unit.ID);
                        CB.IsChecked = true;
                    }
                    Units.Items.Add(CB);
                    CB.Click += (sendere, ee) =>
                      {
                          if ((bool)CB.IsChecked)
                          { Selector_Class.ListUnit.Add(unit.ID); }
                          else
                          { Selector_Class.ListUnit.Remove(unit.ID); }
                      };
                }
            }
        }
        private void Units_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            //if (SS.Values.Any())
            //{ DG_Show.ItemsSource = SS.Values; }
            //Text.Text = $"План: {SS.Values.Count(x => (x as Selector.IThisShow).NegotiationAssistandID > 0)} В обработке:{SS.Values.Count}";
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Add.Content.ToString() == "Редактировать")
            {
                Add.Content = ">Редактирование<";
                SS.LoadEditor();
                Units_Click(null, null);
            }
            else
            {
                Add.Content = "Редактировать";
                SS.LoadSelects();
            }
            //Add.SleepControl(1000);
        }

        private void FolderAct_Click(object sender, RoutedEventArgs e)
        { MyTools.OpenFolder(Directory.GetCurrentDirectory().ToString() + "\\Документы\\Акты", true); }
    }
}