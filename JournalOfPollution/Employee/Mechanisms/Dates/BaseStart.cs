using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.Employee.Mechanisms
{
    public static class BaseStart
    {
        public static void Start()
        {            
            PollutionBase_Class.LoadAllPolutions();
            PollutionBase_Class.LoadCalculationFormuls();
            PollutionBase_Class.LoadAccurateMeasurement();
            AdditionnTable.LoadUnit();
            AdditionnTable.LoadUnits();
            AdditionnTable.LoadPeriod();
            ControlHit_Class.LoadHit();
        }
    }
}
