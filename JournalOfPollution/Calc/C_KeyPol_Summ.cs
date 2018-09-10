using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Calc
{

    public class C_KeyPol_Summ
    {
        public C_KeyPol_Summ(uint PollutionID,decimal Summ,bool IsTarif)
        {
            this.PollutionID = PollutionID;
            this.Summ = Summ;
            this.IsTarif = IsTarif;
        }
        public        uint PollutionID;
        public decimal Summ;
        public bool IsTarif;
    }
}
