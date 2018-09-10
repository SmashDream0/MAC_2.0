using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Model;

namespace MAC_2
{
    public static partial class Cache
    {
        public static Cache<Pollution> PollutionCache = new Cache<Pollution>();
        public static Cache<Sample> SampleCache = new Cache<Sample>();
        public static Cache<Well> WellCache = new Cache<Well>();
    }
}
