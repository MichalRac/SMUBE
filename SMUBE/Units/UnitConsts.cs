using SMUBE.DataStructures;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units
{
    internal class UnitConsts
    {
        public static int UNIT_SIZE = 1;

        public static UnitStats SquireInfo = new UnitStats(100, 100, 0, 20, 10);
        public static UnitStats ScholarInfo = new UnitStats(50, 25, 100, 5, 15);
        public static UnitStats HunterInfo = new UnitStats(75, 75, 25, 10, 20);
    }
}
