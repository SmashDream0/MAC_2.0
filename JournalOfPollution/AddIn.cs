using System.Windows.Forms;

/// <summary>Общие объекты управления, настройки программы и база данных</summary>
public static class data
{
    /// <summary>Параметры коммандной строки</summary>
    public struct CMD
    {
        /// <summary>Разрешено менять таблицы, если они не соответствуют ожиданиям</summary>
        public readonly static string AlowToChange = "alowtochange=";
        public readonly static string SettingsFile = "settingsfile=";
        public readonly static string SetIncrem = "setincrem=";
        public readonly static string DeleteMe = "deleteme=";
    }

    public static DataBase T1;

    static uint _UserID;
    public static uint UserID
    {
        get { return _UserID; }
        set
        {
            if (PrgSettings != null)
            { _UserID = PrgSettings.Values[(int)Strings.LastUser].UInt = value; }
            else
            { _UserID = value; }
        }
    }

    /// <summary>Инкрементный множитель таблиц</summary>
    public static int Increm = -1;
    public static bool DeleteConf = false;
    public static string StName = "asettings";
    public static bool AllowModify;

    #region Настройки

    public static Settings PrgSettings;

    #region Получение

    public static int GetInt32Settings(Strings param)
    { return PrgSettings.Values[(int)param].Int; }

    public static string GetStringSettings(Strings param)
    { return PrgSettings.Values[(int)param].String; }

    public static bool GetBooleanSettings(Strings param)
    { return PrgSettings.Values[(int)param].Bool; }

    #endregion

    #region Занесение
    
    public static void SetSettings(Strings param, int value)
    { PrgSettings.Values[(int)param].Int = value; }

    public static void SetSettings(Strings param, string value)
    { PrgSettings.Values[(int)param].String = value; }

    public static void SetSettings(Strings param, bool value)
    { PrgSettings.Values[(int)param].Bool = value; }

    #endregion

    public enum Strings : byte
    {
        UseSQL,
        SqlIp,
        SqlIp1,
        SqlIp2,
        SqlIpLast,
        SqlLogin,
        SqlPassword,
        DATABASE,
        SqlPort,
        SMTPAdress,
        SMTPPort,
        SMTPUseSSL,
        MailIP,
        IMAPAdress,
        IMAPPort,
        MailLogin,
        MailPass,
        Changes,
        LastUser,
        UseErorLog,
        UseActionLog,
        UseSimpleLog,
    };

    #endregion

    public static readonly string EmaleSubjectPart = "JCalc";
    public static readonly string EmaleSubject = "Обновление " + EmaleSubjectPart;
    public static readonly string SendEmaleSubject = EmaleSubject.Remove(EmaleSubject.Length - EmaleSubjectPart.Length - 1, 1).ToLower();

    public enum Forms : byte { AdminPanel, Settings, Employe };
    public static DataBase.RemoteType DataSourceType { get { return (DataBase.RemoteType)PrgSettings.Values[(int)Strings.UseSQL].Int; } }

    public enum UType : byte
    {
        None = 0,
        /// <summary>Админ</summary>
        Admin = 1,
        /// <summary>Ответственный</summary>
        MainWork = 2,
        /// <summary>Лаборант</summary>
        Laborant = 3,
        /// <summary>Просмотрщик</summary>
        Browser = 4 
    };
    public enum VarType : byte { None = 0, dbl = 1, i32 = 2, Bool = 3 };
    public enum RType : byte { None = 0, List = 1, Scheme = 2 };
    public enum TLocation : byte { None = 0, Top = 1, Left = 2, Bottom = 3, Right = 4 };
    /// <summary>статус</summary>
    public enum EStatus
    {
        None = 0,
        /// <summary>отобран</summary>
        Selected = 1,
        /// <summary>закрыт</summary>
        Close = 2,
        /// <summary>сумма меньше лимита</summary>
        NotLimit = 3,
        /// <summary>без объёма</summary>
        NotVolume = 4
    }
    /// <summary>вид пробы</summary>
    public enum ETypeSample
    {
        None = 0,
        /// <summary>простая</summary>
        Simple = 1,
        /// <summary>смешанная</summary>
        Mixed = 2
    }
    /// <summary>вид колодца</summary>
    public enum ETypeWell
    {
        None = 0,
        /// <summary>контрольный канализационный колодец абонента</summary>
        KK = 1,
        /// <summary>КНС абонента</summary>
        KNS = 2,
        /// <summary>последний колодец на сети абонента</summary>
        PKSA = 3,
        /// <summary>приёмная камера</summary>
        PK = 4
    }
    /// <summary>тип нормы</summary>
    public enum ETypeConstruction
    {
        None = 0,
        /// <summary>канализация</summary>
        Sewer = 1
    }
    /// <summary>получаемое значение</summary>
    public enum EGettingValue
    {
        None = 0,
        /// <summary>Среднее</summary>
        Medium = 1,
        /// <summary>Максимальное</summary>
        Max = 2,
        /// <summary>Сумма</summary>
        Summ = 3
    }
    /// <summary>Тип постановления</summary>
    public enum ETypeResolution
    {
        None = 0,
        /// <summary>Ставка/Стоимость</summary>
        Cost = 1,
        /// <summary>Нормативы/Значания</summary>
        Norm = 2,
        /// <summary>Нормативы/Ставка</summary>
        CostNorm = 3,
    }
    /// <summary>Тип инструкции</summary>
    public enum ETypeInstruction
    {
        None = 0,
        /// <summary>АДМИНИСТРАТОР</summary>
        Admin = 1,
        /// <summary>Основная форма</summary>
        DefWindow = 2,
        /// <summary>Занечение значений</summary>
        EditorValue = 3,
        /// <summary>Редактирование клиента</summary>
        EditorClient = 4,
        /// <summary>Загрузчик объёмов</summary>
        LoadVolume = 5,
        /// <summary>Загрузчик значений из ЖПК</summary>
        LoadSyncJPC = 6
    }
    /// <summary>Тип инструкции</summary>
    public enum ETypeTemplate
    {
        None = 0,
        /// <summary>Акт отбора сточных вод</summary>
        ActSelection = 1,
        /// <summary>Письмо - уведомление</summary>
        LetterNotification = 2,
        /// <summary>Выписка</summary>
        Extract = 3,
        /// <summary>Расчёт платы</summary>
        CalculationFees = 4,
        /// <summary>Реестр</summary>
        Registry = 5,
        /// <summary>Протокол</summary>
        Protocol = 6,
        /// <summary>Результат контроля</summary>
        InspectionResult = 7,
        /// <summary>Журнал</summary>
        Journal = 8,
    }
    /// <summary>Тип клиента</summary>
    public enum ETypeClient
    {
        None = 0,
        /// <summary>ИП</summary>
        Individual = 1,
        /// <summary>Юр лицо</summary>
        Legal = 2,
        /// <summary>Физ лицо</summary>
        Physical= 3,
    }
    public static ColumnType User<ColumnType>(int ColumnIndex) { return T.User.Rows.Get_UnShow<ColumnType>(data.UserID, ColumnIndex); }

    public static ColumnType User<ColumnType>(int ColumnIndex, params int[] RelationPath) { return T.User.Rows.Get_UnShow<ColumnType>(data.UserID, ColumnIndex, RelationPath); }

    public static void User<ColumnType>(int ColumnIndex, ColumnType Value) { T.User.Rows.Set(data.UserID, ColumnIndex, Value); }
}
/// <summary>Таблицы</summary>
public static class T
{
    public static DataBase.ITable

    #region пользователь
        UType,
        User,
    #endregion

    #region дополнительные первостепенные таблицы
    /// <summary>тип клиента</summary>
    TypeClient,
    /// <summary>период</summary>
    Period,
    /// <summary>район</summary>
    DistrictReference,
    /// <summary>адрес</summary>
    AdresReference,
    /// <summary>вид пробы</summary>
    TypeSample,
    /// <summary>вид колодца</summary>
    TypeWell,
    /// <summary>должность</summary>
    Post,
    /// <summary>сотрудник</summary>
    Worker,
    /// <summary>подписывающие</summary>
    RatioSigner,
    /// <summary>представитель</summary>
    Representative,
    /// <summary>статус</summary>
    Status,
    /// <summary>единицы измерения</summary>
    Units,
    /// <summary>тип нормы</summary>
    TypeConstruction,
    /// <summary>подразделение</summary>
    Unit,
    ///// <summary>Подразделение к типу колодца</summary>
    //UnitToTypeWell,
    /// <summary>Получаемое значение</summary>
    GettingValue,
    /// <summary>Тип постановления</summary>
    TypeResolution,
    #endregion

    #region клиент, объект, колодец
    /// <summary>клиент</summary>
    Client,
    /// <summary>реквизиты клиента</summary>
    DetailsClient,
    /// <summary>объект</summary>
    Objecte,
    /// <summary>реквизиты объекта</summary>
    DetailsObject,
    /// <summary>колодец</summary>
    Well,
    #endregion

    #region загрязнение, норматив, декларация
    /// <summary>загрязнение</summary>
    Pollution,
    /// <summary>точность измерений</summary>
    AccurateMeasurement,
    /// <summary>норматив</summary>
    Resolution,
    /// <summary>реквизиты норматива</summary>
    ResolutionClarify,
    /// <summary>значение нормы</summary>
    ValueNorm,
    /// <summary>стоимость нормы</summary>
    PriceNorm,
    /// <summary>декларация</summary>
    Declaration,
    /// <summary>значения декларации</summary>
    DeclarationValue,
    /// <summary>виды загрязнений</summary>
    HitModePollution,
    /// <summary>фактическое загрязнение</summary>
    HitSelectPollution,
    /// <summary>коэффициент</summary>
    Coefficient,
    /// <summary>значения коэффициента</summary>
    CoefficientValue,
    /// <summary>формула расчёта</summary>
    CalculationFormula,
    #endregion

    #region проба, отбор
    /// <summary>проба</summary>
    Sample,
    /// <summary>объёмы</summary>
    Volume,
    /// <summary>отбор колодца</summary>
     SelectionWell,
    /// <summary>значения отбора</summary>
     ValueSelection,
    #endregion

    #region дополнительные второстепенные таблицы
        /// <summary>отношение объект к постановлению</summary>
        ObjectFromResolution,
        /// <summary>Нормативные документы</summary>
        NormDoc,
        /// <summary>среднемесячный объём</summary>
        MidMonthVolume,
        /// <summary>помощник согласования</summary>
        NegotiationAssistant,
        /// <summary>Аккредитация</summary>
        Accredit,
    #endregion

    #region Вспомогательный материал
        /// <summary>Тип инструкции</summary>
        TypeInstruction,
        /// <summary>Инструкции</summary>
        StorageInstructions,
        /// <summary>Тип инструкции</summary>
        TypeTemplate,
        /// <summary>Шаблоны для печати</summary>
        TemplatePrint,
        /// <summary>Текст для шаблона</summary>
        TextFromTemplate;
    #endregion

}

/// <summary>Уникальные табличные представления</summary>
public static class G
{
    public static DataBase.ISTable

    #region пользователь
        UType,
        User,
    #endregion

    #region дополнительные первостепенные таблицы
    /// <summary>тип клиента</summary>
    TypeClient,
    /// <summary>период</summary>
     Period,
    /// <summary>район</summary>
     DistrictReference,
    /// <summary>адрес</summary>
     Adres,
    /// <summary>вид пробы</summary>
     TypeSample,
    /// <summary>вид колодца</summary>
     TypeWell,
    /// <summary>должность</summary>
     Post,
    /// <summary>сотрудник</summary>
    Worker,
    /// <summary>подписывающие</summary>
    RatioSigner,
    /// <summary>представитель</summary>
    Representative,
    /// <summary>статус</summary>
    Status,
    /// <summary>единицы измерения</summary>
    Units,
    /// <summary>тип нормы</summary>
    TypeConstruction,
    /// <summary>подразделение</summary>
    Unit,
    ///// <summary>Подразделение к типу колодца</summary>
    //UnitToTypeWell,
    /// <summary>Получаемое значение</summary>
    GettingValue,
    /// <summary>Тип постановления</summary>
    TypeResolution,
    #endregion

    #region клиент, объект, колодец
    /// <summary>клиент</summary>
    Client,
    /// <summary>реквизиты клиента</summary>
    DetailsClient,
    /// <summary>объект</summary>
    Objecte,
    /// <summary>реквизиты объекта</summary>
    DetailsObject,
    /// <summary>колодец</summary>
    Well,
    #endregion

    #region загрязнение, норматив, декларация
    /// <summary>загрязнение</summary>
    Pollution,
    /// <summary>точность измерений</summary>
    AccurateMeasurement,
    /// <summary>норматив</summary>
    Resolution,
    /// <summary>реквизиты норматива</summary>
    ResolutionClarify,
    /// <summary>значение нормы</summary>
    ValueNorm,
    /// <summary>стоимость нормы</summary>
    PriceNorm,
    /// <summary>декларация</summary>
    Declaration,
    /// <summary>значения декларации</summary>
    DeclarationValue,
    /// <summary>виды загрязнений</summary>
    HitModePollution,
    /// <summary>фактическое загрязнение</summary>
    HitSelectPollution,
    /// <summary>коэффициент</summary>
    Coefficient,
    /// <summary>значения коэффициента</summary>
    CoefficientValue,
    /// <summary>формула расчёта</summary>
    CalculationFormula,
    #endregion

    #region проба, отбор
    /// <summary>проба</summary>
    Sample,
    /// <summary>объёмы</summary>
    Volume,
    /// <summary>отбор колодца</summary>
     SelectionWell,
    /// <summary>значения отбора</summary>
     ValueSelection,
    #endregion

    #region дополнительные второстепенные таблицы
        /// <summary>отношение объект к постановлению</summary>
        ObjectFromResolution,
        /// <summary>Нормативные документы</summary>
        NormDoc,
        /// <summary>среднемесячный объём</summary>
        MidMonthVolume,
        /// <summary>помощник согласования</summary>
        NegotiationAssistant,
        /// <summary>Аккредитация</summary>
        Accredit,
    #endregion

    #region Вспомогательный материал
    /// <summary>Тип инструкции</summary>
    TypeInstruction,
        /// <summary>Инструкции</summary>
        StorageInstructions,
        /// <summary>Тип инструкции</summary>
        TypeTemplate,
        /// <summary>Шаблоны для печати</summary>
        TemplatePrint,
        /// <summary>Текст для шаблона</summary>
        TextFromTemplate;
    #endregion

}

/// <summary>Колонки таблиц</summary>
public static class C
{
    #region пользователь
    /// <summary>тип пользователя</summary>
    public struct UType
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>пользователь</summary>
    public struct User
    {
        /// <summary>имя учетной записи</summary>
        public const byte Login = 0;
        /// <summary>пароль</summary>
        public const byte Pass = Login + 1;
        /// <summary>тип учетной записи</summary>
        public const byte UType = Pass + 1;
        /// <summary>имя компьютера</summary>
        public const byte PCName = UType + 1;
        /// <summary>имя пользователя учетки компьютера</summary>
        public const byte PCUser = PCName + 1;
        /// <summary>текущая дата</summary>
        public const byte CPeriod = PCUser + 1;
        /// <summary>должность</summary>
        public const byte Job = CPeriod + 1;
        /// <summary>фамилия</summary>
        public const byte SureName = Job + 1;
        /// <summary>имя</summary>
        public const byte Name = SureName + 1;
        /// <summary>отчество</summary>
        public const byte Patronymic = Name + 1;
        /// <summary>доступ редактирования</summary>
        public const byte CanRedact = Patronymic + 1;
        /// <summary>используется-ли учетная запись</summary>
        public const byte IsHere = CanRedact + 1;
        /// <summary>электронная почта</summary>
        public const byte Mail = IsHere + 1;
        /// <summary>разрешено-ли использовать учетную запись</summary>
        public const byte Enabled = Mail + 1;
        /// <summary>описание причины блокировки</summary>
        public const byte Cause = Enabled + 1;
        /// <summary>версия программы</summary>
        public const byte PrgVer = Cause + 1;

        public static Form GetEdit(DataBase.ISTable SubTable, uint ID, int Width, bool Virtual)
        {
            var UEF = new Edit_Form(SubTable, ID, Width, Virtual);

            var Enabled = T.User.Rows.Get_UnShow<uint>(data.UserID, C.User.UType) == (uint)data.UType.Admin;

            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Login));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Pass, true, null, true));
            UEF.AddControl(new Edit_Form.RelationCombo_class(UEF, C.User.UType, Enabled));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Mail));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Job));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.SureName));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Name));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Patronymic));

            if (Enabled)
            {
                UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.PCName));
                UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.PCUser));
            }

            UEF.AddControl(C.User.CPeriod);

            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.IsHere, Enabled));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Enabled, Enabled));

            return UEF;
        }
        public static Form GetEdit(DataBase.ISTable SubTable, int Width, bool Virtual)
        {
            var UEF = new Edit_Form(SubTable, Width, Virtual);

            var Enabled = T.User.Rows.Get_UnShow<uint>(data.UserID, C.User.UType) == (uint)data.UType.Admin;

            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Login));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Pass, true, null, true));
            UEF.AddControl(new Edit_Form.RelationCombo_class(UEF, C.User.UType, Enabled));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Mail));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Job));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.SureName));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Name));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Patronymic));

            if (Enabled)
            {
                UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.PCName));
                UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.PCUser));
            }

            UEF.AddControl(C.User.CPeriod);

            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.IsHere, Enabled));
            UEF.AddControl(new Edit_Form.Inputs_class(UEF, C.User.Enabled, Enabled));

            return UEF;
        }
    }

    #endregion

    #region дополнительные первостепенные таблицы
    /// <summary>тип клиента</summary>
    public struct TypeClient
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
        /// <summary>в падежах</summary>
        public const byte InCase = Name + 1;
    }
    /// <summary>период</summary>
    public struct Period
    {
        /// <summary>месяц</summary>
        public const byte YM = 0;
        /// <summary>тариф</summary>
        public const byte Price = YM + 1;
        /// <summary>ндс</summary>
        public const byte NDS = Price + 1;
        /// <summary>минимальный лимит</summary>
        public const byte MinLimits = NDS + 1;
        /// <summary>Норма кол-ва проб в день</summary>
        public const byte SamplesMonthLimit = MinLimits + 1;
    }
    /// <summary>район</summary>
    public struct DistrictReference
    {
        /// <summary>район</summary>
        public const byte Name = 0;
        /// <summary>порядок</summary>
        public const byte Order = Name + 1;
    }
    /// <summary>адрес</summary>
    public struct AdresReference
    {
        /// <summary>адрес</summary>
        public const byte Adres = 0;
        /// <summary>район</summary>
        public const byte District = Adres + 1;
    }
    /// <summary>вид пробы</summary>
    public struct TypeSample
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>вид колодца</summary>
    public struct TypeWell
    {
        /// <summary>краткое наименование</summary>
        public const byte CurtName = 0;
        /// <summary>полное наименование</summary>
        public const byte FullName = CurtName + 1;
    }
    /// <summary>должность</summary>
    public struct Post
    {
        /// <summary>краткое наименование</summary>
        public const byte CurtName = 0;
        /// <summary>полное наименование</summary>
        public const byte FullName = CurtName + 1;
    }
    /// <summary>сотрудник</summary>
    public struct Worker
    {
        /// <summary>фамилия</summary>
        public const byte SureName = 0;
        /// <summary>имя</summary>
        public const byte Name = SureName + 1;
        /// <summary>отчество</summary>
        public const byte Patronymic = Name + 1;
        /// <summary>должность</summary>
        public const byte Post = Patronymic + 1;
        /// <summary>Месяц от</summary>
        public const byte YMFrom = Post + 1;
        /// <summary>Месяц до</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>Пользователь</summary>
        public const byte User = YMTo + 1;
    }
    /// <summary>подписывающие/для отбора</summary>
    public struct RatioSigner
    {
        /// <summary>тип шаблона</summary>
        public const byte TypeTemplate = 0;
        /// <summary>должность</summary>
        public const byte Post = TypeTemplate + 1;
        /// <summary>работник</summary>
        public const byte Worker = Post + 1;
        /// <summary>лист</summary>
        public const byte List = Worker + 1;
        /// <summary>положение</summary>
        public const byte Position = List + 1;
    }
    /// <summary>представитель</summary>
    public struct Representative
    {
        /// <summary>фио</summary>
        public const byte FIO = 0;
        /// <summary>client</summary>
        public const byte Client = FIO + 1;
        /// <summary>должность</summary>
        public const byte Post = Client + 1;
    }
    /// <summary>статус</summary>
    public struct Status
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>единицы измерения</summary>
    public struct Units
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>тип сооружения</summary>
    public struct TypeConstruction
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>получаемое значение</summary>
    public struct GettingValue
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>тип постановления</summary>
    public struct TypeResolution
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>тип шаблона</summary>
    public struct TypeTemplate
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
    }
    /// <summary>Подразделение</summary>
    public struct Unit
    {
        /// <summary>тип сооружения</summary>
        public const byte TypeConstruction = 0;
        /// <summary>наименование</summary>
        public const byte Name = TypeConstruction + 1;
        /// <summary>полное наименование</summary>
        public const byte FullName = Name + 1;
        /// <summary>адрес</summary>
        public const byte Contact = FullName + 1;
        /// <summary>номер подразделения</summary>
        public const byte Podr = Contact + 1;
    }
    ///// <summary>Подразделение к типу колодца</summary>
    //public struct UnitToTypeWell
    //{
    //    /// <summary>подразделение</summary>
    //    public const byte Unit = 0;
    //    /// <summary>тип колодца</summary>
    //    public const byte TypeWell = Unit + 1;
    //}
    #endregion

    #region клиент, объект, колодец
    /// <summary>клиент</summary>
    public struct Client
    {
        /// <summary>ИНН</summary>
        public const byte INN = 0;
        /// <summary>можно отбирать</summary>
        public const byte CanSelect = INN + 1;
        /// <summary>обязан подать декларацию</summary>
        public const byte MustDeclair = CanSelect + 1;
        /// <summary>дата создания в месяцах</summary>
        public const byte YMFrom = MustDeclair + 1;
        /// <summary>дата закрытия в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>тип клиента</summary>
        public const byte TypeClient = YMTo + 1;
    }
    /// <summary>реквизиты клиента</summary>
    public struct DetailsClient
    {
        /// <summary>клиент</summary>
        public const byte Client = 0;
        /// <summary>полное наименование</summary>
        public const byte FullName = Client + 1;
        /// <summary>почтовый адрес</summary>
        public const byte AdresP = FullName + 1;
        /// <summary>юридический адрес</summary>
        public const byte AdresUr = AdresP + 1;
        /// <summary>дата действия с в месяцах</summary>
        public const byte YM = AdresUr + 1;
        /// <summary>наименование в дательном падеже</summary>
        public const byte NameInDative = YM + 1;
    }
    /// <summary>объект</summary>
    public struct Objecte
    {
        /// <summary>клиент</summary>
        public const byte Client = 0;
        /// <summary>номер папки</summary>
        public const byte NumberFolder = Client + 1;
        /// <summary>адрес фактический</summary>
        public const byte AdresFact = NumberFolder + 1;
        /// <summary>вид пробы</summary>
        public const byte TypeSample = AdresFact + 1;
        /// <summary>дата создания в месяцах</summary>
        public const byte YMFrom = TypeSample + 1;
        /// <summary>дата закрытия в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>лицевые из 1С</summary>
        public const byte Account1C = YMTo + 1;
        /// <summary>обособленный</summary>
        public const byte Separate = Account1C + 1;
    }
    /// <summary>реквизиты объекта</summary>
    public struct DetailsObject
    {
        /// <summary>объект</summary>
        public const byte Object = 0;
        /// <summary>дата действия с в месяцах</summary>
        public const byte YM = Object + 1;
        /// <summary>добавочное имя</summary>
        public const byte AddName = YM + 1;
        /// <summary>юридический фактический</summary>
        public const byte LegalAdresReference = AddName + 1;
        /// <summary>почтовый фактический</summary>
        public const byte MailAdresReference = LegalAdresReference + 1;
    }
    /// <summary>колодец</summary>
    public struct Well
    {
        /// <summary>объект</summary>
        public const byte Object = 0;
        /// <summary>тип колодца</summary>
        public const byte TypeWell = Object + 1;
        /// <summary>номер колодца</summary>
        public const byte Number = TypeWell + 1;
        /// <summary>дата создания в месяцах</summary>
        public const byte YMFrom = Number + 1;
        /// <summary>дата закрытия в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>подразделение</summary>
        public const byte Unit = YMTo + 1;
    }
    #endregion

    #region загрязнение, норматив, декларация
    /// <summary>загрязнение</summary>
    public struct Pollution
    {
        /// <summary>краткое наименование</summary>
        public const byte CurtName = 0;
        /// <summary>полное наименование</summary>
        public const byte FullName = CurtName + 1;
        /// <summary>код</summary>
        public const byte Key = FullName + 1;
        /// <summary>единицы измерения</summary>
        public const byte Units = Key + 1;
        /// <summary>точность</summary>
        public const byte Round = Units + 1;
        /// <summary>номер по порядку</summary>
        public const byte Number = Round + 1;
        /// <summary>диапозонное значение</summary>
        public const byte HasRange = Number + 1;
        /// <summary>дата создания в месяцах</summary>
        public const byte YMFrom = HasRange + 1;
        /// <summary>дата закрытия в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>показывать в основном окне</summary>
        public const byte Show = YMTo + 1;
        /// <summary>уникальный ключ в рамках всех программ</summary>
        public const byte UniqueKey = Show + 1;
        /// <summary>методика</summary>
        public const byte Method = UniqueKey + 1;
    }
    /// <summary>точность измерений</summary>
    public struct AccurateMeasurement
    {
        /// <summary>загрязнение</summary>
        public const byte Pollution = 0;
        /// <summary>от</summary>
        public const byte From = Pollution + 1;
        /// <summary>до</summary>
        public const byte To = From + 1;
        /// <summary>значение</summary>
        public const byte Number = To + 1;
        /// <summary>проценты</summary>
        public const byte IsPercent = Number + 1;
        /// <summary>дата действия с в месяцах</summary>
        public const byte YM = Number + 1;
    }
    /// <summary>постановление</summary>
    public struct Resolution
    {
        /// <summary>краткое наименование</summary>
        public const byte CurtName = 0;
        /// <summary>название из 1С</summary>
        public const byte NameOf1C = CurtName + 1;
        /// <summary>дата создания в месяцах</summary>
        public const byte YMFrom = NameOf1C + 1;
        /// <summary>дата закрытия в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>тип нормы</summary>
        public const byte TypeConstruction = YMTo + 1;
        /// <summary>выключен принудительно</summary>
        public const byte OffForce = TypeConstruction + 1;
        /// <summary>цвет</summary>
        public const byte Color = OffForce + 1;
    }
    /// <summary>актуализация постановления</summary>
    public struct ResolutionClarify
    {
        /// <summary>номер постановления</summary>
        public const byte Number = 0;
        /// <summary>норматив</summary>
        public const byte Resolution = Number + 1;
        /// <summary>полное наименование</summary>
        public const byte FullName = Resolution + 1;
        /// <summary>описание</summary>
        public const byte Note = FullName + 1;
        /// <summary>акты</summary>
        public const byte Acts = Note + 1;
        /// <summary>действует с в месяцах</summary>
        public const byte YMFrom = Acts + 1;
        /// <summary>действует по в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>тип постановления</summary>
        public const byte TypeResolution = YMTo + 1;
    }
    /// <summary>значение нормы</summary>
    public struct ValueNorm
    {
        /// <summary>актуализация постановления</summary>
        public const byte ResolutionClarify = 0;
        /// <summary>загрязнение</summary>
        public const byte Pollution = ResolutionClarify + 1;
        /// <summary>от</summary>
        public const byte From = Pollution + 1;
        /// <summary>до</summary>
        public const byte To = From + 1;
        /// <summary>подразделение</summary>
        public const byte Unit = To + 1;
        /// <summary>множитель</summary>
        public const byte Multiplier = Unit + 1;
        /// <summary>Время действия от</summary>
        public const byte YMFrom = Multiplier + 1;
        /// <summary>Время действия до</summary>
        public const byte YMTo = YMFrom + 1;
    }
    /// <summary>стоимость нормы</summary>
    public struct PriceNorm
    {
        /// <summary>актуализация постановления</summary>
        public const byte ResolutionClarify = 0;
        /// <summary>загрязнение</summary>
        public const byte Pollution = ResolutionClarify + 1;
        /// <summary>стоимоть</summary>
        public const byte Price = Pollution + 1;
        /// <summary>множитель как коэффициент воздействия</summary>
        public const byte MultiAs = Price + 1;
        /// <summary>Время действия от</summary>
        public const byte YMFrom = MultiAs + 1;
        /// <summary>Время действия до</summary>
        public const byte YMTo = YMFrom + 1;
    }
    /// <summary>декларация</summary>
    public struct Declaration
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
        /// <summary>колодец</summary>
        public const byte Well = Name + 1;
        /// <summary>действует с в месяцах</summary>
        public const byte YM = Well + 1;
    }
    /// <summary>значения декларации</summary>
    public struct DeclarationValue
    {
        /// <summary>декларация</summary>
        public const byte Declaration = 0;
        /// <summary>загрязнение</summary>
        public const byte Pollution = Declaration + 1;
        /// <summary>от</summary>
        public const byte From = Pollution + 1;
        /// <summary>до</summary>
        public const byte To = From + 1;
    }
    /// <summary>Виды загрязнений</summary>
    public struct HitModePollution
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
        /// <summary>примечание</summary>
        public const byte Note = Name + 1;
        /// <summary>множитель</summary>
        public const byte Multiplier = Note + 1;
        /// <summary>срок отслеживания в месяцах</summary>
        public const byte MonthTrack = Multiplier + 1;
        /// <summary>месяц начала отслеживания</summary>
        public const byte MonthStartTack = MonthTrack + 1;
        /// <summary>косяк</summary>
        public const byte Joint = MonthStartTack + 1;
        /// <summary>формула</summary>
        public const byte Formula = Joint + 1;
        /// <summary>загрязнение</summary>
        public const byte Pollution = Formula + 1;
        /// <summary>постановление</summary>
        public const byte Resolution = Pollution + 1;
        /// <summary>Может отбираться единолично</summary>
        public const byte CanOnly = Resolution + 1;
        /// <summary>формула наказания</summary>
        public const byte FormulaPunishment = CanOnly + 1;
    }
    /// <summary>коэффициент</summary>
    public struct Coefficient
    {
        /// <summary>норматив</summary>
        public const byte Resolution = 0;
        /// <summary>загрязненеи</summary>
        public const byte Pollution = Resolution + 1;
        /// <summary>действует с в месяцах</summary>
        public const byte YMFrom = Pollution + 1;
        /// <summary>действует до в месяцах</summary>
        public const byte YMTo = YMFrom + 1;
        /// <summary>для сравнения</summary>
        public const byte Compare = YMTo + 1;
    }
    /// <summary>значения коэффициента</summary>
    public struct CoefficientValue
    {
        /// <summary>коэффициент</summary>
        public const byte Coefficient = 0;
        /// <summary>от</summary>
        public const byte From = Coefficient + 1;
        /// <summary>до</summary>
        public const byte To = From + 1;
        /// <summary>коэффициент</summary>
        public const byte Value = To + 1;
    }
    /// <summary>формула расчёта</summary>
    public struct CalculationFormula
    {
        /// <summary>норматив</summary>
        public const byte ResolutionClarify = 0;
        /// <summary>загрязнение</summary>
        public const byte Pollution = ResolutionClarify + 1;
        /// <summary>номер</summary>
        public const byte Number = Pollution + 1;
        /// <summary>формула</summary>
        public const byte Formula = Number + 1;
        /// <summary>получаемое значение</summary>
        public const byte GettingValueFormula = Formula + 1;
        /// <summary>формула расчёта ссылка</summary>
        public const byte CalculationFormulaLink = GettingValueFormula + 1;
        /// <summary>получаемое значение ссылки</summary>
        public const byte GettingValueLink = CalculationFormulaLink + 1;
        /// <summary>действует с в месяцах</summary>
        public const byte YM = GettingValueLink + 1;
        /// <summary>формула по умолчанию</summary>
        public const byte DefFormula = YM + 1;
        /// <summary>метка</summary>
        public const byte Label = DefFormula + 1;
    }
    #endregion

    #region проба, отбор
    /// <summary>проба</summary>
    public struct Sample
    {
        /// <summary>месяц создания</summary>
        public const byte YM = 0;
        /// <summary>сотрудник</summary>
        public const byte Worker = YM + 1;
        /// <summary>представитель</summary>
        public const byte Representative = Worker + 1;
        /// <summary>статус</summary>
        public const byte Status = Representative + 1;
    }
    /// <summary>объёмы</summary>
    public struct Volume
    {
        /// <summary>проба</summary>
        public const byte Sample = 0;
        /// <summary>объём</summary>
        public const byte Value = Sample + 1;
        /// <summary>период</summary>
        public const byte Period = Value + 1;
    }
    /// <summary>отбор колодца</summary>
    public struct SelectionWell
    {
        /// <summary>колодец</summary>
        public const byte Well = 0;
        /// <summary>проба</summary>
        public const byte Sample = Well + 1;
        /// <summary>номер</summary>
        public const byte Number = Sample + 1;
        /// <summary>дата время в минутах</summary>
        public const byte DateTime = Number + 1;
    }
    /// <summary>значения отбора</summary>
    public struct ValueSelection
    {
        /// <summary>колодец</summary>
        public const byte SelectionWell = 0;
        /// <summary>загрязнение</summary>
        public const byte Pollution = SelectionWell + 1;
        /// <summary>значение</summary>
        public const byte Value = Pollution + 1;
    }
    /// <summary>фактическое загрязнение</summary>
    public struct HitSelectPollution
    {
        /// <summary>тип загрязнения</summary>
        public const byte HitModePollution = 0;
        /// <summary>Объект</summary>
        public const byte Object = HitModePollution + 1;
        /// <summary>колличество попадения</summary>
        public const byte Number = Object + 1;
        /// <summary>Месяц фиксации</summary>
        public const byte YM = Number + 1;
        /// <summary>отобран единолично</summary>
        public const byte Only = YM + 1;
    }
    #endregion

    #region дополнительные второстепенные таблицы
    /// <summary>отношение объект к постановлению</summary>
    public struct ObjectFromResolution
    {
        /// <summary>постановление</summary>
        public const byte Resolution = 0;
        /// <summary>объект</summary>
        public const byte Object = Resolution + 1;
        /// <summary>причина</summary>
        public const byte Reason = Object + 1;
        /// <summary>применение</summary>
        public const byte Application = Reason + 1;
    }
    /// <summary>нормативные документы</summary>
    public struct NormDoc
    {
        /// <summary>акт</summary>
        public const byte Act = 0;
        /// <summary>счет</summary>
        public const byte Score = Act + 1;
        /// <summary>счет-фактура</summary>
        public const byte Invoces = Score + 1;
        /// <summary>дата в днях</summary>
        public const byte Date = Invoces + 1;
        /// <summary>сумма</summary>
        public const byte Summ = Date + 1;
        /// <summary>постановление</summary>
        public const byte Resolution = Summ + 1;
        /// <summary>объём</summary>
        public const byte Volume = Resolution + 1;
    }
    /// <summary>среднемесячный объём</summary>
    public struct MidMonthVolume
    {
        /// <summary>объект</summary>
        public const byte Objecte = 0;
        /// <summary>год</summary>
        public const byte Year = Objecte + 1;
        /// <summary>объём</summary>
        public const byte Volume = Year + 1;
    }
    /// <summary>помошник согласования</summary>
    public struct NegotiationAssistant
    {
        /// <summary>объект</summary>
        public const byte Objecte = 0;
        /// <summary>отбор</summary>
        public const byte Sample = Objecte + 1;
        /// <summary>месяц</summary>
        public const byte YM = Sample + 1;
        /// <summary>дата печати</summary>
        public const byte YMD = YM + 1;
        /// <summary>отборщик</summary>
        public const byte Worker = YMD + 1;
    }
    /// <summary>аккредитация</summary>
    public struct Accredit
    {
        /// <summary>Текст</summary>
        public const byte Text = 0;
        /// <summary>Дата от в днях</summary>
        public const byte YMDFrom = Text + 1;
        /// <summary>Дата до в днях</summary>
        public const byte YMDTo = YMDFrom + 1;
    }
    #endregion

    #region Вспомогательный материал
    /// <summary>хранилище инструкций</summary>
    public struct StorageInstructions
    {
        /// <summary>тип пользователя</summary>
        public const byte UType = 0;
        /// <summary>наименование</summary>
        public const byte Name = UType + 1;
        /// <summary>путь начиная от папки программы</summary>
        public const byte Path = Name + 1;
        /// <summary>тип инструкции</summary>
        public const byte TypeInstruction = Path + 1;
    }
    /// <summary>шаблоны для печати</summary>
    public struct TemplatePrint
    {
        /// <summary>наименование</summary>
        public const byte Name = 0;
        /// <summary>тип шаблона</summary>
        public const byte TypeTemplate = Name + 1;
        /// <summary>путь</summary>
        public const byte Path = TypeTemplate + 1;
        /// <summary>месяц начала действия</summary>
        public const byte YM = Path + 1;
    }
    /// <summary>текст для шаблона</summary>
    public struct TextFromTemplate
    {
        /// <summary>шаблоны для печати</summary>
        public const byte TemplatePrint = 0;
        /// <summary>имя листа</summary>
        public const byte ListName = TemplatePrint + 1;
        /// <summary>доступные метки</summary>
        public const byte Template = ListName + 1;
        /// <summary>текст для заполнения</summary>
        public const byte Text = Template + 1;
        /// <summary>метка расположения на шаблоне</summary>
        public const byte ThisTemplate = Text + 1;
    }
    #endregion
}