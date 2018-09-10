using AutoTable;
using System;
using System.Collections.Generic;
using System.Linq;
using MAC_2.Model;
using MAC_2.Helpers;

namespace MAC_2.Employee.Mechanisms.LoadVolume
{
    internal class Item : I_Base_IDandValues
    {
        public Item(uint objecteID, Dictionary<string, string> Values)
        {
            SetValues(Values);
            this.ID = objecteID;
            SetValues();
        }
        public Item(Item file, double[] Volume, string Acount)
        {
            this.ID = file.ID;
            this.NameClient = file.NameClient;
            this.INN = file.INN;
            this.Adres = file.Adres;
            this.period = file.period;
            this.Volume = Volume.Sum();
            this.Acount = Acount;
            this.Volumes = Volume.Select(x => x.ToString()).Aggregate((a, b) => $"{a} + {b}");
            SetValues();
        }
        public Item(SelectionWell sw)
        {
            client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.GetIDValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client));
            NameClient = client.Detail.FullName.StringDivision(30);
            INN = client.INN;
            objecte = sw.Objecte;
            Adres = objecte.Adres;
            sample = sw.Sample;
        }

        public string NameClient { get; internal set; }
        public string INN { get; internal set; }
        public double Volume { get; internal set; }
        public string Acount { get; internal set; }
        public string Volumes { get; }
        public Period period = null;
        public uint ID { get; }
        public Dictionary<string, object> Values { get; internal set; }
        public Sample sample;
        public Client client;
        public string Tarif => (period == null ? String.Empty : $"{period.Price.ToString("#.00")} от {MyTools.YearMonth_From_YM(period.YM)}");

        public Objecte objecte;
        public int NumberFolder => objecte == null ? 0 : objecte.NumberFolder;

        public string Adres { get; internal set; }
        public string AdresFromBase => objecte == null ? string.Empty : objecte.Adres.StringDivision(40);

        private void SetValues()
        {
            Values = new Dictionary<string, object>();
            Values.Add(Columns.name, NameClient);
            Values.Add(Columns.inn, INN);
            Values.Add(Columns.adres, Adres);
        }
        public void SetClient(SelectionWell sw, Objecte obj = null)
        {
            if (sw == null)
            { return; }
            client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.GetIDValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client));
            if (obj == null)
            { objecte = client.Objects.FirstOrDefault(x => AdresHelper.ComparisonAdres(x.Adres, Adres) || Acount.Split(',').FirstOrDefault(y => x.Accounts.Contains(y)) != null); }
            else
            { objecte = obj; }
            if (objecte != null)
            {
                List<string> ac = new List<string>();
                foreach (var one in Acount.Split(','))
                {
                    if (!objecte.Accounts.Contains(one))
                    { ac.Add(one); }
                }
                if (ac.Count > 0)
                { objecte.SetAccounts(ac.ToArray()); }
                sample = Helpers.LogicHelper.SampleLogic.FirstModel(sw.SampleID);
            }
        }
        public void SetClient(SelectionWell[] sw)
        {
            client = Helpers.LogicHelper.ClientsLogic.FirstModel(sw.First().GetIDValue(C.SelectionWell.Well, C.Well.Object, C.Objecte.Client));
            objecte = client.Objects.FirstOrDefault(x => AdresHelper.ComparisonAdres(x.Adres, Adres) || Acount.Split(',').FirstOrDefault(y => x.Accounts.Contains(y)) != null);
            if (objecte != null)
            {
                List<string> ac = new List<string>();
                foreach (var one in Acount.Split(','))
                {
                    if (!objecte.Accounts.Contains(one))
                    { ac.Add(one); }
                }
                if (ac.Count > 0)
                { objecte.SetAccounts(ac.ToArray()); }
                var sel = sw.FirstOrDefault(x => x.GetIDValue(C.SelectionWell.Well, C.Well.Object) == objecte.ID);
                if (sel != null)
                { sample = Helpers.LogicHelper.SampleLogic.FirstModel(sel.SampleID); }
            }

        }
        private void SetValues(Dictionary<string, string> Values)
        {
            foreach (var value in Values)
            {
                switch (value.Key)
                {
                    case Columns.name:
                        {
                            NameClient = value.Value.StringDivision(30);
                            break;
                        }
                    case Columns.inn:
                        {
                            INN = value.Value;
                            break;
                        }
                    case Columns.adres:
                        {
                            Adres = value.Value.StringDivision(30);
                            break;
                        }
                    case Columns.volnew:
                        {
                            period = LoadVolumes.NewPeriod;
                            goto case Columns.volume;
                        }
                    case Columns.volold:
                        {
                            period = LoadVolumes.CurrentPeriod;
                            goto case Columns.volume;
                        }
                    case Columns.volume:
                        {
                            Volume = value.Value.TryParseDouble();
                            break;
                        }
                    case Columns.acount:
                        {
                            Acount = value.Value;
                            break;
                        }
                }
            }
        }
    }
}
