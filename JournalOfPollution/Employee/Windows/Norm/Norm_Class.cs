using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AutoTable;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using MAC_2.Model;
using MAC_2.Employee.Windows.Norm;

namespace MAC_2.Employee.Windows
{
    public class NormViewModel
    {
        public NormViewModel(DataGrid DG)
        {
            this.DG = DG;
            PollutionBase_Class.LoadValueNorms();
            PollutionBase_Class.LoadPriceNorm();
            
            Start();
        }
        DataGrid DG;

        private void Start()
        {
            PollutionBase_Class.LoadAccurateMeasurement(true);
            LoadThisShow();
            DG.ItemsSource = _shows;
        }
        
        private PollutionItem[] _shows;
        //public IEnumerable<Pollution> Pollutions { get; private set; }
        //public Resolution_NormValues[] Resolutions { get; private set; }

        private void LoadThisShow()
        {
            _shows = PollutionBase_Class.AllPolutions.Select(x => new PollutionItem(x.ID)).ToArray();

            bool ReadOnly = false;

            if ((data.UType)data.User<uint>(C.User.UType) == data.UType.Admin)
            { ReadOnly = true; }

            List<MyTools.C_DGColumn_Bind> columns = new List<MyTools.C_DGColumn_Bind>();

            columns.Add(new MyTools.C_DGColumn_Bind("Код", MyTools.E_TypeColumnDG.Text, "Key", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, IsReadOnly: ReadOnly)));
            columns.Add(new MyTools.C_DGColumn_Bind("Краткое имя", MyTools.E_TypeColumnDG.Text, "CurtName", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, IsReadOnly: ReadOnly)));
            columns.Add(new MyTools.C_DGColumn_Bind("Полное имя", MyTools.E_TypeColumnDG.Text, "FullName", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, IsReadOnly: ReadOnly)));
            columns.Add(new MyTools.C_DGColumn_Bind("Округл", MyTools.E_TypeColumnDG.Text, "Round", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, IsReadOnly: ReadOnly)));
            columns.Add(new MyTools.C_DGColumn_Bind("Номер\nпо\nпорядку", MyTools.E_TypeColumnDG.Text, "Number", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay)));
            columns.Add(new MyTools.C_DGColumn_Bind("Показывать\nв основном\nокне", MyTools.E_TypeColumnDG.CheckBox, "Show", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay)));
            columns.Add(new MyTools.C_DGColumn_Bind("Точность\nизмерений", MyTools.E_TypeColumnDG.Text, "Value", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.OneWay, style: new MyTools.C_Style_DGColumn(10))));

            foreach (var resolution in PollutionBase_Class.AllResolution)
            {
                var units = PollutionBase_Class.AllValueNorm.Where(x=>x.ResolutionID == resolution.ID && x.UnitID > 0).GroupBy(x => x.UnitID).Select(x => x.First().Unit).ToArray();
                var type = resolution.GetResolutionClarify.TypeResolution;

                switch (type)
                {
                    case data.ETypeResolution.CostNorm:
                    case data.ETypeResolution.Cost:
                        {
                            columns.Add(new MyTools.C_DGColumn_Bind($"{resolution.CurtName} Стоимость", MyTools.E_TypeColumnDG.Text, $"priceNorms[{resolution.GetResolutionClarify.ID}].Value", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, style: new MyTools.C_Style_DGColumn(brush: resolution.Color))));
                            if (type == data.ETypeResolution.CostNorm)
                            { goto case data.ETypeResolution.Norm; }
                            break;
                        }
                    case data.ETypeResolution.Norm:
                        {
                            if (units.Any())
                            {
                                foreach (var unit in units)
                                {
                                    columns.Add(new MyTools.C_DGColumn_Bind($"{resolution.CurtName}\n{unit.Name}", MyTools.E_TypeColumnDG.Text, $"ValueNorm[{unit.ID}|{resolution.GetResolutionClarify.ID}].Value", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, style: new MyTools.C_Style_DGColumn(brush: resolution.Color))));
                                }
                            }
                            else
                            {
                                columns.Add(new MyTools.C_DGColumn_Bind($"{resolution.CurtName}", MyTools.E_TypeColumnDG.Text, $"ValueNorm[0|{resolution.GetResolutionClarify.ID}].Value", new MyTools.C_Setting_DGColumn(System.Windows.Data.BindingMode.TwoWay, style: new MyTools.C_Style_DGColumn(brush: resolution.Color))));
                            }
                            break;
                        }
                }
            }

            foreach (var column in columns)
            { DG.Columns.Add(column.Column); }

            DG.FrozenColumnCount = 2;
        }
    }
}