
namespace MAC_2
{
    public static partial class Misc
    {
        public static void DataBaseLoad(StartupLogo_Window.Loading_class Loading)
        {
            DataBase.ATSettings.AllowQuerryAutoConvertTypes = true;

            #region дополнительные первостепенные часть 1 таблицы

            if (!AddSynch(Loading, "Period", "Период", ref T.Period, ref G.Period,
                newTable =>
                {
                    newTable.Columns.AddInt32("YM", "Месяц", DataBase.ETypeView.YMT);
                    newTable.Columns.AddDecimal("Price", "Тариф");
                    newTable.Columns.AddDecimal("NDS", "НДС");
                    newTable.Columns.AddDecimal("MinLimits", "Минимальный лимит");
                    newTable.Columns.AddDecimal("SamplesMonthLimit", "Норма кол-ва проб в день");

                    newTable.Columns.Add_Unique(C.Period.YM);
                }, false)) { return; }

            if (!AddSynch(Loading, "DistrictReference", "Район", ref T.DistrictReference, ref G.DistrictReference,
                newTable =>
                {
                    newTable.Columns.AddString("Name", "Район", 20);
                    newTable.Columns.AddInt32("Order", "Порядок", DataBase.ETypeView.intT);

                    newTable.Columns.Add_Unique(C.DistrictReference.Name);
                }, false)) { return; }

            if (!AddSynch(Loading, "AdresReference", "Адрес", ref T.AdresReference, ref G.Adres,
                newTable =>
                {
                    newTable.Columns.AddString("Adres", "Адрес", 200);
                    newTable.Columns.AddRelation(T.DistrictReference.GetColumn(C.DistrictReference.Name));

                    newTable.Columns.Add_Unique(C.AdresReference.Adres);
                }, false)) { return; }

            if (!AddSynch(Loading, "Post", "Должность", ref T.Post, ref G.Post,
                newTable =>
                {
                    newTable.Columns.AddString("CurtName", "Краткое наименование", 40);
                    newTable.Columns.AddString("FullName", "Полное наименование", 120);

                    newTable.Columns.Add_Unique(C.Post.FullName);
                }, false)) { return; }

            if (!AddSynch(Loading, "Worker", "Работник", ref T.Worker, ref G.Worker,
                newTable =>
                {
                    newTable.Columns.AddString("SureName", "Фамилия", 30);
                    newTable.Columns.AddString("Name", "Имя", 30);
                    newTable.Columns.AddString("Patronymic", "Отчество", 30);
                    newTable.Columns.AddRelation(T.Post.GetColumn(C.Post.FullName));
                    newTable.Columns.AddInt32("YMFrom", "Месяц от", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Месяц до", DataBase.ETypeView.YMT);
                    newTable.Columns.AddRelation(T.User.GetColumn(C.User.Login));

                    newTable.Columns.Add_Unique(C.Worker.SureName, C.Worker.Name, C.Worker.Patronymic);
                }, false)) { return; }

            if (!AddSynch(Loading, "RatioSigner", "Подписывающие/для отбора", ref T.RatioSigner, ref G.RatioSigner,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.TypeTemplate.GetColumn(C.TypeTemplate.Name));
                    newTable.Columns.AddRelation(T.Post.GetColumn(C.Post.CurtName));
                    newTable.Columns.AddRelation(T.Worker.GetColumn(C.Worker.SureName));
                    newTable.Columns.AddString("List", "Имя листа", 20);
                    newTable.Columns.AddInt32("Position", "Положение", DataBase.ETypeView.intT);

                    newTable.Columns.Add_Unique(C.RatioSigner.TypeTemplate, C.RatioSigner.Post,C.RatioSigner.List, C.RatioSigner.Position);
                    newTable.Columns.Add_Unique(C.RatioSigner.TypeTemplate, C.RatioSigner.Worker, C.RatioSigner.List, C.RatioSigner.Position);
                }, false)) { return; }

            if (!AddSynch(Loading, "Units", "Единицы измерений", ref T.Units, ref G.Units,
                newTable =>
                {
                    newTable.Columns.AddString("Name", "Наименование", 20);

                    newTable.Columns.Add_Unique(C.Units.Name);
                }, false)) { return; }

            if (!AddSynch(Loading, "Unit", "Подразделение", ref T.Unit, ref G.Unit,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.TypeConstruction.GetColumn(C.TypeConstruction.Name));
                    newTable.Columns.AddString("Name", "Наименование", 40);
                    newTable.Columns.AddString("FullName", "Полное наименование", 100);
                    newTable.Columns.AddString("Contact", "Адрес", 100);
                    newTable.Columns.AddInt32("Podr", "Номер подразделения", DataBase.ETypeView.intT);

                    newTable.Columns.Add_Unique(C.Unit.Name, C.Unit.FullName);
                }, false)) { return; }

            //if (!AddSynch(Loading, "UnitToTypeWell", "Подразделение к типу колодца", ref T.UnitToTypeWell, ref G.UnitToTypeWell,
            //    newTable =>
            //    {
            //        newTable.Columns.AddRelation(T.Unit.GetColumn(C.Unit.Name));
            //        newTable.Columns.AddRelation(T.TypeWell.GetColumn(C.TypeWell.FullName));

            //        newTable.Columns.Add_Unique(C.UnitToTypeWell.Unit, C.UnitToTypeWell.TypeWell);
            //    }, false)) { return; }

            #endregion

            #region клиент, объект, колодец

            if (!AddSynch(Loading, "Client", "Клиент", ref T.Client, ref G.Client,
                newTable =>
                {
                    newTable.Columns.AddString("INN", "ИНН", 12, DataBase.EColLocation.Local, true, "0");
                    newTable.Columns.AddBool("CanSelect", "Можно отбирать", DataBase.EColLocation.Local, true, true);
                    newTable.Columns.AddBool("MustDeclair", "Обязан подать декларацию");
                    newTable.Columns.AddInt32("YMFrom", "Дата создания в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Дата закрытия в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddRelation(T.TypeClient.GetColumn(C.TypeClient.Name));

                    newTable.Columns.Add_Unique(C.Client.INN);
                }, false)) { return; }

            if (!AddSynch(Loading, "DetailsClient", "Реквизиты клиента", ref T.DetailsClient, ref G.DetailsClient,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Client.GetColumn(C.Client.INN));
                    newTable.Columns.AddString("FullName", "Полное наименование", 150);
                    newTable.Columns.AddRelation(T.AdresReference.GetColumn(C.AdresReference.Adres), "Post", "Почтовый адрес");
                    newTable.Columns.AddRelation(T.AdresReference.GetColumn(C.AdresReference.Adres), "Legal", "Юридический адрес");
                    newTable.Columns.AddInt32("YM", "Дата действия с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddString("NameInDative", "Наименование в дательном падеже", 150);

                    newTable.Columns.Add_Unique(C.DetailsClient.Client, C.DetailsClient.YM);
                }, false)) { return; }

            if (!AddSynch(Loading, "Object", "Объект", ref T.Objecte, ref G.Objecte,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Client.GetColumn(C.Client.INN));
                    newTable.Columns.AddInt32("NumberFolder", "Номер папки", DataBase.ETypeView.intT);
                    newTable.Columns.AddRelation(T.AdresReference.GetColumn(C.AdresReference.Adres), "Fact", "Адрес фактический");
                    newTable.Columns.AddRelation(T.TypeSample.GetColumn(C.TypeSample.Name));
                    newTable.Columns.AddInt32("YMFrom", "Дата создания в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Дата закрытия в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddString("Account1C", "Лицевые из 1С", 220);
                    newTable.Columns.AddBool("Separate", "Обособленный");

                    newTable.Columns.Add_Unique(C.Objecte.NumberFolder);
                }, false)) { return; }

            if (!AddSynch(Loading, "DetailsObject", "Реквизиты объекта", ref T.DetailsObject, ref G.DetailsObject,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Objecte.GetColumn(C.Objecte.NumberFolder));
                    newTable.Columns.AddInt32("YM", "Дата действия с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddString("AddName", "Добавочное имя", 70);
                    newTable.Columns.AddRelation(T.AdresReference.GetColumn(C.AdresReference.Adres), "Legal", "Юридический фактический");
                    newTable.Columns.AddRelation(T.AdresReference.GetColumn(C.AdresReference.Adres), "Mail", "Почтовый фактический");
                }, false)) { return; }

            if (!AddSynch(Loading, "Well", "Колодец", ref T.Well, ref G.Well,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Objecte.GetColumn(C.Objecte.NumberFolder));
                    newTable.Columns.AddRelation(T.TypeWell.GetColumn(C.TypeWell.CurtName));
                    newTable.Columns.AddInt32("Number", "Номер колодца", DataBase.ETypeView.intT);
                    newTable.Columns.AddInt32("YMFrom", "Дата создания в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Дата закрытия в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddRelation(T.Unit.GetColumn(C.Unit.Name));

                    newTable.Columns.Add_Unique(C.Well.Object, C.Well.Number);
                }, false)) { return; }

            #endregion

            #region дополнительные первостепенные часть 2 таблицы

            if (!AddSynch(Loading, "Representative", "Представитель", ref T.Representative, ref G.Representative,
                newTable =>
                {
                    newTable.Columns.AddString("FIO", "Фамилия", 80);
                    newTable.Columns.AddRelation(T.Client.GetColumn(C.Client.INN));
                    newTable.Columns.AddString("Post", "Должность", 120);

                    newTable.Columns.Add_Unique(C.Representative.FIO, C.Representative.Client, C.Representative.Post);
                }, false)) { return; }

            #endregion

            #region загрязнение, норматив, декларация

            if (!AddSynch(Loading, "Pollution", "Загрязнение", ref T.Pollution, ref G.Pollution,
                newTable =>
                {
                    newTable.Columns.AddString("CurtName", "Краткое наименование", 40);
                    newTable.Columns.AddString("FullName", "Полное наименование", 40);
                    newTable.Columns.AddInt32("Key", "Код", DataBase.ETypeView.intT);
                    newTable.Columns.AddRelation(T.Units.GetColumn(C.Units.Name));
                    newTable.Columns.AddInt32("Round", "Точность", DataBase.ETypeView.intT);
                    newTable.Columns.AddInt32("Number", "Номер по порядку", DataBase.ETypeView.intT);
                    newTable.Columns.AddBool("HasRange", "Диапозонное значение");
                    newTable.Columns.AddInt32("YMFrom", "Действует от в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Действует до в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddBool("Show", "Показывать в основном окне", DataBase.EColLocation.Local, true, true);
                    newTable.Columns.AddString("UniqueKey", "Уникальный ключ в рамках всех программ", 20);
                    newTable.Columns.AddString("Method", "Методика", 100);

                    newTable.Columns.Add_Unique(C.Pollution.Key, C.Pollution.Number);
                }, false)) { return; }

            if (!AddSynch(Loading, "AccurateMeasurement", "Точность измерений", ref T.AccurateMeasurement, ref G.AccurateMeasurement,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddDecimal("From", "От");
                    newTable.Columns.AddDecimal("To", "До");
                    newTable.Columns.AddDecimal("Number", "Значение");
                    newTable.Columns.AddBool("IsPercent", "Проценты");
                    newTable.Columns.AddInt32("YM", "Дата действия с в месяцах", DataBase.ETypeView.YMT);
                }, false)) { return; }

            if (!AddSynch(Loading, "Resolution", "Постановление", ref T.Resolution, ref G.Resolution,
                newTable =>
                {
                    newTable.Columns.AddString("CurtNmae", "Краткое наименование", 20);
                    newTable.Columns.AddString("NameOf1C", "Имя из 1С", 20);
                    newTable.Columns.AddInt32("YMFrom", "Действует от в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Действует до в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddRelation(T.TypeConstruction.GetColumn(C.TypeConstruction.Name));
                    newTable.Columns.AddBool("OffForce", "Выключен принудительно", DataBase.EColLocation.Local, true, false);
                    newTable.Columns.AddString("Color", "Цвет", 9, DataBase.ETypeView.ColorStringT);
                }, false)) { return; }

            if (!AddSynch(Loading, "ResolutionClarify", "Актуализация постановления", ref T.ResolutionClarify, ref G.ResolutionClarify,
                newTable =>
                {
                    newTable.Columns.AddString("Number", "Номер постановления", 20);
                    newTable.Columns.AddRelation(T.Resolution.GetColumn(C.Resolution.CurtName));
                    newTable.Columns.AddString("FullName", "Полное наименование", 100);
                    newTable.Columns.AddString("Note", "Описание", 100);
                    newTable.Columns.AddString("Acts", "Акты", 700);
                    newTable.Columns.AddInt32("YMFrom", "Действует с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Действует до в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddRelation(T.TypeResolution.GetColumn(C.TypeResolution.Name));

                    newTable.Columns.Add_Unique(C.ResolutionClarify.Resolution, C.ResolutionClarify.YMFrom);
                }, false)) { return; }

            if (!AddSynch(Loading, "ValueNorm", "Значение нормы", ref T.ValueNorm, ref G.ValueNorm,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.ResolutionClarify.GetColumn(C.ResolutionClarify.Resolution));
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddDecimal("From", "От");
                    newTable.Columns.AddDecimal("To", "До");
                    newTable.Columns.AddRelation(T.Unit.GetColumn(C.Unit.Name));
                    newTable.Columns.AddDecimal("Multiplier", "Множитель");
                    newTable.Columns.AddInt32("YMFrom", "Действует с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Действует до в месяцах", DataBase.ETypeView.YMT);

                    newTable.Columns.Add_Unique(C.ValueNorm.Pollution, C.ValueNorm.ResolutionClarify, C.ValueNorm.Unit, C.ValueNorm.YMFrom);
                }, false)) { return; }

            if (!AddSynch(Loading, "PriceNorm", "Стоимость нормы", ref T.PriceNorm, ref G.PriceNorm,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.ResolutionClarify.GetColumn(C.ResolutionClarify.Resolution));
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddDecimal("Price", "Стоимость");
                    newTable.Columns.AddDOUBLE("MultiAs", "Множитель как коэффициент воздействия");
                    newTable.Columns.AddInt32("YMFrom", "Действует с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Действует до в месяцах", DataBase.ETypeView.YMT);

                    newTable.Columns.Add_Unique(C.PriceNorm.Pollution, C.PriceNorm.ResolutionClarify, C.PriceNorm.YMFrom);
                }, false)) { return; }

            if (!AddSynch(Loading, "Declaration", "Декларация", ref T.Declaration, ref G.Declaration,
                newTable =>
                {
                    newTable.Columns.AddString("Name", "Наименование", 20);
                    newTable.Columns.AddRelation(T.Well.GetColumn(C.Well.Number));
                    newTable.Columns.AddInt32("YM", "Действует с в месяцах", DataBase.ETypeView.YMT);

                    newTable.Columns.Add_Unique(C.Declaration.Well, C.Declaration.YM);
                }, false)) { return; }

            if (!AddSynch(Loading, "DeclarationValue", "Значения декларации", ref T.DeclarationValue, ref G.DeclarationValue,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Declaration.GetColumn(C.Declaration.Name));
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddDecimal("From", "От");
                    newTable.Columns.AddDecimal("To", "До");

                    newTable.Columns.Add_Unique(C.DeclarationValue.Declaration, C.DeclarationValue.Pollution);
                }, false)) { return; }

            if (!AddSynch(Loading, "HitModePollution", "Виды загрязнений", ref T.HitModePollution, ref G.HitModePollution,
                newTable =>
                {
                    newTable.Columns.AddString("Name", "Наименование", 20);
                    newTable.Columns.AddString("Note", "Описание", 100);
                    newTable.Columns.AddString("Multi", "Множитель", 50);
                    newTable.Columns.AddInt32("MonthTrack", "Срок отслеживания в месяцах", DataBase.ETypeView.intT);
                    newTable.Columns.AddInt32("MonthStartTack", "Месяц начала отслеживания", DataBase.ETypeView.intT);
                    newTable.Columns.AddBool("Joint", "Косяк", DataBase.EColLocation.Local, true, true);
                    newTable.Columns.AddString("Formula", "Формула", 40);
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddRelation(T.Resolution.GetColumn(C.Resolution.CurtName));
                    newTable.Columns.AddBool("CanOnly", "Может отбираться единолично");
                    newTable.Columns.AddString("FormulaPunishment", "Формула наказания", 40);
                }, false)) { return; }

            if (!AddSynch(Loading, "Coefficient", "Коэффициент", ref T.Coefficient, ref G.Coefficient,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.ResolutionClarify.GetColumn(C.ResolutionClarify.Resolution));
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddInt32("YMFrom", "Действует с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMTo", "Действует с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddBool("Compare", "Для сравнения");
                }, false)) { return; }

            if (!AddSynch(Loading, "CoefficientValue", "Значение коэффициента", ref T.CoefficientValue, ref G.CoefficientValue,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Coefficient.GetColumn(C.Coefficient.Pollution));
                    newTable.Columns.AddDecimal("From", "От");
                    newTable.Columns.AddDecimal("To", "До");
                    newTable.Columns.AddDecimal("Value", "Коэффициент");
                }, false)) { return; }

            if (!AddSynch(Loading, "CalculationFormula", "Формула расчёта", ref T.CalculationFormula, ref G.CalculationFormula,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.ResolutionClarify.GetColumn(C.ResolutionClarify.Resolution));
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddInt32("Number", "Номер", DataBase.ETypeView.intT);
                    newTable.Columns.AddString("Formula", "Формула", 40);
                    newTable.Columns.AddRelation(T.GettingValue.GetColumn(C.GettingValue.Name), "Formula");
                    newTable.Columns.AddRelation(T.CalculationFormula.GetColumn(C.CalculationFormula.Number), "Link");
                    newTable.Columns.AddRelation(T.GettingValue.GetColumn(C.GettingValue.Name), "Link");
                    newTable.Columns.AddInt32("YM", "Действует с в месяцах", DataBase.ETypeView.YMT);
                    newTable.Columns.AddBool("DefFormula", "Формула для использования по умолчанию");
                    newTable.Columns.AddString("Label", "Метка", 15);

                    newTable.Columns.Add_Unique(C.CalculationFormula.ResolutionClarify, C.CalculationFormula.Pollution, C.CalculationFormula.DefFormula);
                }, false)) { return; }

            #endregion

            #region проба, отбор

            if (!AddSynch(Loading, "Sample", "Проба", ref T.Sample, ref G.Sample,
                newTable =>
                {
                    newTable.Columns.AddInt32("YM", "Месяц создания", DataBase.ETypeView.YMT);
                    newTable.Columns.AddRelation(T.Worker.GetColumn(C.Worker.SureName));
                    newTable.Columns.AddRelation(T.Representative.GetColumn(C.Representative.FIO));
                    newTable.Columns.AddRelation(T.Status.GetColumn(C.Status.Name));
                }, false)) { return; }

            if (!AddSynch(Loading, "Volume", "Объёмы", ref T.Volume, ref G.Volume,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Sample.GetColumn(C.Sample.Status));
                    newTable.Columns.AddDOUBLE("Value", "Объём");
                    newTable.Columns.AddRelation(T.Period.GetColumn(C.Period.Price));

                    newTable.Columns.Add_Unique(C.Volume.Sample, C.Volume.Period);
                }, false)) { return; }

            if (!AddSynch(Loading, "SelectionWell", "Отбор колодца", ref T.SelectionWell, ref G.SelectionWell,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Well.GetColumn(C.Well.Number));
                    newTable.Columns.AddRelation(T.Sample.GetColumn(C.Sample.Status));
                    newTable.Columns.AddInt32("Number", "Номер", DataBase.ETypeView.intT);
                    newTable.Columns.AddInt32("DateTime", "Дата время в минутах", DataBase.ETypeView.YMDHMT);
                }, false)) { return; }

            if (!AddSynch(Loading, "ValueSelection", "Значения отбора", ref T.ValueSelection, ref G.ValueSelection,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.SelectionWell.GetColumn(C.SelectionWell.Well));
                    newTable.Columns.AddRelation(T.Pollution.GetColumn(C.Pollution.FullName));
                    newTable.Columns.AddDecimal("Value", "Значение");
                }, false)) { return; }

            if (!AddSynch(Loading, "HitSelectPollution", "Фактическое загрязнения", ref T.HitSelectPollution, ref G.HitSelectPollution,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.HitModePollution.GetColumn(C.HitModePollution.Name));
                    newTable.Columns.AddRelation(T.Sample.GetColumn(C.Sample.Status));
                    newTable.Columns.AddInt32("Number", "Колличество попадений", DataBase.ETypeView.intT);
                    newTable.Columns.AddBool("Only", "Отобран единолично");
                }, false)) { return; }

            #endregion

            #region дополнительные второстепенные таблицы 
            
            if (!AddSynch(Loading, "ObjectFromResolution", "Отношение объект к постановлению", ref T.ObjectFromResolution, ref G.ObjectFromResolution,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Resolution.GetColumn(C.Resolution.CurtName));
                    newTable.Columns.AddRelation(T.Objecte.GetColumn(C.Objecte.NumberFolder));
                    newTable.Columns.AddString("Reason", "Причина", 120);
                    newTable.Columns.AddBool("Application", "Заперетить отбор", DataBase.EColLocation.Local, true, true);

                    newTable.Columns.Add_Unique(C.ObjectFromResolution.Resolution, C.ObjectFromResolution.Object);
                }, false)) { return; }

            if (!AddSynch(Loading, "NormDoc", "Нормативные документы", ref T.NormDoc, ref G.NormDoc,
                newTable =>
                {
                    newTable.Columns.AddString("Act", "Акт", 50);
                    newTable.Columns.AddString("Score", "Счет", 50);
                    newTable.Columns.AddString("Invoces", "Счет-фактура", 50);
                    newTable.Columns.AddInt32("Date", "Дата", DataBase.ETypeView.YMDT);
                    newTable.Columns.AddDecimal("Summ", "Сумма");
                    newTable.Columns.AddRelation(T.Resolution.GetColumn(C.Resolution.CurtName));
                    newTable.Columns.AddRelation(T.Volume.GetColumn(C.Volume.Value));
                }, false)) { return; }

            if (!AddSynch(Loading, "MidMonthVolume", "Среднемесячный объём", ref T.MidMonthVolume, ref G.MidMonthVolume,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Objecte.GetColumn(C.Objecte.AdresFact));
                    newTable.Columns.AddInt32("Year", "Год", DataBase.ETypeView.YT);
                    newTable.Columns.AddDOUBLE("Volume", "Объём");
                }, false)) { return; }

            if (!AddSynch(Loading, "NegotiationAssistant", "Помощник согласования", ref T.NegotiationAssistant, ref G.NegotiationAssistant,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.Objecte.GetColumn(C.Objecte.AdresFact));
                    newTable.Columns.AddRelation(T.Sample.GetColumn(C.Sample.Status));
                    newTable.Columns.AddInt32("YM", "Месяц", DataBase.ETypeView.YMT);
                    newTable.Columns.AddInt32("YMD", "Дата", DataBase.ETypeView.YMDT);
                    newTable.Columns.AddRelation(T.Worker.GetColumn(C.Worker.Name));

                    newTable.Columns.Add_Unique(C.NegotiationAssistant.Objecte, C.NegotiationAssistant.YM);
                }, false)) { return; }

            if (!AddSynch(Loading, "Accredit", "Аккредитация", ref T.Accredit, ref G.Accredit,
                newTable =>
                {
                    newTable.Columns.AddString("Text", "Текст", 80);
                    newTable.Columns.AddInt32("YMDFrom", "Дата от в днях", DataBase.ETypeView.YMDT);
                    newTable.Columns.AddInt32("YMDTo", "Дата до в днях", DataBase.ETypeView.YMDT);
                }, false)) { return; }

            #endregion

            #region вспомогательный материал

            if (!AddRemote(Loading, "StorageInstructions", "Хранилище инструкций", ref T.StorageInstructions, ref G.StorageInstructions,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.UType.GetColumn(0));
                    newTable.Columns.AddString("Name", "Имя инструкции", 150);
                    newTable.Columns.AddString("Path", "Путь до инструкции", 150);
                    newTable.Columns.AddRelation(T.TypeInstruction.GetColumn(0));
                }, false)) { return; }

            if (!AddRemote(Loading, "TemplatePrint", "Шаблоны для печати", ref T.TemplatePrint, ref G.TemplatePrint,
                newTable =>
                {
                    newTable.Columns.AddString("Name", "Имя инструкции", 150);
                    newTable.Columns.AddRelation(T.TypeTemplate.GetColumn(0));
                    newTable.Columns.AddString("Path", "Путь до инструкции", 150);
                    newTable.Columns.AddInt32("YM", "Месяц начала действия", DataBase.ETypeView.YMT);

                    newTable.Columns.Add_Unique(C.TemplatePrint.Path);
                    newTable.Columns.Add_Unique(C.TemplatePrint.Name, C.TemplatePrint.YM);
                }, false)) { return; }

            if (!AddRemote(Loading, "TextFromTemplate", "Текст для шаблона", ref T.TextFromTemplate, ref G.TextFromTemplate,
                newTable =>
                {
                    newTable.Columns.AddRelation(T.TemplatePrint.GetColumn(C.TemplatePrint.Name));
                    newTable.Columns.AddString("ListName", "Имя листа", 20);
                    newTable.Columns.AddString("Template", "Доступные метки", 400);
                    newTable.Columns.AddString("Text", "Текст для заполнения", 3000);
                    newTable.Columns.AddString("ThisTemplate", "Метка расположения на шаблоне", 25);
                }, false)) { return; }

            #endregion

            Helpers.LogicHelper.InitLogics();
        }
    }
}