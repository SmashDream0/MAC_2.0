using System.Text;

namespace MAC_2
{
    public static partial class Misc
    {
        public static void DataBaseLoadFT(StartupLogo_Window.Loading_class Loading)
        {
            if (Loading != null)
            { Loading.LoadingComment = "Создание таблиц"; }
            {
                //Менять тут что-либо можно только добавлением записей в таблицы или переименованием полей!
                #region Типы учетных записей

                T.UType = data.T1.Tables.Add(Encoding.GetEncoding(866), "UType", "Типы учетных записей");
                T.UType.Columns.AddString("Name", "Наименование", 12);
                G.UType = T.UType.CreateSubTable();

                G.UType.Rows.Add((uint)data.UType.Admin, new object[] { "Админ" });
                G.UType.Rows.Add((uint)data.UType.MainWork, new object[] { "Ответственный" });
                G.UType.Rows.Add((uint)data.UType.Laborant, new object[] { "Лаборант" });
                G.UType.Rows.Add((uint)data.UType.Browser, new object[] { "Просмотрщик" });

                #endregion

                #region Статус

                T.Status = data.T1.Tables.Add(Encoding.GetEncoding(866), "Status", "Статус");
                T.Status.Columns.AddString("Name", "Наименование", 10);
                G.Status = T.Status.CreateSubTable();

                G.Status.Rows.Add((uint)data.EStatus.Selected, new object[] { "Отобран" });
                G.Status.Rows.Add((uint)data.EStatus.Close, new object[] { "Закрыт" });
                G.Status.Rows.Add((uint)data.EStatus.NotLimit, new object[] { "Сумма меньше лимита" });
                G.Status.Rows.Add((uint)data.EStatus.NotVolume, new object[] { "Без объёма" });

                #endregion

                #region Тип отбора

                T.TypeSample = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeSample", "Тип отбора");
                T.TypeSample.Columns.AddString("Name", "Наименование", 40);
                G.TypeSample = T.TypeSample.CreateSubTable();

                G.TypeSample.Rows.Add((uint)data.ETypeSample.Simple, new object[] { "простая (разовая, точечная)" });
                G.TypeSample.Rows.Add((uint)data.ETypeSample.Mixed, new object[] { "смешанная (усредненная, составная)" });

                #endregion

                #region Тип колодца

                T.TypeWell = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeWell", "Тип колодца");
                T.TypeWell.Columns.AddString("CurtName", "Краткое наименование", 10);
                T.TypeWell.Columns.AddString("FullName", "Полное наименование", 40);
                G.TypeWell = T.TypeWell.CreateSubTable();

                G.TypeWell.Rows.Add((uint)data.ETypeWell.KK, new object[] { "КК", "контрольный канализационный колодец" });
                G.TypeWell.Rows.Add((uint)data.ETypeWell.KNS, new object[] { "КНС", "КНС" });
                G.TypeWell.Rows.Add((uint)data.ETypeWell.PKSA, new object[] { "ПКСА", "последний колодец на сети абонента" });
                G.TypeWell.Rows.Add((uint)data.ETypeWell.PK, new object[] { "ПК", "приёмная камера" });

                #endregion

                #region Тип сооружения

                T.TypeConstruction = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeConstruction", "Тип сооружения");
                T.TypeConstruction.Columns.AddString("Name", "Наименование", 25);
                G.TypeConstruction = T.TypeConstruction.CreateSubTable();

                G.TypeConstruction.Rows.Add((uint)data.ETypeConstruction.Sewer, new object[] { "Канализация" });

                #endregion

                #region Получаемое значение

                T.GettingValue = data.T1.Tables.Add(Encoding.GetEncoding(866), "GettingValue", "Получаемое значение");
                T.GettingValue.Columns.AddString("Name", "Наименование", 25);
                G.GettingValue = T.GettingValue.CreateSubTable();

                G.GettingValue.Rows.Add((uint)data.EGettingValue.Medium, new object[] { "Среднее значение" });
                G.GettingValue.Rows.Add((uint)data.EGettingValue.Max, new object[] { "Максимальное значение" });
                G.GettingValue.Rows.Add((uint)data.EGettingValue.Summ, new object[] { "Сумма значений" });

                #endregion

                #region Тип постановления

                T.TypeResolution = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeResolution", "Тип постановления");
                T.TypeResolution.Columns.AddString("Name", "Наименование", 20);
                G.TypeResolution = T.TypeResolution.CreateSubTable();

                G.TypeResolution.Rows.Add((uint)data.ETypeResolution.Cost, new object[] { "Ставка/Стоимость" });
                G.TypeResolution.Rows.Add((uint)data.ETypeResolution.Norm, new object[] { "Нормативы/Значания" });
                G.TypeResolution.Rows.Add((uint)data.ETypeResolution.CostNorm, new object[] { "Нормативы/Ставка" });

                #endregion

                #region Тип инструкции

                T.TypeInstruction = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeInstruction", "Тип инструкции");
                T.TypeInstruction.Columns.AddString("Name", "Наименование", 20);
                G.TypeInstruction = T.TypeInstruction.CreateSubTable();

                G.TypeInstruction.Rows.Add((uint)data.ETypeInstruction.Admin, new object[] { "Администратор" });
                G.TypeInstruction.Rows.Add((uint)data.ETypeInstruction.DefWindow, new object[] { "Основная форма" });
                G.TypeInstruction.Rows.Add((uint)data.ETypeInstruction.EditorValue, new object[] { "Занечение значений" });
                G.TypeInstruction.Rows.Add((uint)data.ETypeInstruction.EditorClient, new object[] { "Редактирование клиента" });
                G.TypeInstruction.Rows.Add((uint)data.ETypeInstruction.LoadVolume, new object[] { "Загрузчик объёмов" });
                G.TypeInstruction.Rows.Add((uint)data.ETypeInstruction.LoadSyncJPC, new object[] { "Загрузчик значений из ЖПК " });

                #endregion

                #region Тип шаблона

                T.TypeTemplate = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeTemplate", "Тип шаблона");
                T.TypeTemplate.Columns.AddString("Name", "Наименование", 20);
                G.TypeTemplate = T.TypeTemplate.CreateSubTable();

                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.ActSelection, new object[] { "АКТ отбора сточной воды" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.LetterNotification, new object[] { "Письмо - уведомление" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.Extract, new object[] { "Выписка" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.CalculationFees, new object[] { "Расчёт платы" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.Registry, new object[] { "Реестр" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.Protocol, new object[] { "Протокол испытаний" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.InspectionResult, new object[] { "Результат контроля" });
                G.TypeTemplate.Rows.Add((uint)data.ETypeTemplate.Journal, new object[] { "Журнал" });

                #endregion

                #region Тип клиента

                T.TypeClient = data.T1.Tables.Add(Encoding.GetEncoding(866), "TypeClient", "Тип клиента");
                T.TypeClient.Columns.AddString("Name", "Наименование", 10);
                T.TypeClient.Columns.AddString("InCase", "В падежах", 250);
                G.TypeClient = T.TypeClient.CreateSubTable();

                G.TypeClient.Rows.Add((uint)data.ETypeClient.Individual, new object[] { "ИП",
                    "Индивидуальный предприниматель" +
                    "#Индивидуального предпринимателя" +
                    "#Индивидуальному предпринимателю" +
                    "#Индивидуального предпринимателя" +
                    "#Индивидуальным предпринимателем" +
                    "#Индивидуальном предпринимателе" });
                G.TypeClient.Rows.Add((uint)data.ETypeClient.Legal, new object[] { "Юр лицо",
                    "Руководитель" +
                    "#Руководителя" +
                    "#Руководителю" +
                    "#Руководителя" +
                    "#Руководителем" +
                    "#Руководителе"});
                G.TypeClient.Rows.Add((uint)data.ETypeClient.Physical, new object[] { "Физ лицо",
                    "Физическое лицо" +
                    "#Физического лица" +
                    "#Физическому лицу" +
                    "#Физическое лицо" +
                    "#Физическим лицом" +
                    "#Физическом лице"});

                #endregion
            }

            if (!AddRemote(Loading, "User", "Пользователь", ref T.User, ref G.User,
                newTable =>
                {
                    newTable.Columns.AddString("Login", "Логин", 55);
                    newTable.Columns.AddString("Pass", "Пароль", 25, DataBase.EColLocation.Remote);
                    newTable.Columns.AddRelation(T.UType, "Name");
                    newTable.Columns.AddString("PCName", "Имя компьютера", 50, true);
                    newTable.Columns.AddString("PCUser", "Имя пользователя", 50, true);
                    newTable.Columns.AddInt32("CPeriod", "Текущая  дата в месяцах", DataBase.ETypeView.YMT); //Месяц
                    newTable.Columns.AddString("Job", "Должность", 150);
                    newTable.Columns.AddString("Surename", "Фамилия", 150);
                    newTable.Columns.AddString("Name", "Имя", 150);
                    newTable.Columns.AddString("Patronymic", "Отчество", 150);
                    newTable.Columns.AddBool("CanRedact", "Доступно редактирование", DataBase.EColLocation.Local, true, true);

                    newTable.Columns.AddAutoUpdate("IsHere", "Используется", DataBase.EColLocation.Remote, DataBase.ETypeView.boolT);
                    newTable.Columns.AddString("Mail", "Почта", 50);
                    newTable.Columns.AddBool("Enabled", "Разрешено использовать", DataBase.EColLocation.Remote, false, true);
                    newTable.Columns.AddString("Cause", "Причина закрытия", 55, DataBase.EColLocation.Remote, false, string.Empty);
                    newTable.Columns.AddString("PrgVer", "Версия программы", 25);

                    newTable.Columns.Add_Unique("Login");
                }, false)) { return; }
            T.User.Rows.SetEditForm(C.User.GetEdit);
            T.User.Rows.SetAddForm(C.User.GetEdit);
        }
    }
}
