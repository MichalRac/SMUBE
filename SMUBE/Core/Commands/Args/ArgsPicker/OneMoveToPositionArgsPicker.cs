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
    public sealed class OneMoveToPositionArgsPicker : ArgsPicker
    {
        private bool IsPathless { get; }
        private SMUBEVector2<int> _currentTargetCoordinates;
        public OneMoveToPositionArgsPicker(ICommand command, BattleStateModel battleStateModel, bool isPathless) : base(command, battleStateModel)
        {
            IsPathless = isPathless;
            SetDefaultTarget();
        }
        
        public override void Submit()
        {
            var currentArgs = GetCommandArgs();
            if (Command.CommandArgsValidator.Validate(currentArgs, BattleStateModel))
            {
                ArgsConfirmed?.Invoke(currentArgs);
                IsResolved = true;
            }
            else
            {
                ArgsInvalid?.Invoke();
            }
        }

        public override void Return()
        {
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
            var viablePositions = BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions;

            if (viablePositions == null || viablePositions.Count == 0)
            {
                return;
            }
            
            _currentTargetCoordinates = BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions.First().TargetPosition.Coordinates;
        }

        public override bool IsAnyValid()
        {
            return BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions
                .Any(pos => !pos.TargetPosition.IsOccupied() && pos.TargetPosition.IsWalkable());
        }

        public override CommandArgs GetCommandArgs()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var positionDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, 
                activeUnit.UnitData.BattleScenePosition.Coordinates, _currentTargetCoordinates, IsPathless);
            return new CommonArgs(activeUnit.UnitData, null, BattleStateModel, positionDelta);
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
            TryMoveToOffset(xDelta, yDelta);
            ArgsUpdated?.Invoke(GetCommandArgs());
        }
        
        private void TryMoveToOffset(int xDelta, int yDelta)
        {
            var targetPos = new SMUBEVector2<int>(_currentTargetCoordinates.x + xDelta, _currentTargetCoordinates.y + yDelta);
            if (IsValidTarget(targetPos))
            {
                _currentTargetCoordinates = targetPos;
            }
        }

        public override string GetPickerInfo()
        {
            return "Pick any empty reachable position!";
        }

        public override string GetPickerState()
        {
            return IsResolved 
                ? "Resolved"
                : "Pick any empty reachable position";
        }

        private bool IsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (targetPos.x < 0 || targetPos.x >= BattleStateModel.BattleSceneState.Width)
                return false;
            if (targetPos.y < 0 || targetPos.y >= BattleStateModel.BattleSceneState.Height)
                return false;
            
            if (IsPathless)
            {
                return BattleStateModel.BattleSceneState.IsEmpty(targetPos);
            }
            else
            {
                foreach (var reachablePosition in BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions)
                {
                    if (reachablePosition.TargetPosition.Coordinates.Equals(targetPos))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        public override CommandArgs GetSuggestedArgs(ArgsPreferences argsPreferences)
        {
            if (argsPreferences.TargetingPreference != ArgsEnemyTargetingPreference.None
                || argsPreferences.PositionTargetingPreference != ArgsPositionTargetingPreference.None)
            {
                return null;
            }

            switch (argsPreferences.MovementTargetingPreference)
            {
                case ArgsMovementTargetingPreference.None:
                    // random
                    return GetAnyStrategy();
                case ArgsMovementTargetingPreference.GetOutOfReach:
                    // go to non reachable by enemies, or towards closest non reachable by enemies
                    return GetOutOfReachStrategy();
                case ArgsMovementTargetingPreference.GetCloserCarefully:
                    // approach enemies without getting into range, or minimizing the number of enemies
                    return GetCloserCarefullyStrategy();
                case ArgsMovementTargetingPreference.GetCloserAggressively:
                    // get as close as possible to nearest enemy
                    return GetClosestStrategy();
                case ArgsMovementTargetingPreference.OptimizeFortifiedPosition:
                    // go to most fortified position reachable
                    return GetByPositionBuffStrategy(false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region AIArgStrategies
        
        private CommandArgs GetAnyStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var reachablePositions = BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions
                .Where(pos => !pos.TargetPosition.IsOccupied() && pos.TargetPosition.IsWalkable()).ToList();
                    
            if (!reachablePositions.Any())
                return null;
            
            var target = reachablePositions.GetRandom();
            
            var positionDelta = new PositionDelta(unit.UnitData.UnitIdentifier, unit.UnitData.BattleScenePosition.Coordinates, target.TargetPosition.Coordinates);
            var result = new CommonArgs(unit.UnitData, null, BattleStateModel, positionDelta);
            result.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetAnyStrategy)}";
            return result;
        }
        
        private CommandArgs GetOutOfReachStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var allValidPaths = BattleStateModel.BattleSceneState.PathfindingHandler
                .AllUnitPaths[unit.UnitData.UnitIdentifier].Where(p => p.IsReachable && !p.TargetPosition.IsOccupied() && p.TargetPosition.IsWalkable()).ToList();

            if (!allValidPaths.Any())
            {
                return null;
            }

            allValidPaths.Shuffle();
            var validPathsSorted = allValidPaths.OrderBy(p => p.ShortestDistance);

            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var reachableByEnemyHeatmap = new CountReachingUnitsOfTeamIdHeatmap(enemyTeamId, BattleStateModel, true);
            reachableByEnemyHeatmap.ProcessHeatmap(BattleStateModel);

            var minRisk = int.MaxValue;
            var minTurnsForImprovement = int.MaxValue;
            PositionDelta optimalPositionDelta = null;
            
            foreach (var validPath in validPathsSorted)
            {
                var targetCoordinates = validPath.TargetPosition.Coordinates;
                var potentialRisk = reachableByEnemyHeatmap.Heatmap[targetCoordinates.x][targetCoordinates.y];
                var unitSpeed = unit.UnitData.UnitStats.Speed;
                var turnsRequired = BattleStateModel.BattleSceneState.PathfindingHandler.GetRequiredMovementTurns(validPath, unitSpeed);

                if (turnsRequired <= minTurnsForImprovement)
                {
                    minTurnsForImprovement = turnsRequired;
                    if (potentialRisk >= minRisk) continue;
                    
                    minRisk = potentialRisk;

                    if (turnsRequired == 1)
                    {
                        optimalPositionDelta = new PositionDelta(unit.UnitData.UnitIdentifier, 
                            unit.UnitData.BattleScenePosition.Coordinates, targetCoordinates);
                    }
                    else
                    {
                        BattleStateModel.BattleSceneState.PathfindingHandler
                            .GetLastReachableOnPath(BattleStateModel, validPath.TargetPosition, out var lastReachable, out _);
                        optimalPositionDelta = new PositionDelta(unit.UnitData.UnitIdentifier, 
                            unit.UnitData.BattleScenePosition.Coordinates, lastReachable.Coordinates);
                    }
                }
            }

            if (optimalPositionDelta == null)
            {
                return GetAnyStrategy();
            }
            
            return new CommonArgs(unit.UnitData, null, BattleStateModel, optimalPositionDelta);
        }
        
        private CommandArgs GetCloserCarefullyStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var allValidPaths = BattleStateModel.BattleSceneState.PathfindingHandler
                .AllUnitReachablePaths[unit.UnitData.UnitIdentifier]
                .Where(p => p.IsReachable && !p.TargetPosition.IsOccupied() && p.TargetPosition.IsWalkable()).ToList();

            if (!allValidPaths.Any())
            {
                return null;
            }

            allValidPaths.Shuffle();
            var validPathsSorted = allValidPaths.OrderBy(p => p.ShortestDistance).ToList();

            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var reachableByEnemyHeatmap = new CountReachingUnitsOfTeamIdHeatmap(enemyTeamId, BattleStateModel, true);
            reachableByEnemyHeatmap.ProcessHeatmap(BattleStateModel);

            var distanceToEnemiesHeatmap = new DistanceToUnitOfTeamIdHeatmap(enemyTeamId, false, BattleStateModel);
            distanceToEnemiesHeatmap.ProcessHeatmap(BattleStateModel);
            
            var minRisk = int.MaxValue;
            var minDistance = int.MaxValue;
            PositionDelta optimalPositionDelta = null;
            
            foreach (var validPath in validPathsSorted)
            {
                var targetCoordinates = validPath.TargetPosition.Coordinates;
                var potentialRisk = reachableByEnemyHeatmap.Heatmap[targetCoordinates.x][targetCoordinates.y];

                if (potentialRisk > minRisk)
                {
                    continue;
                }
                
                minRisk = potentialRisk;
                var distance = distanceToEnemiesHeatmap.Heatmap[targetCoordinates.x][targetCoordinates.y];

                if (distance < minDistance)
                {
                    minDistance = distance;
                    optimalPositionDelta = new PositionDelta(unit.UnitData.UnitIdentifier, 
                        unit.UnitData.BattleScenePosition.Coordinates, targetCoordinates);
                }
            }
            
            if (optimalPositionDelta == null)
            {
                return GetAnyStrategy();
            }

            return new CommonArgs(unit.UnitData, null, BattleStateModel, optimalPositionDelta);
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
            
            var minDistance = int.MaxValue;
            CommonArgs minDistanceArgs = null;

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, true)
                    .Where(p => !p.TargetPosition.Coordinates.Equals(activeUnit.UnitData.BattleScenePosition.Coordinates)).ToList();

                if (reachableSurroundingCache.Count == 0)
                    continue;

                var closestReachableSurroundingPosition = reachableSurroundingCache
                    .OrderBy(p => p.ShortestDistance).First();
                
                if (closestReachableSurroundingPosition.ShortestDistance < minDistance)
                {
                    minDistance = closestReachableSurroundingPosition.ShortestDistance;
                    var speed = activeUnit.UnitData.UnitStats.Speed;
                    var stepsRequired = BattleStateModel.BattleSceneState.PathfindingHandler.GetRequiredMovementTurns(closestReachableSurroundingPosition, speed);

                    if (stepsRequired > 1)
                    {
                        BattleStateModel.BattleSceneState.PathfindingHandler.GetLastReachableOnPath(BattleStateModel,
                            closestReachableSurroundingPosition.TargetPosition, out var lastReachable, out _);
                        var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, 
                            activeUnit.UnitData.BattleScenePosition.Coordinates, 
                            lastReachable.Coordinates);
                        var args = new CommonArgs(activeUnit.UnitData, null, BattleStateModel, posDelta);
                        minDistanceArgs = args;
                    }
                    else if(stepsRequired == 1)
                    {
                        var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, 
                            activeUnit.UnitData.BattleScenePosition.Coordinates, 
                            closestReachableSurroundingPosition.TargetPosition.Coordinates);
                        var args = new CommonArgs(activeUnit.UnitData, null, BattleStateModel, posDelta);
                        minDistanceArgs = args;
                    }
                    else if (stepsRequired == 0)
                    {
                        throw new Exception();
                    }
                }
            }

            if (minDistanceArgs == null || minDistanceArgs.PositionDelta.Target.Equals(minDistanceArgs.PositionDelta.Start))
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetClosestStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
            }

            minDistanceArgs.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetClosestStrategy)}";
            return minDistanceArgs;
        }
        
        /*
        private CommandArgs GetByReachableEnemiesAfterTurnStrategy(bool minimize)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var enemyTeamId = activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;

            var validTargets
                = BattleStateModel.GetTeamUnits(enemyTeamId)
                    .Where(u => u.UnitData.UnitStats.IsAlive()).ToList();

            if (validTargets.Count == 0)
            {
                return null;
            }
            
            validTargets.Shuffle();
            
            var minReachableEnemies = int.MinValue;
            var maxReachableEnemies = int.MaxValue;
            CommonArgs optimalReachedUnitsArgs = null;
            
            var inEnemyRangeCountHeatmap = new CountReachingUnitsOfTeamIdHeatmap(enemyTeamId, BattleStateModel, true);
            inEnemyRangeCountHeatmap.ProcessHeatmap(BattleStateModel);

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition);

                if (reachableSurroundingCache.Count == 0)
                    continue;
                
                reachableSurroundingCache.Shuffle();

                foreach (var potentialTargetPosition in reachableSurroundingCache)
                {
                    var coordinates = potentialTargetPosition.TargetPosition.Coordinates;
                    var enemyRangeCount = inEnemyRangeCountHeatmap.Heatmap[coordinates.x][coordinates.y];
                    if (minimize)
                    {
                        if (enemyRangeCount < minReachableEnemies)
                        {
                            minReachableEnemies = enemyRangeCount;
                            var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, coordinates);
                            optimalReachedUnitsArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel, posDelta);
                        }
                    }
                    else
                    {
                        if (enemyRangeCount > maxReachableEnemies)
                        {
                            maxReachableEnemies = enemyRangeCount;
                            var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, coordinates);
                            optimalReachedUnitsArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel, posDelta);
                        }
                    }
                }
            }

            return optimalReachedUnitsArgs;
        }
        */
        
        private CommandArgs GetByPositionBuffStrategy(bool minimize)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            int bestScore = int.MinValue;
            int worstScore = int.MaxValue;
            CommonArgs optimalArgs = null;

            var validPaths = BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions
                .Where(p => !p.TargetPosition.IsOccupied() && p.TargetPosition.IsWalkable()).ToList();

            if (validPaths.Count == 0)
                return null;
            
            validPaths.Shuffle();

            foreach (var potentialTargetPosition in validPaths)
            {
                var positionScore = GetPositionContentTypeScore(potentialTargetPosition.TargetPosition.ContentType);

                if (minimize)
                {
                    if (positionScore < worstScore)
                    {
                        worstScore = positionScore;
                        var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, potentialTargetPosition.TargetPosition.Coordinates);
                        optimalArgs = new CommonArgs(activeUnit.UnitData, null, BattleStateModel, posDelta);
                    }
                }
                else
                {
                    if (positionScore > bestScore)
                    {
                        bestScore = positionScore;
                        var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, potentialTargetPosition.TargetPosition.Coordinates);
                        optimalArgs = new CommonArgs(activeUnit.UnitData, null, BattleStateModel, posDelta);
                    }
                }
            }

            return optimalArgs;
        }

        private int GetPositionContentTypeScore(BattleScenePositionContentType type)
        {
            switch (type)
            {
                case BattleScenePositionContentType.Obstacle:
                case BattleScenePositionContentType.ObstacleTimed:
                    return -1;
                case BattleScenePositionContentType.Unstable:
                    return 0;
                case BattleScenePositionContentType.None:
                    return 1;
                case BattleScenePositionContentType.DefensiveTimed:
                    return 2;
                case BattleScenePositionContentType.Defensive:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        #endregion

    }
}