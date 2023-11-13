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

        public static UnitStats SquireInfo = new UnitStats(new Squire(), 350, 50, 0, 100, 20, 350);
        public static UnitStats ScholarInfo = new UnitStats(new Scholar(), 175, 0, 50, 50, 5, 250);
        public static UnitStats HunterInfo = new UnitStats(new Hunter(), 300, 75, 0, 75, 10, 450);
    }
}
