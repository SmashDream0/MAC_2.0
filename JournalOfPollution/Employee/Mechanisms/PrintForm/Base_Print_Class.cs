using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AutoTable;
using MAC_2.Employee.Mechanisms;
using System.Windows;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.PrintForm
{
    public abstract class BasePrint : MyTools.C_A_BaseOpen_Excel
    {
        public BasePrint(data.ETypeTemplate typeTemplate)
        {
            TemplateStorage = Template_Class.Get(typeTemplate);
        }

        protected Styles styles;

        public void Start()
        {
            if (TemplateStorage != null)
            { internalStart(); }
        }

        protected abstract void internalStart();

        #region styles

        ///// <summary>Border {RLTB}/ Alignment {CC}/ Font {Times New Roman 10}/ Wrap</summary>
        //public static ICellStyle s_RLTB_CC_T10_W;
        ///// <summary>Border {RLTB}/ Alignment {CC}/ Font {Times New Roman 9}/ Wrap</summary>
        //public static ICellStyle s_RLTB_CC_T9_W;
        ///// <summary>Border {RLTB}/ Alignment {LC}/ Font {Times New Roman 10}/ Wrap</summary>
        //public static ICellStyle s_RLTB_LC_T10_W;
        ///// <summary>Border {RLTB}/ Alignment {LC}/ Font {Times New Roman 9}/ Wrap</summary>
        //public static ICellStyle s_RLTB_LC_T9_W;
        ///// <summary>Border {RLTB}/ Alignment {CC}/ Font {Times New Roman 6}/ Wrap</summary>
        //public static ICellStyle s_RLTB_CC_T6_W;
        ///// <summary>Border {RLTB}/ Alignment {RC}/ Font {Times New Roman 10}/ Wrap</summary>
        //public static ICellStyle s_RLTB_RC_T10_W;
        ///// <summary>Border {RLTB}/ Alignment {RC}/ Font {Times New Roman 10}/ Wrap/ Bold</summary>
        //public static ICellStyle s_RLTB_RC_T10_W_B;

        protected virtual bool CreateStyle()
        {
            if (book == null)
            { return false; }
            styles = new Styles(book);
            return styles.CreateStyle();
        }

        #endregion

        protected enum EPathPrint { Documents, Arhives }
        /// <summary>Шаблон</summary>
        protected Template_Class.TP TemplateStorage;
        /// <summary>Для выдачи сообщения заполнения</summary>
        protected Control_Print CP;

        /// <summary>Распечатать</summary>
        /// <param name="OutFolder">Путь начиная с местаположения программы</param>
        /// <param name="Name">Имя файла</param>
        /// <param name="pathPrint">Тип пути</param>
        protected void Print(string OutFolder, string Name, EPathPrint pathPrint)
        {
            string path = Directory.GetCurrentDirectory().ToString();
            switch (pathPrint)
            {
                case EPathPrint.Documents:
                    {
                        path += "\\Документы";
                        break;
                    }
                case EPathPrint.Arhives:
                    {
                        path += "\\Архив";
                        break;
                    }
            }
            path += "\\" + OutFolder;
            if (!Directory.Exists(path))
            { Directory.CreateDirectory(path); }
            path += "\\" + Name + ".xls";
            ATMisc.SaveExcel(book, path, true);
        }

        #region Автоэллементы

        /// <summary>Выбор даты</summary>
        public class DateSelector
        {
            public DateSelector(int YMD, string Text = null)
            {
                if (Text != null)
                { this.Text = Text; }
                SelectDate = new DatePicker();
                if (YMD != 0)
                { SelectDate.SelectedDate = new DateTime((YMD - 1) * 864000000000); }
                else
                { SelectDate.SelectedDate = DateTime.Now; }
            }
            public DateTime dateTime => (DateTime)SelectDate.SelectedDate;
            DatePicker SelectDate;
            string Text;
            /// <summary>Элемент для обзора</summary>
            public WrapPanel View
            {
                get
                {
                    WrapPanel result = new WrapPanel();
                    TextBlock Text = new TextBlock { Text = this.Text != null ? this.Text : "Выберите дату" };
                    result.Children.Add(Text);
                    result.Children.Add(SelectDate);
                    return result;
                }
            }
        }

        #region месный функционал

        /// <summary>Заполнение текста из базы</summary>
        /// <param name="ListName">имя листа</param>
        protected void LoadTextTemplate(ISheet sheet, string ListName, int count = 0)
        {
            CellExchange_Class ex = new CellExchange_Class(sheet);
            foreach (var one in TemplateStorage.textFromTP.Where(x => x.ListName == ListName))
            {
                var texts = one.Text.Split('\n').ToList();
                ICellStyle defStyle = null;
                IRow Defrow = SearchRowFromMark(sheet, one.ThisTemplate);
                if (texts.FirstOrDefault(x => x.Contains(StyleTarget.DefStyle)) != null)
                {
                    defStyle = LoadStyle(texts.First(x => x.Contains(StyleTarget.DefStyle)));
                    texts.Remove(texts.First(x => x.Contains(StyleTarget.DefStyle)));
                }
                if (texts.Count > 1)
                {
                    foreach (var text in texts)
                    {
                        Defrow = Defrow.CopyRowTo(Defrow.RowNum + 1);
                        IRow row = sheet.GetRow(Defrow.RowNum - 1);
                        string stroka = text;
                        if (text.Contains(StyleTarget.Style))
                        {
                            ICellStyle style = LoadStyle(text.Substring(text.IndexOf(StyleTarget.Style)));
                            foreach (var cell in row.Cells)
                            { cell.CellStyle = style; }
                            stroka = text.Substring(0, text.IndexOf(StyleTarget.Style));
                        }
                        else if (defStyle != null)
                        {
                            foreach (var cell in row.Cells)
                            { cell.CellStyle = defStyle; }
                        }
                        stroka = stroka.StringDivision(80);
                        ex.AddExchange(one.ThisTemplate, (e) => e.Row.Height = (short)(300 / 11 * 12 * stroka.Split('\n').Length), 1);
                        ex.AddExchange(one.ThisTemplate, stroka, 1);
                    }
                }
                else
                { ex.AddExchange(one.ThisTemplate, one.Text, count); }
                ex.AddExchange(one.ThisTemplate, (e) => { sheet.RemoveRow(e.Row); }, 1);
            }
            ex.Exchange();
        }

        #endregion

        #region Статические строки

        protected class StaticMark
        {
            public const string year = "{год текущий}";
            public const string month = "{месяц текущий}";

            public const string year_month_select = "{месяц год}";

            public const string number_folder = "{номер папки}";

            public const string abonent = "{абонент}";
            public const string legal_adres_abonent = "{юр.адрес}";

            public const string adres_abonent = "{адрес абонента}";

            public const string well = "{колодец}";

            public const string resolution_clarify = "{постановления";//их несколько 621, 644
        }

        /// <summary>Текущие месяц и год</summary>
        protected void MonthYear()
        {
            Substitute.AddExchange(StaticMark.month, MyTools.Month_From_M_C_R(DateTime.Now.Month, Reg: MyTools.ERegistor.ToLower), 0);
            Substitute.AddExchange(StaticMark.year, DateTime.Now.Year, 0);
        }

        /// <summary>Месяц и год отбора</summary>
        protected void MonthYearSelect()
        {
            Substitute.AddExchange(StaticMark.year_month_select,
            $"{MyTools.Month_From_M_C_R(MyTools.M_From_YM(DateControl_Class.SelectMonth), Reg: MyTools.ERegistor.ToLower)} {DateControl_Class.SelectYear} г.", 0);
        }

        /// <summary>Номер папки</summary>
        protected void NumberFolder(params int[] NumberFolder)
        {
            NumberFolder = NumberFolder.Distinct().ToArray();
            Substitute.AddExchange(StaticMark.number_folder, $"папк{(NumberFolder.Length > 1 ? 'и' : 'а')} {NumberFolder.Select(a => a.ToString()).Aggregate((a, b) => $"{a}, {b}")}", 0);
        }

        /// <summary>Наименование клиента</summary>
        protected void ClientName(Client client, bool TypeClient = true)
        {
            DetailsClient Details = client.Detail;
            string Storege = TypeClient ? client.TypeClient.InCase[(int)MyTools.ECases.Dative].Default + "\n" : string.Empty;
            switch (client.TypeClient.typeClient)
            {
                case data.ETypeClient.Individual:
                case data.ETypeClient.Physical:
                    {
                        Storege += TypeClient ? Details.NameInDative : Details.FullName;
                        break;
                    }
                case data.ETypeClient.Legal:
                    {
                        Storege += Details.FullName;
                        break;
                    }
            }
            Substitute.AddExchange(StaticMark.abonent, Storege, 1);
        }

        /// <summary>Юридический адрес клиента</summary>
        protected void ClientAdres(Client client, bool full)
        {
            Substitute.AddExchange(StaticMark.legal_adres_abonent,
                Helpers.LogicHelper.AdresLogic.FirstModel(client.Detail.AdresLegalID).Adr.CutAdres(full),
                0);
        }

        /// <summary>Адрес по объекту</summary>
        protected void ObjectAdres(Objecte objecte, bool full)
        {
            CP = new Control_Print();
            if (objecte.Separate)
            {
                DetailsObject details = objecte.Detail;
                if (details.MailAdres.Length > 0)
                { Substitute.AddExchange(StaticMark.adres_abonent, details.MailAdres.CutAdres(full), 0); }
                else if (details.LegalAdres.Length > 0)
                { Substitute.AddExchange(StaticMark.adres_abonent, details.LegalAdres.CutAdres(full), 0); }
                else
                { MessageBox.Show("Объект имеет индикатор абособленный, но адрес не задан!"); }
            }
            else
            {
                DetailsClient Details = objecte.Client.Detail;
                string Storege = string.Empty;
                bool PostAdres = Details.AdresPostID != 0;
                if (PostAdres)
                { Substitute.AddExchange(StaticMark.adres_abonent, Helpers.LogicHelper.AdresLogic.FirstModel(Details.AdresPostID).Adr.CutAdres(full), 0); }
                else
                {
                    CP.Elems.SetRowFromGrid(MyTools.GL_Auto);
                    WrapPanel wp = new WrapPanel();
                    wp.Orientation = Orientation.Vertical;
                    if (Details.AdresLegalID > 0)
                    {
                        RadioButton AdresLegal = new RadioButton();
                        AdresLegal.Content = Helpers.LogicHelper.AdresLogic.FirstModel(Details.AdresLegalID).Adr;
                        AdresLegal.Checked += (sender, e) => { Storege = AdresLegal.Content.ToString(); };
                        wp.Children.Add(AdresLegal);
                    }
                    RadioButton AdresObj = new RadioButton();
                    AdresObj.Content = objecte.Adres;
                    AdresObj.Checked += (sender, e) => { Storege = objecte.Adres; };
                    wp.Children.Add(AdresObj);
                    CP.Elems.SetFromGrid(wp);
                    CP.ShowDialog();
                }
                if (!PostAdres)
                { Substitute.AddExchange(StaticMark.adres_abonent, Storege.CutAdres(full), 0); }
            }

        }

        protected void WellNumber(SelectionWell sw)
        {
            Substitute.AddExchange(StaticMark.well, sw.Well.PresentNumber, 1);
        }

        /// <summary>Постановления</summary>
        protected void ResolutionClarify(Resolution resolution, bool show)
        {
            if (show)
            { Substitute.AddExchange($"{StaticMark.resolution_clarify} {resolution.CurtName}" + '}', resolution.GetResolutionClarify.Acts, 1); }
            else
            { Substitute.AddExchange($"{StaticMark.resolution_clarify} {resolution.CurtName}" + '}', e => { e.Row.ZeroHeight = true; }, 1); }
        }

        #endregion

        #endregion
    }
}