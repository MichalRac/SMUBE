using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands.RegularAttack
{
    public class RegularAttackArgs : CommandArgs
    {
        public UnitIdentifier UnitTarget { get; private set; }
    }
}
