using MAC_2.Employee.Mechanisms;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using AutoTable;
using System.Windows.Media;
using MAC_2.PortingBase;

namespace MAC_2.EmployeeWindow.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
            InstructionsMessage_Class.LoadInstructions(ThisMenu, data.ETypeInstruction.Admin);

            SubTables = new DataBase.ISTable[data.T1.Tables.Count];

            for (int i = 0; i < SubTables.Length; i++)
            {
                var Table = data.T1.Tables[i];

                if (Table.GetSubTable.Count == 0)
                { SubTables[i] = Table.CreateSubTable(); }
                else
                { SubTables[i] = Table.GetSubTable[0]; }
            }
            SubTables = SubTables.OrderBy(x => x.Parent.DataSource.Type).ThenBy(x => x.Name).ToArray();
        }
        void GetAutoForm(DataBase.ISTable GetValue)
        {
            if ((bool)LoadNew_check.IsChecked)
            { GetValue.QUERRY(DataBase.State.None).SHOW.DO(); }

            GetValue.GetAutoForm(AutoForm.ShowType.Admin).ShowDialog();
        }
        DataBase.ISTable[] SubTables;
        private void AdminPanel_Load(object sender, EventArgs e)
        {
            data.PrgSettings.Forms[(int)data.Forms.AdminPanel].Set(this);

            for (int i = 0; i < SubTables.Length; i++)
            { AnyTable_combo.Items.Add("(" + SubTables[i].Parent.DataSource.Type.ToString() + ")" + SubTables[i].Name); }
        }
        private void UsersEditor_button_Click(object sender, RoutedEventArgs e)
        { GetAutoForm(G.User); }
        private void Settings_button_Click(object sender, EventArgs e)
        { (new Settings_Form()).ShowDialog(); }

        private void AdminPanel_WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        { data.PrgSettings.Forms[(int)data.Forms.AdminPanel].Set(this); }
        private void DownloadSpeed_button_Click(object sender, EventArgs e)
        {
            long Summ = 0;
            var SB = new StringBuilder();

            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                SB.Append(data.T1.Tables[i].Name + ", Initial=" + ((double)((DataBase.table)data.T1.Tables[i]).InitilalTime / 10000000).ToString() + ", ShowRows=" + ((double)((DataBase.table)data.T1.Tables[i]).ShowRowsTime / 10000000).ToString() + ", Download=" + ((double)((DataBase.table)data.T1.Tables[i]).DownloadTime / 10000000).ToString() + "\n");
                Summ += ((DataBase.table)data.T1.Tables[i]).InitilalTime;
            }

            SB.Append("Summ Initial=" + ((double)Summ / 10000000).ToString() + ", ").Append("Middle Initial=" + ((double)Summ / data.T1.Tables.Count / 10000000).ToString() + ", ");

            System.Windows.MessageBox.Show(SB.ToString());
        }
        private void AnyTableEdit_button_Click(object sender, EventArgs e)
        {
            if (AnyTable_combo.SelectedIndex > -1)
            {
                var SubTable = SubTables[AnyTable_combo.SelectedIndex];
                if ((bool)LoadAll_check.IsChecked)
                {
                    if ((bool)DeletedToo_check.IsChecked)
                    { SubTable.QUERRY(DataBase.State.None).SHOW.DO(); }
                    else
                    { SubTable.QUERRY(DataBase.State.Normal).SHOW.DO(); }
                }

                SubTable.Parent.Rows.CanUseEvents = false;
                SubTable.GetAutoForm(AutoForm.ShowType.Admin).ShowDialog();
                SubTable = null;
            }
        }
        private void ExportToFiles_button_Click(object sender, EventArgs e)
        {
            var SFD = new SaveFileDialog();

            SFD.InitialDirectory = DataBase.table.SubTable.SavePath;
            SFD.FileName = "Таблицы.xls";
            SFD.Title = String.Concat("Выгрузка данных из таблиц");

            if (SFD.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            { return; }

            var path = SFD.FileName.Substring(0, SFD.FileName.Length - System.IO.Path.GetFileName(SFD.FileName).Length);

            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                if (data.T1.Tables[i].RemoteType != DataBase.RemoteType.Local)
                {
                    var SubTable = data.T1.Tables[i].CreateSubTable(false);
                    SubTable.QUERRY(DataBase.State.None).SHOW.DO();

                    SaveTable_Form.SaveXLS(SubTable, path + SubTable.Parent.Name + ".xls", 1, SubTable.Rows.Count, true);
                }
            }
        }
        struct LoadTable
        {
            public LoadTable(DataBase.ISTable Table, string FileName)
            {
                this.Table = Table;
                this.FileName = FileName;
            }
            public readonly DataBase.ISTable Table;
            public readonly string FileName;
        }
        private void LoadFromFiles_button_Click(object sender, EventArgs e)
        {
            var OFD = new OpenFileDialog();

            OFD.InitialDirectory = DataBase.table.SubTable.SavePath;
            OFD.Title = String.Concat("Загрузка данных из таблиц");

            if (OFD.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            { return; }

            var path = OFD.FileName.Substring(0, OFD.FileName.Length - System.IO.Path.GetFileName(OFD.FileName).Length);

            var Tables = new LoadTable[data.T1.Tables.Count];

            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                var SubTable = data.T1.Tables[i].CreateSubTable(false);

                if (SubTable.Parent.RemoteType != DataBase.RemoteType.Local)
                {
                    if (File.Exists(path + SubTable.Parent.Name + ".xls"))
                    { Tables[i] = new LoadTable(SubTable, path + SubTable.Parent.Name + ".xls"); }
                    else if (File.Exists(path + SubTable.Parent.Name + ".xlsx"))
                    { Tables[i] = new LoadTable(SubTable, path + SubTable.Parent.Name + ".xlsx"); }
                    else
                    {
                        System.Windows.MessageBox.Show("Файл " + path + SubTable.Parent.Name + ".xls/.xlsx не существует");
                        return;
                    }
                }
            }
            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                if (Tables[i].Table != null)
                {
                    try { Tables[i].Table.Rows.LoadFromFile(Tables[i].FileName, false, true); }
                    catch (Exception) { System.Windows.MessageBox.Show("Ошибка чтения файла (проверьте не открыт ли файл)"); }
                }
            }
        }
        private void Edit_VNode_button_Click(object sender, EventArgs e)
        { new Employee_Window().ShowDialog(); }
        private void UsersBock_button_Click(object sender, RoutedEventArgs e)
        { new UsersBlock_Form().ShowDialog(); }
        private void ExportToFiles_button_Click(object sender, RoutedEventArgs e)
        {
            var SFD = new SaveFileDialog();

            SFD.InitialDirectory = DataBase.table.SubTable.SavePath;
            SFD.FileName = "Таблицы.xls";
            SFD.Title = String.Concat("Выгрузка данных из таблиц");

            if (SFD.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            { return; }

            var path = SFD.FileName.Substring(0, SFD.FileName.Length - Path.GetFileName(SFD.FileName).Length);

            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                if (data.T1.Tables[i].RemoteType != DataBase.RemoteType.Local)
                {
                    var SubTable = data.T1.Tables[i].CreateSubTable(false);
                    SubTable.QUERRY(DataBase.State.None).SHOW.DO();
                    SaveTable_Form.SaveXLS(SubTable, path + SubTable.Parent.Name + ".xls", 1, SubTable.Rows.Count, true);
                }
            }
        }
        private void LoadFromFiles_button_Click(object sender, RoutedEventArgs e)
        {
            var OFD = new OpenFileDialog();

            OFD.InitialDirectory = DataBase.table.SubTable.SavePath;
            OFD.Title = String.Concat("Загрузка данных из таблиц");

            if (OFD.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            { return; }

            var path = OFD.FileName.Substring(0, OFD.FileName.Length - Path.GetFileName(OFD.FileName).Length);

            var Tables = new LoadTable[data.T1.Tables.Count];

            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                var SubTable = data.T1.Tables[i].CreateSubTable(false);

                if (SubTable.Parent.RemoteType != DataBase.RemoteType.Local)
                {
                    if (File.Exists(path + SubTable.Parent.Name + ".xls"))
                    { Tables[i] = new LoadTable(SubTable, path + SubTable.Parent.Name + ".xls"); }
                    else if (File.Exists(path + SubTable.Parent.Name + ".xlsx"))
                    { Tables[i] = new LoadTable(SubTable, path + SubTable.Parent.Name + ".xlsx"); }
                    else
                    {
                        System.Windows.MessageBox.Show("Файл " + path + SubTable.Parent.Name + ".xls/.xlsx не существует");
                        return;
                    }
                }
            }
            for (int i = 0; i < data.T1.Tables.Count; i++)
            {
                if (Tables[i].Table != null)
                { Tables[i].Table.Rows.LoadFromFile(Tables[i].FileName, false, true); }
            }
        }
        private void Edit_VNode_button_Click(object sender, RoutedEventArgs e)
        { new Employee_Window().ShowDialog(); }

        private void Test_Click(object sender, RoutedEventArgs e)
        { new ColorSelector_Window().ShowDialog(); }

        private void PortingBase_Click(object sender, RoutedEventArgs e)
        { new Porting_Window().ShowDialog(); }
    }
}