using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.Pathfinding;

namespace SMUBE.BattleState
{
    public class BattleStateModel
    {
        public static int FailedCommandExecutions = 0;
        
        public Unit ActiveUnit { get; private set; }
        public List<Unit> Units { get; }
        private Queue<Unit> ActionQueue { get; set; } = new Queue<Unit>();
        public GridBattleScene BattleSceneState { get; }
        
        public BattleStateModel(List<Unit> units)
        {
            Units = units;
            var initGridData = PrepareInitialGridData();
            BattleSceneState = new GridBattleScene(initGridData);
            PreAssignUnitsToPositions(initGridData);
            SetupQueue();
            
            OnNewTurn();
        }

        private BattleStateModel(BattleStateModel sourceBattleStateModel) 
        {
            Units = new List<Unit>();
            ActionQueue = new Queue<Unit>();
            foreach (var unit in sourceBattleStateModel.Units)
            {
                Units.Add(unit.DeepCopy());
            }
            
            foreach (var unit in sourceBattleStateModel.ActionQueue)
            {
                var matchingUnit = Units.Find(u => u.UnitData.UnitIdentifier == unit.UnitData.UnitIdentifier);

                if(matchingUnit != null)
                {
                    ActionQueue.Enqueue(matchingUnit);
                }
                else
                {
                    Console.WriteLine($"Unable to find matching unit for id {unit.UnitData} when deep copying world state");
                }
            }
            BattleSceneState = sourceBattleStateModel.BattleSceneState;
        }

        private InitialGridData PrepareInitialGridData()
        {
            var initialUnitSetup = new List<(SMUBEVector2<int> pos, UnitIdentifier id)>();
            var team0PosCount = 0;
            var team1PosCount = 0;
            
            foreach (var u in Units)
            {
                var initCoordinates = u.UnitData.UnitIdentifier.TeamId == 0 
                    ? BattleSceneBase.DefaultTeam0Positions[team0PosCount++] 
                    : BattleSceneBase.DefaultTeam1Positions[team1PosCount++];
                initialUnitSetup.Add((initCoordinates, u.UnitData.UnitIdentifier));
            }

            var initialGrid = PathfindingConfigurations.GameGrid;
            initialGrid.InitialUnitSetup = initialUnitSetup;
            
            var initGridData = new InitialGridData
            {
                width = 9,
                height = 9,
                InitialUnitSetup = initialUnitSetup,
            };
            
            return initialGrid;
        }
        
        private void PreAssignUnitsToPositions(InitialGridData initGridData)
        {
            foreach (var unit in Units)
            {
                var initData = initGridData.InitialUnitSetup.First(unitSetup => unitSetup.id.Equals(unit.UnitData.UnitIdentifier));
                var initPos = BattleSceneState.Grid[initData.pos.x, initData.pos.y];
                unit.UnitData.BattleScenePosition = initPos;
            }
        }

        public BattleStateModel DeepCopy()
        {
            return new BattleStateModel(this);
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
            bool success = command.TryExecute(this, commandArgs);

            if (!success)
            {
                FailedCommandExecutions++;
            }
            
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

            /*
            var commandResults = command.GetCommandResults(commandArgs);
            if (commandResults.PositionDeltas != null)
            {
                foreach (var positionDelta in commandResults.PositionDeltas)
                {
                    PathfindingAlgorithm.DirtyPositionCache.Add(positionDelta.Start);
                    PathfindingAlgorithm.DirtyPositionCache.Add(positionDelta.Target);
                }
            }
            */
            
            OnNewTurn();
            return true;
        }

        public void OnNewTurn()
        {
            if(GetNextActiveUnit(out var nextUnit))
            {
                nextUnit.UnitData.UnitStats.OnOwnTurnStartEvaluate(this);
                ActiveUnit = nextUnit;

                BattleSceneState.PathfindingHandler.OnNewTurn(this);
                ActiveUnit.UnitCommandProvider.OnNewTurn(this);
            }

            foreach (var battleScenePosition in BattleSceneState.Grid)
            {
                battleScenePosition.OnNewTurn();
            }
            
            foreach (var unit in Units)
            {
                unit.UnitData.UnitStats.OnAnyTurnStartEvaluate(this);
                unit.UnitData.BattleScenePosition.ProcessAssignedUnit(unit);
            }
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

                var coordinates= argUnit.UnitData.BattleScenePosition.Coordinates;
                PathfindingAlgorithm.DirtyPositionCache.Add((coordinates, true));
                BattleSceneState.Grid[coordinates.x, coordinates.y].Clear();
                
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

            Units.Shuffle();
            //var unitsBySpeed = Units.OrderByDescending(u => u.UnitData.UnitStats.Speed);

            foreach (var unitBySpeed in Units)
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
