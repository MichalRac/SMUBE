using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Units
{
    public class UnitData
    {
        public string Name { get; private set; }
        public UnitIdentifier UnitIdentifier { get; }
        public UnitStats UnitStats { get; }

        public UnitData(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats)
        {
            Name = argName;
            UnitIdentifier = argUnitIdentifier;
            UnitStats = argUnitStats;
        }

    }
}
