using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.AI.QLearning;
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
        public List<Unit> AllUnits { get; }
        private Queue<Unit> ActionQueue { get; set; } = new Queue<Unit>();
        public GridBattleScene BattleSceneState { get; }

        public int ActionsTakenCount = 0;
        
        public BattleStateModel(List<Unit> units)
        {
            ActionsTakenCount = 0;
            Units = units;
            AllUnits = new List<Unit>();
            foreach (var unit in Units)
            {
                AllUnits.Add(unit);
            }
            var initGridData = PrepareInitialGridData();
            BattleSceneState = new GridBattleScene(initGridData);
            PreAssignUnitsToPositions(initGridData);
            SetupQueue();
            
            OnNewTurn();
        }

        private BattleStateModel(BattleStateModel sourceBattleStateModel)
        {
            ActionsTakenCount = sourceBattleStateModel.ActionsTakenCount;
            Units = new List<Unit>();
            ActiveUnit = sourceBattleStateModel.ActiveUnit.DeepCopy();
            ActionQueue = new Queue<Unit>();
            foreach (var unit in sourceBattleStateModel.Units)
            {
                Units.Add(unit.DeepCopy());
            }
            AllUnits = new List<Unit>();
            foreach (var unit in AllUnits)
            {
                AllUnits.Add(unit.DeepCopy());
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
            BattleSceneState = sourceBattleStateModel.BattleSceneState.DeepCopy();
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
            
            var initGridData = new InitialGridData
            {
                width = PathfindingConfigurations.GameGrid.width,
                height = PathfindingConfigurations.GameGrid.height,
                InitialUnitSetup = initialUnitSetup,
                InitialObstacleCells = PathfindingConfigurations.GameGrid.InitialObstacleCells,
                InitialUnstableCells = PathfindingConfigurations.GameGrid.InitialUnstableCells,
                InitialDefensiveCells = PathfindingConfigurations.GameGrid.InitialDefensiveCells,
            };
            
            return initGridData;
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
            ProcessQLearningActionCache(command);
            
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
                    //unit.UnitCommandProvider.QLearningLastActionCache = null;
                    TryRemoveUnit(unit);
                }
            }
            
            OnNewTurn();
            ActionsTakenCount++;
            return true;
        }

        private void ProcessQLearningActionCache(ICommand command)
        {
            ActiveUnit.UnitCommandProvider.QLearningLastActionCache = new UnitCommandProvider.QLearningLastActionCacheData()
            {
                stateId = new QLearningState().GetStateNumber(this, ActiveUnit), 
                CommandId = command.CommandId,
                ArgsPreferences = command.ArgsPreferences,
            };
        }

        public void DebugReevaluateCommands()
        {
            ActiveUnit.UnitCommandProvider.OnNewTurn(this);
        }
        
        public void OnNewTurn()
        {
            foreach (var battleScenePosition in BattleSceneState.Grid)
            {
                battleScenePosition.OnNewTurn(this);
            }
            
            if(GetNextActiveUnit(out var nextUnit))
            {
                nextUnit.UnitData.UnitStats.OnOwnTurnStartEvaluate(this);
                ActiveUnit = nextUnit;

                BattleSceneState.PathfindingHandler.OnNewTurn(this);
                ActiveUnit.UnitCommandProvider.OnNewTurn(this);
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
                BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((coordinates, true));
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
        public List<Unit> GetAllTeamUnits(int teamId)
        {
            var teamUnits = new List<Unit>();
            foreach (var unit in AllUnits)
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
            //var unitsByTeamId = Units.OrderBy(u => u.UnitData.UnitIdentifier.TeamId);

            foreach (var unit in Units)
            {
                ActionQueue.Enqueue(unit);
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
