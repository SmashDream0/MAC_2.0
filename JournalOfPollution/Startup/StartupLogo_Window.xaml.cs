using AutoTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MAC_2
{
    public partial class StartupLogo_Window : Window
    {
        Loading_class Loading;
        byte PCount = 0;

        public class Loading_class
        {
            public Loading_class(Loading_delegate action)
            {
                thread = new Thread(() => { action(this); });
                thread.Start();
            }
            public delegate void Loading_delegate(Loading_class LC);

            public string LoadingComment;

            Thread thread;

            public bool Ready { get { return !thread.IsAlive; } }

            public void Abort() { thread.Abort(); }

            public override string ToString()
            {
                if (!Ready)
                { return "Ready=" + Ready.ToString() + ": " + LoadingComment; }
                else
                { return "Ready=" + Ready.ToString(); }
            }
        }

        public StartupLogo_Window(Loading_class.Loading_delegate action)
        {
            InitializeComponent();

            timer = new System.Windows.Forms.Timer();

            Loading = new Loading_class(action);

            
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            timer.Enabled = true;

            this.CenterPosition();
        }
        
        System.Windows.Forms.Timer timer;
        const byte MaxCount = 3;
        private void timer_Tick(object sender, EventArgs e)
        {
            if (PCount == MaxCount)
            { PCount = 1; }
            else
            { PCount++; }

            LableL.Text = "Загрузка:" + Loading.LoadingComment + "...".Substring(PCount);    //отображаю плацебо-загрузку

            if (Loading.Ready)
            {
                this.Drop -= StartupLogo_Form_FormClosing;
                timer.Enabled = false;

                if (data.T1.type== DataBase.RemoteType.Local || data.T1.DataSourceEnabled)
                { DialogResult = true; }
                else
                { DialogResult = false; }
            }
        }
        private void StartupLogo_Form_FormClosing(object sender, System.Windows.DragEventArgs e)
        {
            timer.Enabled = false;
            Loading.Abort();
            //DialogResult = false;
        }

    }
}
