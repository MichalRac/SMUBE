using SMUBE.DataStructures.Units;
using SMUBE.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Core
{
    public class BattleStateModel
    {
        public List<Unit> Units { get; private set; } = new List<Unit>();
        private Queue<Unit> ActionQueue { get; set; } = new Queue<Unit>();

        public BattleStateModel() {}
        public BattleStateModel(List<Unit> units)
        {
            Units = units;
            SetupQueue();
        }

        public bool TryAddUnit(Unit argUnit)
        {
            if (!Units.Contains(argUnit))
            {
                Units.Add(argUnit);
                EnqueueAtEnd(argUnit);
                return true;
            }

            return false;
        }

        public bool TryRemoveUnit(Unit argUnit)
        {
            if (Units.Contains(argUnit))
            {
                Units.Remove(argUnit);
                RemoveFromQueue(argUnit);
                return true;
            }

            return false;
        }

        public List<Unit> GetTeamUnits(int teamId)
        {
            var teamUnits = new List<Unit>();
            foreach (var unit in Units)
            {
                if (unit.UnitIdentifier.TeamId == teamId)
                {
                    teamUnits.Add(unit);
                }
            }
            return teamUnits;
        }

        public bool TryGetUnit(UnitIdentifier unitIdentifier, out Unit result)
        {
            result = null;

            foreach (var unit in Units)
            {
                if (unit.UnitIdentifier == unitIdentifier)
                {
                    result = unit;
                    return true;
                }
            }

            return false;
        }

        #region Queueing
        private void SetupQueue()
        {
            ActionQueue.Clear();

            var unitsBySpeed = Units.OrderByDescending(u => u.UnitInfo.Speed);

            foreach (var unitBySpeed in unitsBySpeed)
            {
                ActionQueue.Enqueue(unitBySpeed);
            }
        }

        private void EnqueueAtEnd(Unit argUnit)
        {
            ActionQueue.Enqueue(argUnit);
        }

        private void RemoveFromQueue(Unit argUnit)
        {
            var newQueue = new Queue<Unit>();
            while(ActionQueue.Count > 0)
            {
                var unit = ActionQueue.Dequeue();
                if(argUnit.Equals(unit))
                {
                    continue;
                }
                newQueue.Enqueue(argUnit);
            }
            ActionQueue = newQueue;
        }

        public bool GetNextActiveUnit(out Unit result)
        {
            result = null;

            if (ActionQueue == null || ActionQueue.Count == 0)
            {
                return false;
            }

            result = ActionQueue.Peek();
            return true;
        }

        public bool GetUnitQueueShallowCopy(out Queue<Unit> unitQueue)
        {
            unitQueue = new Queue<Unit>(ActionQueue);
            if (unitQueue == null || unitQueue.Count == 0)
            {
                return false;
            }

            return true;
        }
        #endregion

    }
}
