using SMUBE.DataStructures;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
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

        public static UnitStats SquireInfo = new UnitStats(new Squire(), 1000, 100, 0, 10, 20, 10);
        public static UnitStats ScholarInfo = new UnitStats(new Scholar(), 500, 25, 100, 10, 5, 15);
        public static UnitStats HunterInfo = new UnitStats(new Hunter(), 750, 75, 25, 10, 10, 20);
    }
}
