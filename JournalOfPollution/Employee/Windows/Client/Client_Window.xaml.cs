using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoTable;
using MAC_2.Employee.Mechanisms;

namespace MAC_2.Employee.Windows
{
    /// <summary>
    /// Логика взаимодействия для Client_Window.xaml
    /// </summary>
    public partial class Client_Window : Window
    {
        public Client_Window(uint clientID)
        {
            InitializeComponent();
            this.GetSetting();
            CC = new Client_Class(TabControlView, clientID);
            InstructionsMessage_Class.LoadInstructions(ThisMenu, data.ETypeInstruction.EditorClient);
        }

        Client_Class CC;
    }
}
