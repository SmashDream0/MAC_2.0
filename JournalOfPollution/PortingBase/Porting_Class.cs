using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS;
using NPOI.SS.UserModel;
using System.Windows.Forms;
using System.Windows.Controls;
using AutoTable;
using System.Windows.Data;

namespace MAC_2.PortingBase
{
    public class BasePorting_Class
    {
        public BasePorting_Class(Grid grid)
        {
            this.grid = grid;
            var OFD = new MyTools.C_OFD_Def(new MyTools.C_Setting_OpenFileDialog("Выберите книгу", MyTools.ETypeFileFilter.BookExcel_xls_xlsx));
            WorkBook = ATMisc.GetExcel(OFD.FileName, true);
            LoadColumn();
            LoadRow();
        }
        IWorkbook WorkBook;
        Grid grid;
        WrapPanel WPColumn;
        System.Windows.Controls.DataGrid DG;
        private void LoadColumn()
        {
            grid.SetRowFromGrid(MyTools.GL_Auto);
            WPColumn = new WrapPanel();
            grid.SetFromGrid(WPColumn);
            grid.SetRowFromGrid(MyTools.GL_Auto);
            DG = new System.Windows.Controls.DataGrid();
            DG.AutoGenerateColumns = false;
            grid.SetFromGrid(DG);
            foreach (var one in WorkBook.GetSheetAt(0).GetRow(0))
            {
                var col =new DataGridTextColumn();
                System.Windows.Data.Binding bind = new System.Windows.Data.Binding("Values[col" + one.ColumnIndex+']');
                col.Binding = bind;
                col.Header = one.StringCellValue;
                DG.Columns.Add(col);
            }
            var cols = new DataGridTextColumn();
            System.Windows.Data.Binding binds = new System.Windows.Data.Binding("YMD");
            cols.Binding = binds;
            cols.Header = "date";
            DG.Columns.Add(cols);
        }
        private void LoadRow()
        {
            Loaded = new List<SHOW>();
            int rows = WorkBook.GetSheetAt(0).LastRowNum;
            for (int i=1;i<rows+1;i++)
            {
                Dictionary<string, string> Values = new Dictionary<string, string>();
                foreach (var one in WorkBook.GetSheetAt(0).GetRow(i))
                { Values.Add("col" + one.ColumnIndex, one.StringCellValue); }
                Loaded.Add(new SHOW((uint)i, Values));
            }
            DG.ItemsSource = Loaded;
        }
        List<SHOW> Loaded;
        class SHOW
        {
            public SHOW(uint IDNumber, Dictionary<string, string> Values)
            {
                this.IDNumber = IDNumber;
                this.Values = Values;
            }
            uint IDNumber { get; }
            public Dictionary<string, string> Values { get; }
            public string YMD { get { return MyTools.StringDATE_In_intDate(Values["col0"], MyTools.EInputDate.YMDHMS, MyTools.EInputDate.YMDHM).ToString(); } }
        }
    }
}