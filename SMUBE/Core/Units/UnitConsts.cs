using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.Units
{
    internal class UnitConsts
    {
        public static int UNIT_SIZE = 1;

        public static UnitStats SquireInfo = new UnitStats(new Squire(), 
            350, 100, 0, 
            100, 20, 35);
        
        public static UnitStats ScholarInfo = new UnitStats(new Scholar(), 
            175, 0, 100, 
            50, 5, 25);
        
        public static UnitStats HunterInfo = new UnitStats(new Hunter(), 
            300, 100, 0, 
            75, 10, 45);
    }
}
