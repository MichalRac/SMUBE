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
using SMUBE.Units;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneMoveToOneArgsPicker : ArgsPicker
    {
        enum Stage
        {
            ChooseTargetUnit,
            ChooseMoveSpot,
        }

        private Stage _currentStage = Stage.ChooseTargetUnit;
        private UnitData _targetUnitData;
   
        private List<SMUBEVector2<int>> ValidTargets = new List<SMUBEVector2<int>>();
        private int _currentTargetIndex = 0;

        private List<SMUBEVector2<int>> ValidMoveTargets = new List<SMUBEVector2<int>>();
        private int _currentMoveIndex = 0;

        public OneMoveToOneArgsPicker(ICommand command, BattleStateModel battleStateModel)
            : base(command, battleStateModel)
        {
            SetDefaultTarget();
        }

        protected override void SetDefaultTarget()
        {
            ValidTargets.Clear();
            ValidMoveTargets.Clear();

            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (IsStageOneValid(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    ValidTargets.Add(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates);
                    _targetUnitData = potentialTargetUnit.UnitData;
                    ValidMoveTargets = GetReachableSurroundingPositions(_targetUnitData.BattleScenePosition.Coordinates)
                        .Select(p => p.Coordinates).ToList();
                }
            }
        }

        public override bool IsAnyValid()
        {
            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (IsStageOneValid(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Submit()
        {
            if (_currentStage == Stage.ChooseTargetUnit)
            {
                ValidMoveTargets.Clear();
                ValidMoveTargets = GetReachableSurroundingPositions(ValidTargets[_currentTargetIndex])
                    .Select(p => p.Coordinates).ToList();

                if (ValidMoveTargets.Count > 0)
                {
                    _currentMoveIndex = 0;
                    _currentStage = Stage.ChooseMoveSpot;
                }
                else
                {
                    ArgsInvalid?.Invoke();
                }
            }
            else if (_currentStage == Stage.ChooseMoveSpot)
            {
                var currentCommandArgs = GetCommandArgs();
                
                if (Command.CommandArgsValidator.Validate(currentCommandArgs, BattleStateModel))
                {
                    ArgsConfirmed?.Invoke(currentCommandArgs);
                    IsResolved = true;
                }
                else
                {
                    ArgsInvalid?.Invoke();
                }
            }
            else
            {
                ArgsInvalid?.Invoke();
            }
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

            PositionDelta posDelta = null;
            if (_currentStage == Stage.ChooseMoveSpot)
            {
                posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, 
                    activeUnit.UnitData.BattleScenePosition.Coordinates,ValidMoveTargets[_currentMoveIndex]);
            }
            
            return new CommonArgs(activeUnit.UnitData, targetUnitData, BattleStateModel, posDelta);
        }

        public override void Return()
        {
            if (_currentStage == Stage.ChooseMoveSpot)
            {
                OperationCancelled?.Invoke();
            }
            else if (_currentStage == Stage.ChooseTargetUnit)
            {
                ValidMoveTargets.Clear();
                _currentStage = Stage.ChooseTargetUnit;
            }
            else
            {
                throw new ArgumentException();
            }
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
            switch (_currentStage)
            {
                case Stage.ChooseTargetUnit:
                    TryMoveToOffset(xDelta, yDelta);
                    break;
                case Stage.ChooseMoveSpot:
                    if (xDelta == 1 || yDelta == 1)
                    {
                        _currentMoveIndex++;

                        if (_currentMoveIndex >= ValidMoveTargets.Count)
                        {
                            _currentMoveIndex = 0;
                        }
                    }
                    else
                    {
                        _currentMoveIndex--;

                        if (_currentMoveIndex < 0)
                        {
                            _currentMoveIndex = ValidMoveTargets.Count -1;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ArgsUpdated?.Invoke(GetCommandArgs());
        }

        private void TryMoveToOffset(int xDelta, int yDelta)
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
        }

        private bool IsStageOneValid(SMUBEVector2<int> targetPos)
        {
            if (!ContainsValidTarget(targetPos))
            {
                return false;
            }
            
            if (!IsTargetReachable(targetPos))
            {
                return false;
            }
            
            return true;
        }

        private bool ContainsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier == null)
            {
                return false;
            }
            
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.None)
            {
                return true;
            }
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.OtherUnit)
            {
                var sameId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.Equals(BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier);
                return !sameId;
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
        
        private bool IsTargetReachable(SMUBEVector2<int> targetPos)
        {
            var reachableSurroundingPositions = GetReachableSurroundingPositions(targetPos);
            return reachableSurroundingPositions.Count > 0;
        }

        private List<BattleScenePosition> GetReachableSurroundingPositions(SMUBEVector2<int> targetPos)
        {
            var targetBattleScenePosition = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y];
            var reachableSurroundingPositions = BattleStateModel.BattleSceneState
                .PathfindingHandler.GetReachableSurroundingPositions(BattleStateModel, targetBattleScenePosition, BattleStateModel.ActiveUnit.UnitData.UnitIdentifier);
            return reachableSurroundingPositions;
        }
        
        public override string GetPickerInfo()
        {
            string argString;

            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Ally)
                argString = "ally";
            else if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Enemy)
                argString = "enemy";
            else
                throw new ArgumentOutOfRangeException();
            
            switch (_currentStage)
            {
                case Stage.ChooseTargetUnit:
                    return $"Pick a reachable {argString} unit!";
                case Stage.ChooseMoveSpot:
                    return "Pick a reachable position next to target unit!";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetPickerState()
        {
            switch (_currentStage)
            {
                case Stage.ChooseTargetUnit:
                    return "Valid Target! Confirm to select it!";
                case Stage.ChooseMoveSpot:
                    return "Confirm to select target attack position, or change to another one if available!";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
                case ArgsEnemyTargetingPreference.MostDmgDealt:
                    return GetMaxDmgDealtStrategy();
                case ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange:
                    return GetByEnemyWithTeamIdInRangeCountStrategy(BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId, false);
                case ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn:
                    return GetByReachableEnemiesAfterTurnStrategy(true);
                case ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn:
                    return GetByReachableEnemiesAfterTurnStrategy(false);
                case ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn:
                    return GetByPositionBuffStrategy(true);
                case ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn:
                    return GetByPositionBuffStrategy(false);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // unused
            /*
            else if (argsPreferences.MovementTargetingPreference != ArgsMovementTargetingPreference.None)
            {
                switch (argsPreferences.MovementTargetingPreference)
                {
                    case ArgsMovementTargetingPreference.None:
                        return GetAnyStrategy();
                    case ArgsMovementTargetingPreference.GetOutOfReach:
                        return GetAnyStrategy();
                    case ArgsMovementTargetingPreference.GetCloserCarefully:
                        return GetByEnemyWithTeamIdInRangeCountStrategy(BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0, true);
                    case ArgsMovementTargetingPreference.GetCloserAggressively:
                        return GetAnyStrategy();
                    case ArgsMovementTargetingPreference.OptimizeFortifiedPosition:
                        return GetByPositionBuffStrategy(true);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            */
        }

        #region AIArgStrategies
        
        private List<Unit> GetValidTargets(Unit activeUnit)
        {
            List<Unit> validTargets;
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Enemy)
            {
                validTargets = BattleStateModel.GetTeamUnits(activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0)
                    .Where(u => u.UnitData.UnitStats.IsAlive()).ToList();
            }
            else if(Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.OtherUnit)
            {
                validTargets = BattleStateModel.Units.Where(u => u.UnitData.UnitStats.IsAlive() 
                                                                 && !u.UnitData.UnitIdentifier.Equals(activeUnit.UnitData.UnitIdentifier)).ToList();
            }
            else
            {
                throw new ArgumentException();
            }

            return validTargets;
        }

        private CommandArgs GetAnyStrategy()
        {
            var activeUnit = BattleStateModel.ActiveUnit;

            var validTargets = GetValidTargets(activeUnit);
            
            if (validTargets.Count == 0)
            {
                return null;
            }

            validTargets.Shuffle();

            foreach (var validTarget in validTargets)
            {
                var reachableSurrounding = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetReachableSurroundingPositions(BattleStateModel, validTarget.UnitData.BattleScenePosition, activeUnit.UnitData.UnitIdentifier);

                if (reachableSurrounding.Count == 0)
                    continue;

                reachableSurrounding.Shuffle();

                var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, reachableSurrounding.First().Coordinates);
                var args = new CommonArgs(activeUnit.UnitData, new List<UnitData>() { validTarget.UnitData }, BattleStateModel, posDelta);

                return args;
            }

            return null;
        }
        
        private CommandArgs GetClosestStrategy()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets = GetValidTargets(activeUnit);

            if (validTargets.Count == 0)
            {
                return null;
            }

            validTargets.Shuffle();
            
            var minDistance = int.MaxValue;
            CommonArgs minDistanceArgs = null;

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, ignoredOccupant: activeUnit.UnitData.UnitIdentifier);

                if (reachableSurroundingCache.Count == 0)
                    continue;

                foreach (var potentialTargetNode in reachableSurroundingCache)
                {
                    if (potentialTargetNode.ShortestDistance < minDistance)
                    {
                        minDistance = potentialTargetNode.ShortestDistance;
                        var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, potentialTargetNode.TargetPosition.Coordinates);
                        var args = new CommonArgs(activeUnit.UnitData, new List<UnitData>() { validTarget.UnitData }, BattleStateModel, posDelta);
                        minDistanceArgs = args;
                    }
                }
            }
            if (minDistanceArgs == null)
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetClosestStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
            }

            return minDistanceArgs;
        }
        
        private CommandArgs GetLeastHpStrategy(bool usePercentage)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets = GetValidTargets(activeUnit);

            if (validTargets.Count == 0)
            {
                return null;
            }

            var minHp = int.MaxValue;
            var minHpPercentage = float.MaxValue;
            CommonArgs minHpArgs = null;

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, ignoredOccupant: activeUnit.UnitData.UnitIdentifier);

                if (reachableSurroundingCache.Count == 0)
                    continue;

                if (!usePercentage && validTarget.UnitData.UnitStats.CurrentHealth < minHp)
                {
                    var closestSurroundingNode = reachableSurroundingCache.OrderBy(pathCache => pathCache.ShortestDistance).First();
                    minHp = validTarget.UnitData.UnitStats.CurrentHealth;
                    var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, closestSurroundingNode.TargetPosition.Coordinates);
                    var args = new CommonArgs(activeUnit.UnitData, new List<UnitData>() { validTarget.UnitData }, BattleStateModel, posDelta);
                    minHpArgs = args;
                    continue;
                }

                if (usePercentage)
                {
                    var healthPercentage = validTarget.UnitData.UnitStats.CurrentHealth / validTarget.UnitData.UnitStats.MaxHealth;
                    if (healthPercentage < minHpPercentage)
                    {
                        minHpPercentage = healthPercentage;
                        var closestSurroundingNode = reachableSurroundingCache.OrderBy(pathCache => pathCache.ShortestDistance).First();
                        var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, closestSurroundingNode.TargetPosition.Coordinates);
                        var args = new CommonArgs(activeUnit.UnitData, new List<UnitData>() { validTarget.UnitData }, BattleStateModel, posDelta);
                        minHpArgs = args;
                        continue;
                    }
                }
            }
            if (minHpArgs == null)
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetLeastHpStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
            }

            return minHpArgs;
        }
        
        private CommandArgs GetMaxDmgDealtStrategy()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets = GetValidTargets(activeUnit);

            if (validTargets.Count == 0)
            {
                return null;
            }

            var maxDmg = int.MinValue;
            CommonArgs maxDmgArgs = null;

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, ignoredOccupant: activeUnit.UnitData.UnitIdentifier);

                if (reachableSurroundingCache.Count == 0)
                    continue;

                var closestSurroundingNode = reachableSurroundingCache.OrderBy(pathCache => pathCache.ShortestDistance).First();
                var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, closestSurroundingNode.TargetPosition.Coordinates);
                var potentialArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel, posDelta);
                var commandResults = Command.GetCommandResults(potentialArgs);

                foreach (var effect in commandResults.effects)
                {
                    if (!(effect is DamageEffect damageEffect))
                    {
                        continue;
                    }

                    var finalValue = damageEffect.GetFinalValue(potentialArgs, commandResults);
                    if (finalValue > maxDmg)
                    {
                        maxDmg = finalValue;
                        maxDmgArgs = potentialArgs;
                    }
                }
            }
            if (maxDmgArgs == null)
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetMaxDmgDealtStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
            }

            return maxDmgArgs;
        }
        
        private CommandArgs GetByEnemyWithTeamIdInRangeCountStrategy(int teamIdInRange, bool minimize)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            
            var validTargets = GetValidTargets(activeUnit);

            if (validTargets.Count == 0)
            {
                return null;
            }

            var maxReachedUnits = int.MinValue;
            var minReachedUnits = int.MaxValue;
            var minDistance = int.MaxValue;
            CommonArgs optimalReachedUnitsArgs = null;

            var unitsOfTeamIdInRangeHeatmap = new CountReachingUnitsOfTeamIdHeatmap(teamIdInRange, BattleStateModel, true);
            unitsOfTeamIdInRangeHeatmap.ProcessHeatmap(BattleStateModel);

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, ignoredOccupant: activeUnit.UnitData.UnitIdentifier);

                if (reachableSurroundingCache.Count == 0)
                    continue;
                
                var closestSurroundingNode = reachableSurroundingCache.OrderBy(pathCache => pathCache.ShortestDistance).First();
                var coordinates = closestSurroundingNode.TargetPosition.Coordinates;
                var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, closestSurroundingNode.TargetPosition.Coordinates);
                var potentialArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel, posDelta);

                if (minimize)
                {
                    if (unitsOfTeamIdInRangeHeatmap.Heatmap[coordinates.x][coordinates.y] < minReachedUnits)
                    {
                        minReachedUnits = unitsOfTeamIdInRangeHeatmap.Heatmap[coordinates.x][coordinates.y];
                        minDistance = closestSurroundingNode.ShortestDistance;
                        optimalReachedUnitsArgs = potentialArgs;
                    }
                    else if (unitsOfTeamIdInRangeHeatmap.Heatmap[coordinates.x][coordinates.y] == minReachedUnits)
                    {
                        if (closestSurroundingNode.ShortestDistance < minDistance)
                        {
                            minDistance = closestSurroundingNode.ShortestDistance;
                            optimalReachedUnitsArgs = potentialArgs;
                        }
                    }

                }
                else
                {
                    if (unitsOfTeamIdInRangeHeatmap.Heatmap[coordinates.x][coordinates.y] > maxReachedUnits)
                    {
                        maxReachedUnits = unitsOfTeamIdInRangeHeatmap.Heatmap[coordinates.x][coordinates.y];
                        minDistance = closestSurroundingNode.ShortestDistance;
                        optimalReachedUnitsArgs = potentialArgs;
                    }
                    else if (unitsOfTeamIdInRangeHeatmap.Heatmap[coordinates.x][coordinates.y] == maxReachedUnits)
                    {
                        if (closestSurroundingNode.ShortestDistance < minDistance)
                        {
                            minDistance = closestSurroundingNode.ShortestDistance;
                            optimalReachedUnitsArgs = potentialArgs;
                        }
                    }
                }
            }
            if (optimalReachedUnitsArgs == null)
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetByEnemyWithTeamIdInRangeCountStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
            }

            return optimalReachedUnitsArgs;
        }
        
        private CommandArgs GetByReachableEnemiesAfterTurnStrategy(bool minimize)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var enemyTeamId = activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;

            var validTargets = GetValidTargets(activeUnit);

            if (validTargets.Count == 0)
            {
                return null;
            }
            
            validTargets.Shuffle();
            
            var minReachableEnemies = int.MaxValue;
            var maxReachableEnemies = int.MinValue;
            CommonArgs optimalReachedUnitsArgs = null;
            
            var inEnemyRangeCountHeatmap = new CountReachingUnitsOfTeamIdHeatmap(enemyTeamId, BattleStateModel, true);
            inEnemyRangeCountHeatmap.ProcessHeatmap(BattleStateModel);

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, ignoredOccupant: activeUnit.UnitData.UnitIdentifier);

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

            if (optimalReachedUnitsArgs == null)
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetByReachableEnemiesAfterTurnStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
            }

            return optimalReachedUnitsArgs;
        }
        
        private CommandArgs GetByPositionBuffStrategy(bool minimize)
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var enemyTeamId = activeUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;

            var validTargets = GetValidTargets(activeUnit);

            if (validTargets.Count == 0)
            {
                return null;
            }
            
            validTargets.Shuffle();

            int bestScore = int.MinValue;
            int worstScore = int.MaxValue;
            CommonArgs optimalArgs = null;
            
            var inEnemyRangeCountHeatmap = new CountReachingUnitsOfTeamIdHeatmap(enemyTeamId, BattleStateModel, true);
            inEnemyRangeCountHeatmap.ProcessHeatmap(BattleStateModel);

            foreach (var validTarget in validTargets)
            {
                var reachableSurroundingCache = BattleStateModel.BattleSceneState.PathfindingHandler
                    .GetSurroundingPathCache(BattleStateModel, validTarget.UnitData.BattleScenePosition, ignoredOccupant: activeUnit.UnitData.UnitIdentifier);

                if (reachableSurroundingCache.Count == 0)
                    continue;
                
                reachableSurroundingCache.Shuffle();

                foreach (var potentialTargetPosition in reachableSurroundingCache)
                {
                    var positionScore = GetPositionContentTypeScore(potentialTargetPosition.TargetPosition.ContentType);

                    if (minimize)
                    {
                        if (positionScore < worstScore)
                        {
                            worstScore = positionScore;
                            var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, potentialTargetPosition.TargetPosition.Coordinates);
                            optimalArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel, posDelta);
                        }
                    }
                    else
                    {
                        if (positionScore > bestScore)
                        {
                            bestScore = positionScore;
                            var posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, potentialTargetPosition.TargetPosition.Coordinates);
                            optimalArgs = new CommonArgs(activeUnit.UnitData, new List<UnitData> { validTarget.UnitData }, BattleStateModel, posDelta);
                        }
                    }
                }
            }
            if (optimalArgs == null)
            {
                var fallback = GetAnyStrategy();
                fallback.DebugSource = $"{nameof(OneMoveToPositionArgsPicker)}:{nameof(GetByPositionBuffStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                return fallback;
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