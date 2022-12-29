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
    public class Unit
    {
        public UnitData UnitData { get; private set; }

        public Unit(UnitData argUnitData)
        {
            UnitData = argUnitData;
        }

        public Unit(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats) 
            : this(new UnitData(argName, argUnitIdentifier, argUnitStats)) { }

        public object GetViableCommands()
        {
            throw new NotImplementedException();
        }
    }
}
