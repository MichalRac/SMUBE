using SMUBE.DataStructures;
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
        public Unit(int id, int teamId)
        {
            UnitIdentifier = new UnitIdentifier(id, teamId);
        }
    }
}
