using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.BattleState.Heatmap.GeneralHeatmaps;
using SMUBE.Commands._Common;
using SMUBE.Commands.Effects;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToOneArgsPicker : ArgsPicker
    {
        private List<SMUBEVector2<int>> ValidTargets = new List<SMUBEVector2<int>>();
        private int _currentTargetIndex = 0;
        
        public OneToOneArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
        {
            SetDefaultTarget();
        }

        public override void Submit()
        {
            var args = GetCommandArgs();
            IsResolved = true;
            ArgsConfirmed?.Invoke(args);
        }

        public override void Return()
        {
            ValidTargets.Clear();
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
            ValidTargets.Clear();

            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (ContainsValidTarget(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    ValidTargets.Add(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates);
                }
            }
        }

        public override bool IsAnyValid()
        {
            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (ContainsValidTarget(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    return true;
                }
            }
            return false;
        }

        public override CommandArgs GetCommandArgs()
        {
            var activeUnit = BattleStateModel.ActiveUnit;

            var targetUnitData = new List<UnitData>();
            var currentTargetCoordinates = ValidTargets[_currentTargetIndex];
            if (BattleStateModel.BattleSceneState.Grid[currentTargetCoordinates.x, currentTargetCoordinates.y].UnitIdentifier != null)
            {
                var targetUnitId = BattleStateModel.BattleSceneState.Grid[currentTargetCoordinates.x, currentTargetCoordinates.y].UnitIdentifier;
                targetUnitData.Add(BattleStateModel.Units.Find(u => u.UnitData.UnitIdentifier.Equals(targetUnitId)).UnitData);
            }
            
            return new CommonArgs(activeUnit.UnitData, targetUnitData, BattleStateModel);
        }

        public override void Up()
        {
            HandlePickerInput(0, 1);
        }

        public override void Down()
        {
            HandlePickerInput(0, -1);
        }

        public override void Left()
        {
            HandlePickerInput(-1, 0);
        }

        public override void Right()
        {
            HandlePickerInput(1, 0);
        }
        
        private void HandlePickerInput(int xDelta, int yDelta)
        {
            if (xDelta == 1 || yDelta == 1)
            {
                _currentTargetIndex++;

                if (_currentTargetIndex >= ValidTargets.Count)
                {
                    _currentTargetIndex = 0;
                }
            }
            else
            {
                _currentTargetIndex--;

                if (_currentTargetIndex < 0)
                {
                    _currentTargetIndex = ValidTargets.Count -1;
                }
            }
            ArgsUpdated?.Invoke(GetCommandArgs());
        }

        public override string GetPickerInfo()
        {
            return "Choose target unit!";
        }

        public override string GetPickerState()
        {
            return "Choose target unit!";
        }

        private bool ContainsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier == null)
            {
                return false;
            }
            
            var activeUnitTeamId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;
            var targetTeamId = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier.TeamId;
            
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Ally)
            {
                return activeUnitTeamId == targetTeamId;
            }
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Enemy)
            {
                return activeUnitTeamId != targetTeamId;
            }
            
            throw new ArgumentOutOfRangeException();
        }
        
        /*
        public override CommandArgs GetSuggestedArgs(ArgsPreferences argsPreferences)
        {
            var unit = BattleStateModel.ActiveUnit;
            
            // todo hardcoded 2 teams limit
            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var enemyTeamUnits = BattleStateModel.GetTeamUnits(enemyTeamId).Where(u => u.UnitData.UnitStats.IsAlive()).ToList();
            if (enemyTeamUnits.Any())
            {
                var targetUnitData = enemyTeamUnits.ElementAt(new Random().Next(enemyTeamUnits.Count() - 1)).UnitData;
                return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnitData }, BattleStateModel);
            }

            return null;
        }
    */
        
        public override CommandArgs GetSuggestedArgs(ArgsPreferences argsPreferences)
        {
            if (argsPreferences.MovementTargetingPreference != ArgsMovementTargetingPreference.None
                || argsPreferences.PositionTargetingPreference != ArgsPositionTargetingPreference.None)
            {
                return null;
            }

            switch (argsPreferences.TargetingPreference)
            {
                case ArgsEnemyTargetingPreference.None:
                    return GetAnyStrategy();
                case ArgsEnemyTargetingPreference.Closest:
                    return GetClosestStrategy();
                case ArgsEnemyTargetingPreference.LeastHpPoints:
                    return GetLeastHpStrategy(false);
                case ArgsEnemyTargetingPreference.LeastHpPercentage:
                    return GetLeastHpStrategy(true);
                case ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange:
                    return GetEnemyWithMostAlliesInRangeStrategy();
                case ArgsEnemyTargetingPreference.MostDmgDealt:
                    // until the only One-to-One are Taunt and LowerEnemyDefense (no damage applied)
                    return GetAnyStrategy();
                case ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn:
                case ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn:
                case ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn:
                case ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn:
                    // one-to-one doesn't involve movement
                    return GetAnyStrategy();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
                
        private CommandArgs GetAnyStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            
            // todo hardcoded 2 teams limit
            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var enemyTeamUnits = BattleStateModel.GetTeamUnits(enemyTeamId).Where(u => u.UnitData.UnitStats.IsAlive()).ToList();
            if (enemyTeamUnits.Any())
            {
                var targetUnitData = enemyTeamUnits.ElementAt(new Random().Next(enemyTeamUnits.Count() - 1)).UnitData;
                return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnitData }, BattleStateModel);
            }

            return null;
        }
        
        private CommandArgs GetClosestStrategy()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets
                = BattleStateModel.GetTeamUnits(activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0)
                    .Where(u => u.UnitData.UnitStats.IsAlive()).ToList();

            if (validTargets.Count == 0)
            {
                return null;
            }

            validTargets.Shuffle();
            
            var minDistance = int.MaxValue;
            CommonArgs minDistanceArgs = null;

            foreach (var validTarget in validTargets)
            {
                var surroundingPositions = BattleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(BattleStateModel, validTarget.UnitData.BattleScenePosition);

                foreach (var surroundingPosition in surroundingPositions)
                {
                    var activeUnitPaths = BattleStateModel.BattleSceneState.PathfindingHandler.GetAllPathCacheSetsForUnit(BattleStateModel, activeUnit.UnitData.UnitIdentifier).Data;
                    var coordinates = surroundingPosition.Coordinates;
                    var pathCache = activeUnitPaths[coordinates.x, coordinates.y];
                    if (pathCache != null && pathCache.ShortestDistance != 0 && pathCache.ShortestDistance != int.MaxValue)
                    {
                        if (pathCache.ShortestDistance < minDistance)
                        {
                            minDistance = pathCache.ShortestDistance;
                            minDistanceArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData>() { validTarget.UnitData }, BattleStateModel);
                        }
                    }
                }
            }

            return minDistanceArgs;
        }
        
        private CommandArgs GetLeastHpStrategy(bool usePercentage)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets
                = BattleStateModel.GetTeamUnits(activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0)
                    .Where(u => u.UnitData.UnitStats.IsAlive()).ToList();

            if (validTargets.Count == 0)
            {
                return null;
            }

            var minHp = int.MaxValue;
            var minHpPercentage = float.MaxValue;
            CommonArgs minHpArgs = null;

            foreach (var validTarget in validTargets)
            {
                if (!usePercentage && validTarget.UnitData.UnitStats.CurrentHealth < minHp)
                {
                    minHp = validTarget.UnitData.UnitStats.CurrentHealth;
                    var args = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel);
                    minHpArgs = args;
                }

                if (usePercentage)
                {
                    var healthPercentage = validTarget.UnitData.UnitStats.CurrentHealth / validTarget.UnitData.UnitStats.MaxHealth;
                    if (healthPercentage < minHpPercentage)
                    {
                        minHpPercentage = healthPercentage;
                        var args = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel);
                        minHpArgs = args;
                    }
                }
            }

            return minHpArgs;
        }
        
        private CommandArgs GetEnemyWithMostAlliesInRangeStrategy()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets
                = BattleStateModel.GetTeamUnits(activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0)
                    .Where(u => u.UnitData.UnitStats.IsAlive()).ToList();

            if (validTargets.Count == 0)
            {
                return null;
            }
            
            validTargets.Shuffle();

            var maxReachedUnits = int.MinValue;
            CommonArgs maxReachedUnitsArgs = null;

            var alliesInRangeHeatmap = new CountReachingUnitsOfTeamIdHeatmap(activeUnit.UnitData.UnitIdentifier.TeamId, BattleStateModel, true);
            alliesInRangeHeatmap.ProcessHeatmap(BattleStateModel);

            foreach (var validTarget in validTargets)
            {
                var coordinates = validTarget.UnitData.BattleScenePosition.Coordinates;

                if (alliesInRangeHeatmap.Heatmap[coordinates.x][coordinates.y] < maxReachedUnits)
                {
                    maxReachedUnits = alliesInRangeHeatmap.Heatmap[coordinates.x][coordinates.y];
                    maxReachedUnitsArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel);
                }
            }

            return maxReachedUnitsArgs;
        }
        
        
    }
}