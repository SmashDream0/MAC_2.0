using AutoTable;
using System.Collections.Generic;

namespace MAC_2.Model
{
    /// <summary>Отборы</summary>
    public class Sample : MyTools.C_A_BaseFromAllDB
    {
        public Sample(uint ID, bool CanEdit = true) : base(G.Sample, ID, CanEdit)
        { }
        /// <summary>Месяц создания</summary>
        public int YM => T.Sample.Rows.Get<int>(ID, C.Sample.YM);
        /// <summary>ID статуса</summary>
        public uint StatusID => T.Sample.Rows.Get_UnShow<uint>(ID, C.Sample.Status);
        /// <summary>ID сотрудника</summary>
        public uint WorkerID => T.Sample.Rows.Get_UnShow<uint>(ID, C.Sample.Worker);
        /// <summary>ID представителя</summary>
        public uint RepresentativeID => T.Sample.Rows.Get_UnShow<uint>(ID, C.Sample.Representative);

        public NegotiationAssistant NegotiationAssistant
        { get; private set; }

        private Dictionary<uint, Volume> _volumes = new Dictionary<uint, Volume>();
        private Dictionary<uint, SelectionWell> _selectionWell = new Dictionary<uint, Model.SelectionWell>();
        
        public IEnumerable<ValueSelection> ValueSelection
        {
            get
            {
                var list = new List<ValueSelection>();

                foreach (var selectionWell in SelectionWells)
                {
                    list.AddRange(selectionWell.ValueSelections);
                }

                return list.ToArray();
            }
        }
        public IEnumerable<Volume> Volumes => _volumes.Values;
        public IEnumerable<SelectionWell> SelectionWells => _selectionWell.Values;

        public bool Add(Volume volume)
        {
            if (volume.SampleID == this.ID)
            {
                if (this._volumes.ContainsKey(volume.ID))
                { this._volumes[volume.ID] = volume; }
                else
                { this._volumes.Add(volume.ID, volume); }

                //volume.ad()

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ClearVolumes()
        { this._volumes.Clear(); }

        public bool Add(SelectionWell selectionWell)
        {
            if (selectionWell.SampleID == this.ID)
            {
                if (this._selectionWell.ContainsKey(selectionWell.ID))
                { this._selectionWell[selectionWell.ID] = selectionWell; }
                else
                { this._selectionWell.Add(selectionWell.ID, selectionWell); }
                
                selectionWell.Add(this);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Add(NegotiationAssistant negotiationAssistant)
        {
            if (negotiationAssistant.SampleID == this.ID && NegotiationAssistant == null)
            {
                NegotiationAssistant = negotiationAssistant;
                negotiationAssistant.Add(this);

                return true;
            }
            else
            { return false; }
        }
    }
}
