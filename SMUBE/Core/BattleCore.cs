using SMUBE.DataStructures;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Core
{
    public class BattleCore
    {
        public List<Unit> Units { get; private set; } = new List<Unit>();

        public bool TryAddUnit(Unit argUnit)
        {
            if(!Units.Contains(argUnit))
            {
                Units.Add(argUnit);
                return true;
            }

            return false;
        }

        public bool TryRemoveUnit(Unit argUnit)
        {
            if (Units.Contains(argUnit))
            {
                Units.Remove(argUnit);
                return true;
            }

            return false;
        }

        public List<Unit> GetTeamUnits(int teamId)
        {
            var teamUnits = new List<Unit>();
            foreach (var unit in Units)
            {
                if(unit.UnitIdentifier.TeamId == teamId)
                {
                    teamUnits.Add(unit);
                }
            }
            return teamUnits;
        }

        public bool GetUnit(UnitIdentifier unitIdentifier, out Unit result)
        {
            result = null;

            foreach (var unit in Units)
            {
                if(unit.UnitIdentifier == unitIdentifier)
                {
                    result = unit;
                    return true;
                }
            }

            return false;
        }
    }
}
