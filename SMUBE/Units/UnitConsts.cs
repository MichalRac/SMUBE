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
        public static UnitInfo SquireInfo = new UnitInfo("Squire", 100, 100, 0, 20, 10);
        public static UnitInfo ScholarInfo = new UnitInfo("Scholar", 50, 25, 100, 5, 15);
        public static UnitInfo HunterInfo = new UnitInfo("Hunter", 75, 75, 25, 10, 20);
    }
}
