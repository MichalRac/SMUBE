using Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMUBE.BattleState
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

        public bool IsFinished(out int winnerTeam)
        {
            winnerTeam = -1;
            var teamOneSurvivorCount = GetTeamUnits(0).Where(u => u.UnitData.UnitStats.CurrentHealth > 0).Count();
            var teamTwoSurvivorCount = GetTeamUnits(1).Where(u => u.UnitData.UnitStats.CurrentHealth > 0).Count();

            if (teamOneSurvivorCount > 0 && teamTwoSurvivorCount == 0)
                winnerTeam = 0;
            else if (teamOneSurvivorCount == 0 && teamTwoSurvivorCount > 0)
                winnerTeam = 1;

            if (teamOneSurvivorCount == 0 || teamTwoSurvivorCount == 0)
                return true;
            else
                return false;
        }

        public bool ExecuteCommand(ICommand command, CommandArgs commandArgs)
        {
            command.Execute(this, commandArgs);
            if(TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit))
            {
                RemoveFromQueue(activeUnit);
                EnqueueAtEnd(activeUnit);
            }
            for (int i = Units.Count - 1; i >= 0; i--)
            {
                Unit unit = Units[i];
                if (unit.UnitData.UnitStats.CurrentHealth <= 0)
                {
                    TryRemoveUnit(unit);
                }
            }
            return true;
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
                if (unit.UnitData.UnitIdentifier.TeamId == teamId)
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
                if (unit.UnitData.UnitIdentifier == unitIdentifier)
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

            var unitsBySpeed = Units.OrderByDescending(u => u.UnitData.UnitStats.Speed);

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
                if(unit.Equals(argUnit))
                {
                    continue;
                }
                newQueue.Enqueue(unit);
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
