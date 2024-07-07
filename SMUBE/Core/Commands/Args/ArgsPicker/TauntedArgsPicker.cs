using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class TauntedArgsPicker : ArgsPicker
    {
        private SMUBEVector2<int> _target;

        public TauntedArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
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
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (ContainsValidTarget(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    _target = potentialTargetUnit.UnitData.BattleScenePosition.Coordinates;
                    return;
                }
            }
        }

        public override bool IsAnyValid()
        {
            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (IsValid(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
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
            var currentTargetCoordinates = _target;
            if (BattleStateModel.BattleSceneState.Grid[currentTargetCoordinates.x, currentTargetCoordinates.y].UnitIdentifier != null)
            {
                var targetUnitId = BattleStateModel.BattleSceneState.Grid[currentTargetCoordinates.x, currentTargetCoordinates.y].UnitIdentifier;
                targetUnitData.Add(BattleStateModel.Units.Find(u => u.UnitData.UnitIdentifier.Equals(targetUnitId)).UnitData);
            }
            
            return new CommonArgs(activeUnit.UnitData, targetUnitData, BattleStateModel);
        }

        public override void Up()
        {
        }

        public override void Down()
        {
        }

        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override string GetPickerInfo()
        {
            return "Pick target";
        }

        public override string GetPickerState()
        {
            return "Pick target";
        }

        private bool IsValid(SMUBEVector2<int> targetPos)
        {
            if (!ContainsValidTarget(targetPos))
            {
                return false;
            }
            
            if (!IsTargetReachable(targetPos) && !AnyPathExists(targetPos))
            {
                return false;
            }
            
            return true;
        }
    
        private bool AnyPathExists(SMUBEVector2<int> targetPos)
        {
            var startBattleScenePosition = BattleStateModel.ActiveUnit.UnitData.BattleScenePosition;
            var targetBattleScenePosition = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y];

            if (startBattleScenePosition.Coordinates.Equals(targetBattleScenePosition.Coordinates))
            {
                return true;
            }

            var surrounding = BattleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(BattleStateModel, targetBattleScenePosition);
            foreach (var surroundingPos in surrounding)
            {
                var pathExists = BattleStateModel.BattleSceneState.PathfindingHandler.PathExists(BattleStateModel, startBattleScenePosition, surroundingPos, out _);
                if (pathExists)
                {
                    return true;
                }
            }
            return false;
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
        
        private bool ContainsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier == null)
            {
                return false;
            }
            
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.OtherUnit)
            {
                var sameId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.Equals(BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier);
                if (sameId)
                {
                    return false;
                }
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

            return true;
        }
        
        
        public override CommandArgs GetSuggestedArgs(ArgsPreferences _)
        {
            var unit = BattleStateModel.ActiveUnit;
            var viableTargets = unit.UnitCommandProvider.ViableTargets;
            if (viableTargets != null && viableTargets.Count > 0)
            {
                if (BattleStateModel.TryGetUnit(viableTargets[0], out var targetUnit))
                {
                    return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnit.UnitData }, BattleStateModel);
                }
            }
            return null;
        }
    }
}