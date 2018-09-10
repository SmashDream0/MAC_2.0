using MAC_2.Employee.Mechanisms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using System.IO;
using System.Windows;
using NPOI.SS;

namespace MAC_2.PrintForm
{
    public static class Template_Class
    {
        public static TP Get(data.ETypeTemplate type)
        {
            G.TemplatePrint.QUERRY()
                .SHOW
                .WHERE
                    .C(C.TemplatePrint.TypeTemplate, (uint)type)
                .AND
                .OB()
                    .AC(C.TemplatePrint.YM).Less.BV(DateControl_Class.SelectMonth)
                .OR
                    .C(C.TemplatePrint.YM, 0)
                .CB()
                .DO();

            TP[] list = new TP[G.TemplatePrint.Rows.Count];

            for (int i = 0; i < list.Length; i++)
            { list[i] = new TP(G.TemplatePrint.Rows.GetID(i)); }

            if (list.Length == 0)
            {
                MessageBox.Show("В базе не найдены ссылки на шаблоны!", "ВНИМАНИЕ!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            var result = list.OrderBy(x => x.YM).Last();

            if (CheckTemplateExist(result.Path))
            {
                G.TextFromTemplate.QUERRY()
                    .SHOW
                    .WHERE
                    .C(C.TextFromTemplate.TemplatePrint, result.ID)
                    .DO();

                result.textFromTP = new TextFromTP[G.TextFromTemplate.Rows.Count];

                for (int i = 0; i < result.textFromTP.Length; i++)
                { result.textFromTP[i] = new TextFromTP(G.TextFromTemplate.Rows.GetID(i)); }

                return result;
            }

            return null;
        }

        public class TextFromTP : MyTools.C_A_BaseFromAllDB
        {
            public TextFromTP(uint ID, bool CanEdit = true) : base(G.TextFromTemplate, ID, CanEdit)
            { }
            /// <summary>Текст</summary>
            public string Text => T.TextFromTemplate.Rows.Get<string>(ID, C.TextFromTemplate.Text);
            /// <summary>Доступные метки</summary>
            public string Template => T.TextFromTemplate.Rows.Get<string>(ID, C.TextFromTemplate.Template);
            /// <summary>Метка для шаблона</summary>
            public string ThisTemplate => T.TextFromTemplate.Rows.Get<string>(ID, C.TextFromTemplate.ThisTemplate);
            /// <summary>Имя листа</summary>
            public string ListName => T.TextFromTemplate.Rows.Get<string>(ID, C.TextFromTemplate.ListName);
        }

        /// <summary>Шаблон</summary>
        public class TP : MyTools.C_A_BaseFromAllDB
        {
            public TP(uint ID, bool CanEdit = true) : base(G.TemplatePrint, ID, CanEdit)
            { }
            /// <summary>Путь</summary>
            public string Path => $"{Directory.GetCurrentDirectory().ToString()}\\Шаблоны\\{T.TemplatePrint.Rows.Get<string>(ID, C.TemplatePrint.Path)}";
            /// <summary>Месяц начала действия</summary>
            public int YM => T.TemplatePrint.Rows.Get<int>(ID, C.TemplatePrint.YM);
            /// <summary>Получить книгу</summary>
            public NPOI.SS.UserModel.IWorkbook WorkBook
            { get { return ATMisc.GetExcel(Path, true); } }
            public TextFromTP[] textFromTP;
        }

        private static bool CheckTemplateExist(string Path)
        {
            if (!File.Exists(Path))
            {
                MessageBox.Show($"Файл по пути \"{Path}\" не найден", "ВНИМАНИЕ!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }
    }
}