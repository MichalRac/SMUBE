using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.BattleState.Heatmap.GeneralHeatmaps;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;
using SMUBE.Units;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToPositionArgsPicker : ArgsPicker
    {
        private readonly bool _allowOccupied;
        private readonly bool _allowSpecial;
        private readonly bool _allowWalkable;
        private readonly bool _allowNonWalkable;

        private SMUBEVector2<int> _currentTargetCoordinates;

        public OneToPositionArgsPicker(ICommand command, BattleStateModel battleStateModel, 
            bool allowOccupied, bool allowSpecial, bool allowWalkable, bool allowNonWalkable) 
            : base(command, battleStateModel)
        {
            _allowOccupied = allowOccupied;
            _allowSpecial = allowSpecial;
            _allowWalkable = allowWalkable;
            _allowNonWalkable = allowNonWalkable;
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
            var validTargets = BattleStateModel.BattleSceneState.PathfindingHandler
                .GetAllReachablePathsForActiveUnit(BattleStateModel).Where(target => !target.TargetPosition.Coordinates.Equals(BattleStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates)).ToList();

            if (validTargets.Any())
            {
                _currentTargetCoordinates = validTargets.First().TargetPosition.Coordinates;
            }
        }

        public override bool IsAnyValid()
        {
            var activeUnitPosition = BattleStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates;
            return BattleStateModel.BattleSceneState.PathfindingHandler.GetAllReachablePathsForActiveUnit(BattleStateModel)
                .Count(target => !target.TargetPosition.Coordinates.Equals(activeUnitPosition)) 
                   > 0;
        }

        public override CommandArgs GetCommandArgs()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var targetPositions = new List<SMUBEVector2<int>>() { _currentTargetCoordinates };
            return new CommonArgs(activeUnit.UnitData, null, BattleStateModel, null, targetPositions);
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
            return "Pick any empty position!";
        }

        public override string GetPickerState()
        {
            return IsResolved
                ? "Resolved"
                : "Pick any empty position";
        }

        private bool IsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (targetPos.x < 0 || targetPos.x >= BattleStateModel.BattleSceneState.Width)
                return false;
            if (targetPos.y < 0 || targetPos.y >= BattleStateModel.BattleSceneState.Height)
                return false;

            var targetPosition = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y];

            
            if (!_allowSpecial && targetPosition.IsSpecial())
            {
                return false;
            }
            
            if (!_allowOccupied && targetPosition.IsOccupied())
            {
                return false;
            }

            if (!_allowWalkable && targetPosition.IsWalkable())
            {
                return false;
            }

            if (!_allowNonWalkable && !targetPosition.IsWalkable())
            {
                return false;
            }

            return true;
        }
        
        public override CommandArgs GetSuggestedArgs(ArgsPreferences argsPreferences)
        {
            if (argsPreferences.MovementTargetingPreference != ArgsMovementTargetingPreference.None
                || argsPreferences.TargetingPreference != ArgsEnemyTargetingPreference.None)
            {
                return null;
            }

            CommandArgs results = null;
            switch (argsPreferences.PositionTargetingPreference)
            {
                case ArgsPositionTargetingPreference.None:
                    results = GetAnyStrategy();
                    break;
                case ArgsPositionTargetingPreference.OnLeastHpPercentageAlly:
                    if (!_allowOccupied)
                    {
                        results = GetByAllyHpStrategy(true, true);
                    }
                    else
                    {
                        results = GetAnyStrategy();
                    }
                    break;
                case ArgsPositionTargetingPreference.OnMostHpPercentageAlly:
                    if (!_allowOccupied) 
                    {
                        results = GetByAllyHpStrategy(false, true);
                    }
                    else
                    {
                        results = GetAnyStrategy();
                    }
                    break;
                case ArgsPositionTargetingPreference.NextToClosestEnemy:
                    results = GetByCloseToClosestEnemyStrategy();
                    break;
                case ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach:
                    if (!_allowOccupied)
                    {
                        results = GetByAllyWithMostEnemiesInReachStrategy();
                    }
                    else
                    {
                        results = GetAnyStrategy();
                    }
                    break;
                case ArgsPositionTargetingPreference.InBetweenEnemies:
                    if (!_allowOccupied)
                    {
                        results = GetByAverageEnemyPositionStrategy();
                    }
                    else
                    {
                        results = GetAnyStrategy();
                    }
                    break; 
                case ArgsPositionTargetingPreference.InBetweenTeams:
                    // todo in between teams
                    return GetByInBetweenTeamsStrategy();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return results ?? GetAnyStrategy();
        }

        #region AIArgStrategies
 
        private CommandArgs GetAnyStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var validTargets = BattleStateModel.BattleSceneState.AggregateAllPositions(true);
            
            if (!_allowSpecial)
            {
                validTargets = validTargets.Where(target => !target.IsSpecial()).ToList();
            }

            if (!_allowOccupied)
            {
                validTargets = validTargets.Where(target => !target.IsOccupied()).ToList();
            }

            if (validTargets.Count == 0)
            {
                return null;
            }
                
            validTargets.Shuffle();
            var targetPos = validTargets.First().Coordinates;
                    
            return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{targetPos});
        }
        
        private CommandArgs GetByAllyHpStrategy(bool minimize, bool byPercentage)
        {
            var unit = BattleStateModel.ActiveUnit;
            var validTargets = BattleStateModel.GetTeamUnits(unit.UnitData.UnitIdentifier.TeamId);

            if (validTargets.Count == 0)
            {
                return null;
            }
                
            validTargets.Shuffle();

            var minHp = int.MaxValue;
            var maxHp = int.MinValue;
            var minHpPercentage = float.MaxValue;
            var maxHpPercentage = float.MinValue;

            SMUBEVector2<int> optimalPos = validTargets.First().UnitData.BattleScenePosition.Coordinates;
            
            foreach (var validTarget in validTargets)
            {
                var targetHp = validTarget.UnitData.UnitStats.CurrentHealth;
                var targetHpPercentage = targetHp / validTarget.UnitData.UnitStats.MaxHealth;
                
                if (!_allowSpecial && validTarget.UnitData.BattleScenePosition.IsSpecial())
                {
                    continue;
                }

                if (minimize)
                {
                    if (!byPercentage && targetHp < minHp)
                    {
                        minHp = targetHp;
                        optimalPos = validTarget.UnitData.BattleScenePosition.Coordinates;
                    }

                    if (byPercentage && targetHpPercentage < minHpPercentage)
                    {
                        minHpPercentage = targetHpPercentage;
                        optimalPos = validTarget.UnitData.BattleScenePosition.Coordinates;
                    }
                }
                else
                {
                    if (!byPercentage && targetHp > maxHp)
                    {
                        maxHp = targetHp;
                        optimalPos = validTarget.UnitData.BattleScenePosition.Coordinates;
                    }

                    if (byPercentage && targetHpPercentage > maxHpPercentage)
                    {
                        maxHpPercentage = targetHpPercentage;
                        optimalPos = validTarget.UnitData.BattleScenePosition.Coordinates;
                    }
                }
            }
                    
            return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{optimalPos});
        }
        
        private CommandArgs GetByCloseToClosestEnemyStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var validTargets = BattleStateModel.GetTeamUnits(enemyTeamId);
            
            if (validTargets.Count == 0)
            {
                return null;
            }
                
            validTargets.Shuffle();

            var distanceToAnyAllyHeatmap = new DistanceToUnitOfTeamIdHeatmap(unit.UnitData.UnitIdentifier.TeamId, false, BattleStateModel);
            distanceToAnyAllyHeatmap.ProcessHeatmap(BattleStateModel);
            
            var minDistance = int.MaxValue;
            BattleScenePosition closestUnitPosition = validTargets.First().UnitData.BattleScenePosition;
            
            foreach (var validTarget in validTargets)
            {
                var coordinates = validTarget.UnitData.BattleScenePosition.Coordinates;
                var distance = distanceToAnyAllyHeatmap.Heatmap[coordinates.x][coordinates.y];
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestUnitPosition = validTarget.UnitData.BattleScenePosition;
                }
            }

            var surroundingPositions = BattleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(BattleStateModel, closestUnitPosition);

            if (!_allowSpecial)
            {
                surroundingPositions = surroundingPositions.Where(pos => !pos.IsSpecial()).ToList();
            }
            
            if (surroundingPositions.Count == 0)
            {
                var nonEmptyNeighbours = BattleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(BattleStateModel, closestUnitPosition, false);
                nonEmptyNeighbours.Shuffle();

                foreach (var nonEmptyNeighbour in nonEmptyNeighbours)
                {
                    surroundingPositions = BattleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(BattleStateModel, nonEmptyNeighbour);
                   
                    if (!_allowSpecial)
                    {
                        surroundingPositions = surroundingPositions.Where(pos => !pos.IsSpecial()).ToList();
                    }
                   
                    if (surroundingPositions.Any())
                    {
                        break;
                    }
                }
                
                if (!surroundingPositions.Any())
                {
                    var fallback = GetAnyStrategy();
                    fallback.DebugSource = $"{nameof(OneToPositionArgsPicker)}:{nameof(GetByCloseToClosestEnemyStrategy)}-fallback:{nameof(GetAnyStrategy)}";
                    return fallback;
                }
            }
            
            var randomSurrounding = surroundingPositions.GetRandom();
            return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{randomSurrounding.Coordinates});
        }
        
        private CommandArgs GetByAllyWithMostEnemiesInReachStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var validTargets = BattleStateModel.GetTeamUnits(unit.UnitData.UnitIdentifier.TeamId);
            
            if (validTargets.Count == 0)
            {
                return null;
            }
            
            var enemyReachHeatmap = new CountReachingUnitsOfTeamIdHeatmap(enemyTeamId, BattleStateModel, true);
            enemyReachHeatmap.ProcessHeatmap(BattleStateModel);
            
            var maxReachingEnemies = int.MinValue;
            BattleScenePosition optimalTargetPosition = validTargets.First().UnitData.BattleScenePosition;
            
            foreach (var validTarget in validTargets)
            {
                var coordinates = validTarget.UnitData.BattleScenePosition.Coordinates;
                var reachingEnemies = enemyReachHeatmap.Heatmap[coordinates.x][coordinates.y];

                if (!_allowSpecial && validTarget.UnitData.BattleScenePosition.IsSpecial())
                {
                    continue;
                }

                if (reachingEnemies > maxReachingEnemies)
                {
                    maxReachingEnemies = reachingEnemies;
                    optimalTargetPosition = validTarget.UnitData.BattleScenePosition;
                }
            }
            
            return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{optimalTargetPosition.Coordinates});
        }

        private CommandArgs GetByAverageEnemyPositionStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var validTargets = BattleStateModel.GetTeamUnits(enemyTeamId);
            
            if (validTargets.Count == 0)
            {
                return null;
            }

            var x = 0;
            var y = 0;
            
            foreach (var validTarget in validTargets)
            {
                x += validTarget.UnitData.BattleScenePosition.Coordinates.x;
                y += validTarget.UnitData.BattleScenePosition.Coordinates.y;
            }

            x /= validTargets.Count;
            y /= validTargets.Count;

            var potentialPosition = BattleStateModel.BattleSceneState.Grid[x, y];

            if ((!_allowSpecial && potentialPosition.IsSpecial()) 
                || (!_allowOccupied && potentialPosition.IsOccupied())
                || (!_allowNonWalkable && !potentialPosition.IsWalkable())
                || (!_allowWalkable && potentialPosition.IsWalkable()))
            {
                var surrounding = BattleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(BattleStateModel, potentialPosition);
                surrounding.Shuffle();

                foreach (var potentialSurroundingPosition in surrounding)
                {
                    if (!_allowSpecial && potentialSurroundingPosition.IsSpecial())
                    {
                        continue;
                    }
                    if (!_allowOccupied && potentialPosition.IsOccupied())
                    {
                        continue;
                    }
                    if (!_allowWalkable && potentialPosition.IsWalkable())
                    {
                        continue;
                    }
                    if (!_allowNonWalkable && !potentialPosition.IsWalkable())
                    {
                        continue;
                    }
                    return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{potentialSurroundingPosition.Coordinates});
                }
            }
            else
            {
                return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{potentialPosition.Coordinates});
            }

            var fallback = GetAnyStrategy();
            fallback.DebugSource = $"{nameof(OneToPositionArgsPicker)}:{nameof(GetByAverageEnemyPositionStrategy)}-fallback:{nameof(GetAnyStrategy)}";
            return fallback;
        }

        private CommandArgs GetByInBetweenTeamsStrategy()
        {
            var unit = BattleStateModel.ActiveUnit;
            var distanceToTeam1Heatmap = new DistanceToUnitOfTeamIdHeatmap(0, false, BattleStateModel);
            var distanceToTeam2Heatmap = new DistanceToUnitOfTeamIdHeatmap(1, false, BattleStateModel);
            var inBetweenTeamsHeatmap = new InBetweenTeamsHeatmap(BattleStateModel, distanceToTeam1Heatmap, distanceToTeam2Heatmap);
            distanceToTeam1Heatmap.ProcessHeatmap(BattleStateModel);
            distanceToTeam2Heatmap.ProcessHeatmap(BattleStateModel);
            inBetweenTeamsHeatmap.ProcessHeatmap(BattleStateModel);
            
            var minValue = int.MaxValue;
            SMUBEVector2<int> optimalTarget = null;
            
            for (int x = 0; x < BattleStateModel.BattleSceneState.Width; x++)
            {
                for (int y = 0; y < BattleStateModel.BattleSceneState.Height; y++)
                {
                    var position = BattleStateModel.BattleSceneState.Grid[x, y];
                    
                    if (!_allowSpecial && position.IsSpecial())
                    {
                        continue;
                    }
                    if (!_allowOccupied && position.IsOccupied())
                    {
                        continue;
                    }
                    if (!_allowWalkable && position.IsWalkable())
                    {
                        continue;
                    }
                    if (!_allowNonWalkable && !position.IsWalkable())
                    {
                        continue;
                    }
                    
                    var value = inBetweenTeamsHeatmap.Heatmap[x][y];
                    if (value < minValue)
                    {
                        minValue = value;
                        optimalTarget = position.Coordinates;
                    }

                }
            }

            return optimalTarget == null 
                ? GetAnyStrategy() 
                : new CommonArgs(unit.UnitData, null, BattleStateModel, 
                    null, new List<SMUBEVector2<int>>{optimalTarget});
        }


        #endregion
    }
}