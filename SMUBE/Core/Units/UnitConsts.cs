using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.Units
{
    internal class UnitConsts
    {
        public static int UNIT_SIZE = 1;

        public static UnitStats SquireInfo = new UnitStats(new Squire(), 
            500, 100, 0, 
            50, 20, 35);
        
        public static UnitStats ScholarInfo = new UnitStats(new Scholar(), 
            350, 0, 150, 
            50, 5, 25);
        
        public static UnitStats HunterInfo = new UnitStats(new Hunter(), 
            350, 100, 0, 
            100, 10, 45);
    }
}
