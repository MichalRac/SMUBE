using SMUBE.DataStructures;
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
        public UnitIdentifier UnitIdentifier { get; private set; }
        public UnitInfo UnitInfo { get; private set; }

        public Unit(int id, int teamId, BaseCharacter baseCharacter)
        {
            UnitIdentifier = new UnitIdentifier(id, teamId);
            UnitInfo = baseCharacter.UnitInfo;
        }
    }
}
