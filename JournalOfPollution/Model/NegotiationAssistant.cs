using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Помошник согласования</summary>
    public class NegotiationAssistant
        : MyTools.C_A_BaseFromAllDB
    {
        public NegotiationAssistant(uint ID, bool CanEdit = true) : base(G.NegotiationAssistant, ID, CanEdit)
        { }
        /// <summary>ID объекта</summary>
        public uint ObjectID => T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Objecte);
        /// <summary>ID отборщика</summary>
        public uint WorkerID
        {
            get { return T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Worker); }
            set { T.NegotiationAssistant.Rows.Set(ID, C.NegotiationAssistant.Worker, value); }
        }
        /// <summary>ID отбора</summary>
        public uint SampleID
        {
            get { return T.NegotiationAssistant.Rows.Get_UnShow<uint>(ID, C.NegotiationAssistant.Sample); }
            set { SetOneValue(C.NegotiationAssistant.Sample, value); }
        }
        /// <summary>ID отбора</summary>
        public int YMD
        {
            get { return T.NegotiationAssistant.Rows.Get_UnShow<int>(ID, C.NegotiationAssistant.YMD); }
            set { SetOneValue(C.NegotiationAssistant.YMD, value); }
        }

        public bool Add(Worker worker)
        {
            if (worker.ID == WorkerID)
            {
                this.Worker = worker;
                return true;
            }
            else
            { return false; }
        }

        public Sample Sample
        { get; private set; }

        public Objecte Objecte
        { get; private set; }

        public Worker Worker
        { get; private set; }

        public bool Add(Objecte objecte)
        {
            if (objecte.ID == this.ObjectID)
            {
                this.Objecte = objecte;
                return true;
            }
            else
            { return false; }
        }

        public bool Add(Sample sample)
        {
            if (this.Sample == null || sample.ID == this.SampleID)
            {
                this.Sample = sample;
                sample.Add(this);
                return true;
            }
            else
            { return false; }
        }
    }
}
