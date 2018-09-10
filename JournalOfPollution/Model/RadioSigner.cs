using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTable;

namespace MAC_2.Model
{
    /// <summary>Подписывающие / отбирающие</summary>
    public class RatioSigner : MyTools.C_A_BaseFromAllDB
    {
        public RatioSigner(uint ID, bool CanEdit = true) : base(G.RatioSigner, ID, CanEdit)
        { }

        /// <summary>Положение</summary>
        public int Position => T.RatioSigner.Rows.Get_UnShow<int>(ID, C.RatioSigner.Position);
        /// <summary>ID должности</summary>
        public uint PostID => T.RatioSigner.Rows.Get_UnShow<uint>(ID, C.RatioSigner.Post);
        /// <summary>ID работника</summary>
        public uint WorkerID => T.RatioSigner.Rows.Get_UnShow<uint>(ID, C.RatioSigner.Worker);

        public Worker Worker
        { get; private set; }

        public bool Add(Worker worker)
        {
            if (worker.ID == this.WorkerID)
            {
                this.Worker = worker;
                return true;
            }
            else
            { return false; }
        }
    }
}
