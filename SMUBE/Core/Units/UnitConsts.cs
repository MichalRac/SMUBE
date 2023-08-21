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

        public static UnitStats SquireInfo = new UnitStats(new Squire(), 500, 100, 0, 100, 20, 10);
        public static UnitStats ScholarInfo = new UnitStats(new Scholar(), 250, 25, 100, 50, 5, 15);
        public static UnitStats HunterInfo = new UnitStats(new Hunter(), 350, 75, 25, 75, 10, 20);
    }
}
