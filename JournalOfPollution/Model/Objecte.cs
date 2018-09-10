using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;
using MAC_2.Employee.Mechanisms;
using column = MAC_2.EmployeeWindow.Employee_Window.column;

namespace MAC_2.Model
{
    /// <summary>объект</summary>
    public class Objecte : MyTools.C_A_BaseFromAllDB
    {
        /// <param name="LoadAll">Прогрузить все внутренности</param>
        public Objecte(uint ID, bool CanEdit = true) : base(G.Objecte, ID, CanEdit)
        {
            Values = new Dictionary<string, object>();
            Values.Add(T.Objecte.GetColumn(C.Objecte.AdresFact).AlterName, Adres);
            Values.Add(T.Objecte.GetColumn(C.Objecte.NumberFolder).AlterName, NumberFolder);
        }

        public Client Client
        { get; private set; }

        public void InitializeColumns()
        {
            Values = new Dictionary<string, object>();
            Values.Add(column.INN, T.Client.Rows.Get<string>(ClientID, C.Client.INN));  //лучше использовать такую конструкцию, т.к. связь все еще может потеряться
            Values.Add(column.Name, Name);

            Values.Add(column.Adres, T.Objecte.Rows.Get<string>(ID, C.Objecte.AdresFact, C.AdresReference.Adres).StringDivision(20));
            Values.Add(column.NumberFolder, NumberFolder);

            Values.Add(column.DateClose, close);
        }

        public string Name
        => $"{this.Client.Name} {(this.Detail == null ? String.Empty : this.Detail.AddName)}";

        public string close => _close > 0 ? $"Закрыто от {MyTools.YearMonth_From_YM(_close)}" : "";
        int _close => closeC > closeO ? closeO : closeC;

        private int closeO => YMTo;
        private int closeC => T.Client.Rows.Get<int>(ClientID, C.Client.YMTo);

        /// <summary>Адрес</summary>
        public string Adres => T.Objecte.Rows.Get<string>(ID, C.Objecte.AdresFact, C.AdresReference.Adres);
        /// <summary>Номер папки</summary>
        public int NumberFolder => T.Objecte.Rows.Get<int>(ID, C.Objecte.NumberFolder);
        /// <summary>ID вид пробы</summary>
        public uint TypeSampleID => T.Objecte.Rows.Get_UnShow<uint>(ID, C.Objecte.TypeSample);
        /// <summary>Дата создания в месяцах</summary>
        public int YMFrom => T.Objecte.Rows.Get<int>(ID, C.Objecte.YMFrom);
        /// <summary>Дата закрытия в месяцах</summary>
        public int YMTo => T.Objecte.Rows.Get<int>(ID, C.Objecte.YMTo);
        /// <summary>Положение по району</summary>
        public int OrderDistrict => T.Objecte.Rows.Get<int>(ID, C.Objecte.AdresFact, C.AdresReference.District, C.DistrictReference.Order);
        public uint ClientID => T.Objecte.Rows.Get_UnShow<uint>(ID, C.Objecte.Client);
        /// <summary>Обособленный</summary>
        public bool Separate => T.Objecte.Rows.Get<bool>(ID, C.Objecte.Separate);

        private Dictionary<uint, NegotiationAssistant> _negotiationAssistants = new Dictionary<uint, NegotiationAssistant>();

        public bool Add(NegotiationAssistant negotiationAssistant)
        {
            if (negotiationAssistant.ObjectID == this.ID)
            {
                if (_negotiationAssistants.ContainsKey(negotiationAssistant.ID))
                { _negotiationAssistants[negotiationAssistant.ID] = negotiationAssistant; }
                else
                { _negotiationAssistants.Add(negotiationAssistant.ID, negotiationAssistant); }

                negotiationAssistant.Add(this);

                return true;
            }
            else
            { return false; }
        }

        private Dictionary<uint, DetailsObject> _details = new Dictionary<uint, DetailsObject>();

        public IEnumerable<DetailsObject> Details => _details.Values;

        public DetailsObject Detail
        { get { return _details.Values.FirstOrDefault(); } }
        public bool Add(DetailsObject detail)
        {
            if (detail.ObjectID == this.ID)
            {
                if (_details.ContainsKey(detail.ID))
                { _details[detail.ID] = detail; }
                else
                { _details.Add(detail.ID, detail); }

                detail.Add(this);

                return true;
            }
            else
            { return false; }
        }
        /// <summary>Отношение к постановлениям</summary>
        private Dictionary<uint, ObjectFromResolution> _ofr = new Dictionary<uint, ObjectFromResolution>();

        public bool Add(ObjectFromResolution objectFromResolution)
        {
            if (objectFromResolution.ObjectID == this.ID)
            {
                if (_ofr.ContainsKey(objectFromResolution.ID))
                { _ofr[objectFromResolution.ID] = objectFromResolution; }
                else
                { _ofr.Add(objectFromResolution.ID, objectFromResolution); }

                objectFromResolution.Add(this);

                return true;
            }
            else
            { return false; }
        }

        public bool Add(Client client)
        {
            if (client.ID == this.ClientID)
            {
                Client = client;
                return true;
            }
            else
            { return false; }
        }

        private Dictionary<uint, Well> _wells = new Dictionary<uint, Well>();

        public bool Add(Well well)
        {
            if (well.ObjectID == this.ID)
            {
                if (_wells.ContainsKey(well.ID))
                { _wells[well.ID] = well; }
                else
                { _wells.Add(well.ID, well); }

                well.Add(this);

                return true;
            }
            else
            { return false; }
        }

        public IEnumerable<Well> Wells => _wells.Values;
        public IEnumerable<ObjectFromResolution> OFR => _ofr.Values;
        MidMonthVolume MidVolume;
        /// <summary>Получить среднемесячный объём</summary>
        public MidMonthVolume GetMidMonthVolume(int Year)
        {
            if (MidVolume != null && MidVolume.Year == Year)
            { return MidVolume; }
            MidVolume = new MidMonthVolume((uint)G.MidMonthVolume.QUERRY()
                .GET
                .ID()
                .WHERE
                .C(C.MidMonthVolume.Year, Year)
                .AND
                .C(C.MidMonthVolume.Objecte, ID)
                .DO()[0].Value);
            return MidVolume;
        }
        /// <summary>Можно ли отбирать по постановлению</summary>
        /// <param name="ResolutionID">ID постановления</param>
        /// <returns>True можно, False нельзя</returns>
        public bool CanResolution(uint ResolutionID)
        {
            if (OFR != null)
            {
                if (OFR.FirstOrDefault(x => x.ResolutionID == ResolutionID) != null)
                { return !OFR.First(x => x.ResolutionID == ResolutionID).Application; }
            }
            else
            {
                return !((int)G.ObjectFromResolution.QUERRY()
                    .COUNT
                    .WHERE
                    .C(C.ObjectFromResolution.Resolution, ResolutionID)
                    .AND
                    .C(C.ObjectFromResolution.Object, ID)
                    .AND
                    .C(C.ObjectFromResolution.Application, true)
                    .DO()[0].Value > 0);
            }
            return true;
        }
        /// <summary>Л/С из 1С</summary>
        public string[] Accounts => G.Objecte.Rows.Get<string>(ID, C.Objecte.Account1C).Split(',');
        public void SetAccounts(params string[] Acc)
        {
            List<string> result = Acc.Concat(Accounts).ToList();
            result.Remove("");
            result = result.Distinct().ToList();
            SetOneValue(C.Objecte.Account1C, result.Aggregate((a, b) => $"{a},{b}"));
        }
        public void DelectAccounts(params string[] Acc)
        {
            var ac = Accounts.ToList();
            foreach (var one in Acc)
            { ac.Remove(one); }
            SetOneValue(C.Objecte.Account1C, ac.Count > 0 ? ac.Aggregate((a, b) => $"{a},{b}") : "");
        }

        public override string ToString()
        {
            return $"{ID}, {Name}";
        }
    }
}
